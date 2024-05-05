using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace MYTOOL.Tools
{
    public static class Utility
    {
        #region >> 时间[戳]

        private readonly static DateTime UTC_1970_1_1 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 获取UTC时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="isMillisecond">是否毫秒</param>
        /// <returns></returns>
        public static long GetUtcTimeStamp(DateTime dt, bool isMillisecond = true)
        {
            TimeSpan ts = TimeZoneInfo.ConvertTimeToUtc(dt) - UTC_1970_1_1;
            return isMillisecond ? Convert.ToInt64(ts.TotalMilliseconds) : Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// 获取当前UTC时间戳
        /// </summary>
        /// <param name="isMillisecond">是否毫秒</param>
        /// <returns>当前时间戳</returns>
        public static long GetCurrentUtcTimeStamp(bool isMillisecond = true)
        {
            TimeSpan ts = DateTime.UtcNow - UTC_1970_1_1;
            return isMillisecond ? Convert.ToInt64(ts.TotalMilliseconds) : Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// UTC时间戳转本地时间
        /// </summary>
        /// <param name="timeStamp">时间戳(秒或毫秒)</param>
        /// <param name="isMillisecond">是否毫秒</param>
        /// <returns></returns>
        public static DateTime ConvertTimeStampToDateTime(long timeStamp, bool isMillisecond = true)
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(UTC_1970_1_1, TimeZoneInfo.Local); // 当地时区
            return isMillisecond ? startTime.AddMilliseconds(timeStamp) : startTime.AddSeconds(timeStamp);
        }

        #endregion 时间[戳]


        #region >> 3DES

        //3DES自定义密匙
        private static readonly byte[] DES_KEYS = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// 使用3DES加密
        /// </summary>
        /// <param name="data">待加密字节</param>
        /// <param name="encryptKey">加密口令（长度为8）</param>
        /// <param name="encryptIV"></param>
        /// <returns>加密后的字节</returns>
        public static byte[] DESEncrypt(byte[] data, string encryptKey, string encryptIV = null)
        {
            return DESEncrypt(data, Encoding.UTF8.GetBytes(encryptKey), encryptIV == null ? null : Encoding.UTF8.GetBytes(encryptIV));
        }

        /// <summary>
        /// 使用3DES加密
        /// </summary>
        /// <param name="data">待加密字节</param>
        /// <param name="encryptKey">加密口令（长度为8）</param>
        /// <param name="encryptIV"></param>
        /// <returns>加密后的字节</returns>
        public static byte[] DESEncrypt(byte[] data, string encryptKey, byte[] encryptIV = null)
        {
            return DESEncrypt(data, Encoding.UTF8.GetBytes(encryptKey), encryptIV);
        }

        /// <summary>
        /// 使用3DES加密
        /// </summary>
        /// <param name="data">待加密字节</param>
        /// <param name="encryptKey">加密口令（长度为8）</param>
        /// <param name="encryptIV"></param>
        /// <returns>加密后的字节</returns>
        public static byte[] DESEncrypt(byte[] data, byte[] encryptKey, byte[] encryptIV = null)
        {
            encryptIV ??= DES_KEYS;
            using var des = DES.Create();
            using var mStream = new MemoryStream();
            using var cStream = new CryptoStream(mStream, des.CreateEncryptor(encryptKey, encryptIV), CryptoStreamMode.Write);
            cStream.Write(data, 0, data.Length);
            cStream.FlushFinalBlock();
            return mStream.ToArray();
        }


        /// <summary>
        /// 使用3DES解密
        /// </summary>
        /// <param name="data">待解密字节</param>
        /// <param name="decryptKey">解密口令（长度为8）</param>
        /// <param name="decryptIV"></param>
        /// <returns>解密后的字节</returns>
        public static byte[] DESDecrypt(byte[] data, string decryptKey, string decryptIV = null)
        {
            return DESDecrypt(data, Encoding.UTF8.GetBytes(decryptKey), decryptIV == null ? null : Encoding.UTF8.GetBytes(decryptIV));
        }

        /// <summary>
        /// 使用3DES解密
        /// </summary>
        /// <param name="data">待解密字节</param>
        /// <param name="decryptKey">解密口令（长度为8）</param>
        /// <param name="decryptIV"></param>
        /// <returns>解密后的字节</returns>
        public static byte[] DESDecrypt(byte[] data, string decryptKey, byte[] decryptIV = null)
        {
            return DESDecrypt(data, Encoding.UTF8.GetBytes(decryptKey), decryptIV);
        }

        /// <summary>
        /// 使用3DES解密
        /// </summary>
        /// <param name="data">待解密字节</param>
        /// <param name="decryptKey">解密口令（长度为8）</param>
        /// <param name="decryptIV"></param>
        /// <returns>解密后的字节</returns>
        public static byte[] DESDecrypt(byte[] data, byte[] decryptKey, byte[] decryptIV = null)
        {
            decryptIV ??= DES_KEYS;
            using var des = DES.Create();
            using var mStream = new MemoryStream();
            using var cStream = new CryptoStream(mStream, des.CreateDecryptor(decryptKey, decryptIV), CryptoStreamMode.Write);
            cStream.Write(data, 0, data.Length);
            cStream.FlushFinalBlock();
            return mStream.ToArray();
        }

        #endregion 3DES


        #region >> AES

        //AES自定义密匙
        private static readonly byte[] AES_KEYS = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// 对称加密算法AES(块式加密算法)
        /// </summary>
        /// <param name="data">待加密字节</param>
        /// <param name="encryptKey">加密密钥</param>
        /// <param name="encryptIV"></param>
        /// <param name="mode">加密模式</param>
        /// <returns></returns>
        public static byte[] AESEncrypt(byte[] data, string encryptKey, string encryptIV = null, CipherMode mode = CipherMode.CBC)
        {
            return AESEncrypt(data, Encoding.UTF8.GetBytes(encryptKey), encryptIV == null ? null : Encoding.UTF8.GetBytes(encryptIV), mode);
        }

        /// <summary>
        /// 对称加密算法AES(块式加密算法)
        /// </summary>
        /// <param name="data">待加密字节</param>
        /// <param name="encryptKey">加密密钥</param>
        /// <param name="encryptIV"></param>
        /// <param name="mode">加密模式</param>
        /// <returns>加密后的字节</returns>
        public static byte[] AESEncrypt(byte[] data, string encryptKey, byte[] encryptIV = null, CipherMode mode = CipherMode.CBC)
        {
            return AESEncrypt(data, Encoding.UTF8.GetBytes(encryptKey), encryptIV, mode);
        }

        /// <summary>
        /// 对称加密算法AES(块式加密算法)
        /// </summary>
        /// <param name="data">待加密字节</param>
        /// <param name="encryptKey">加密密钥</param>
        /// <param name="encryptIV"></param>
        /// <param name="mode">加密模式</param>
        /// <returns>加密后的字节</returns>
        public static byte[] AESEncrypt(byte[] data, byte[] encryptKey, byte[] encryptIV = null, CipherMode mode = CipherMode.CBC)
        {
            using var aes = Aes.Create();
            aes.Key = encryptKey;
            aes.IV = encryptIV ?? AES_KEYS;
            aes.Mode = mode;
            using var rijndaelEncrypt = aes.CreateEncryptor();
            return rijndaelEncrypt.TransformFinalBlock(data, 0, data.Length);
        }

        /// <summary>
        /// 对称加密算法AES解密
        /// </summary>
        /// <param name="data">待解密字节</param>
        /// <param name="decryptKey">解密密钥,和加密密钥相同</param>
        /// <param name="decryptIV"></param>
        /// <param name="mode">加密模式</param>
        /// <returns>解密成功返回解密后的字节,失败返回空</returns>
        public static byte[] AESDecrypt(byte[] data, string decryptKey, string decryptIV = null, CipherMode mode = CipherMode.CBC)
        {
            return AESDecrypt(data, Encoding.UTF8.GetBytes(decryptKey), decryptIV == null ? null : Encoding.UTF8.GetBytes(decryptIV), mode);
        }

        /// <summary>
        /// 对称加密算法AES解密
        /// </summary>
        /// <param name="data">待解密字节</param>
        /// <param name="decryptKey">解密密钥,和加密密钥相同</param>
        /// <param name="decryptIV"></param>
        /// <param name="mode">加密模式</param>
        /// <returns>解密成功返回解密后的字节,失败返回空</returns>
        public static byte[] AESDecrypt(byte[] data, string decryptKey, byte[] decryptIV = null, CipherMode mode = CipherMode.CBC)
        {
            return AESDecrypt(data, Encoding.UTF8.GetBytes(decryptKey), decryptIV, mode);
        }

        /// <summary>
        /// 对称加密算法AES解密
        /// </summary>
        /// <param name="data">待解密字节</param>
        /// <param name="decryptKey">解密密钥,和加密密钥相同</param>
        /// <param name="decryptIV"></param>
        /// <param name="mode">加密模式</param>
        /// <returns>解密成功返回解密后的字节,失败返回空</returns>
        public static byte[] AESDecrypt(byte[] data, byte[] decryptKey, byte[] decryptIV = null, CipherMode mode = CipherMode.CBC)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = decryptKey;
                aes.IV = decryptIV ?? AES_KEYS;
                aes.Mode = mode;
                using var rijndaelDecrypt = aes.CreateDecryptor();
                return rijndaelDecrypt.TransformFinalBlock(data, 0, data.Length);
            }
            catch
            {
                return null;
            }
        }

        #endregion AES


        #region >> MD5

        /// <summary>
        /// 从字符串获取MD5值
        /// </summary>
        /// <param name="message"></param>
        /// <returns>MD5值</returns>
        public static string MD5Hash(string message)
        {
            return MD5Hash(Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// 从字节获取MD5值
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>MD5值</returns>
        public static string MD5Hash(byte[] buffer)
        {
            return MD5Hash(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 从字节获取MD5值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns>MD5值</returns>
        public static string MD5Hash(byte[] buffer, int offset, int length)
        {
            var sb = new StringBuilder();
            using (var md5 = MD5.Create())
            {
                var data = md5.ComputeHash(buffer, offset, length);
                var count = data.Length;
                for (var i = 0; i < count; i++)
                    sb.Append(data[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 从流中获取MD5值。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>MD5值</returns>
        public static string MD5Hash(Stream stream)
        {
            var sb = new StringBuilder();
            using (var crypto = MD5.Create())
            {
                var data = crypto.ComputeHash(stream);
                var length = data.Length;
                for (var i = 0; i < length; i++)
                    sb.Append(data[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 验证MD5值。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool VerifyMD5Hash(byte[] buffer, string hash)
        {
            var hashOfInput = MD5Hash(buffer);
            return hashOfInput.CompareTo(hash.ToUpper()) == 0;
        }

        /// <summary>
        /// 验证MD5值。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool VerifyMD5Hash(string message, string hash)
        {
            return VerifyMD5Hash(Encoding.UTF8.GetBytes(message), hash);
        }

        #endregion MD5


        #region >> SHA256

        /// <summary>
        /// SHA256
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>字节数组</returns>
        public static byte[] SHA256(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            return SHA256(data, 0, data.Length);
        }

        /// <summary>
        /// SHA256
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <returns>字节数组</returns>
        public static byte[] SHA256(byte[] data)
        {
            return SHA256(data, 0, data.Length);
        }

        /// <summary>
        /// SHA256
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <returns>字节数组</returns>
        public static byte[] SHA256(byte[] data, int offset, int length)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] result = sha256.ComputeHash(data, offset, length);
            return result;
        }

        #endregion SHA256


        #region >> SHA512

        /// <summary>
        /// SHA512
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>字节数组</returns>
        public static byte[] SHA512(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            return SHA512(data, 0, data.Length);
        }

        /// <summary>
        /// SHA512
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <returns>字节数组</returns>
        public static byte[] SHA512(byte[] data)
        {
            return SHA512(data, 0, data.Length);
        }

        /// <summary>
        /// SHA512
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <returns>字节数组</returns>
        public static byte[] SHA512(byte[] data, int offset, int length)
        {
            using var sha512 = System.Security.Cryptography.SHA512.Create();
            byte[] result = sha512.ComputeHash(data, offset, length);
            return result;
        }

        #endregion SHA512


        #region >> CRC

        /// <summary>
        /// CRC-16/CCITT
        /// </summary>
        public static byte[] CRC16_CCITT(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return null;

            return CRC16_CCITT(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// CRC-16/CCITT
        /// </summary>
        public static byte[] CRC16_CCITT(byte[] buffer, int offset, int length)
        {
            if (buffer == null || buffer.Length == 0) return null;

            ushort crc = 0;// Initial value
            for (int i = offset; i < length; i++)
            {
                crc ^= buffer[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) > 0)
                    {
                        crc = (ushort)((crc >> 1) ^ 0x8408);// 0x8408 = reverse 0x1021
                    }
                    else
                    {
                        crc = (ushort)(crc >> 1);
                    }
                }
            }

            byte[] ret = BitConverter.GetBytes(crc);
            Array.Reverse(ret);
            return ret;
        }


        /// <summary>
        /// CRC-16/IBM
        /// </summary>
        public static byte[] CRC16_IBM(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return null;

            return CRC16_IBM(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// CRC-16/IBM
        /// </summary>
        public static byte[] CRC16_IBM(byte[] buffer, int offset, int length)
        {
            if (buffer == null || buffer.Length == 0) return null;

            ushort crc = 0;// Initial value
            for (int i = offset; i < length; i++)
            {
                crc ^= buffer[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) > 0)
                    {
                        crc = (ushort)((crc >> 1) ^ 0xA001);// 0xA001 = reverse 0x8005
                    }
                    else
                    {
                        crc = (ushort)(crc >> 1);
                    }
                }
            }

            byte[] ret = BitConverter.GetBytes(crc);
            Array.Reverse(ret);
            return ret;
        }


        /// <summary>
        /// CRC-16/USB
        /// </summary>
        public static byte[] CRC16_USB(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return null;

            return CRC16_USB(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// CRC-16/USB
        /// </summary>
        public static byte[] CRC16_USB(byte[] buffer, int offset, int length)
        {
            if (buffer == null || buffer.Length == 0) return null;

            ushort crc = 0xFFFF;// Initial value
            for (int i = offset; i < length; i++)
            {
                crc ^= buffer[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) > 0)
                    {
                        crc = (ushort)((crc >> 1) ^ 0xA001);// 0xA001 = reverse 0x8005
                    }
                    else
                    {
                        crc = (ushort)(crc >> 1);
                    }
                }
            }

            byte[] ret = BitConverter.GetBytes((ushort)~crc);
            Array.Reverse(ret);
            return ret;
        }


        /// <summary>
        /// CRC-16/X25 
        /// </summary>
        public static byte[] CRC16_X25(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return null;

            return CRC16_X25(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// CRC-16/X25 
        /// </summary>
        public static byte[] CRC16_X25(byte[] buffer, int offset, int length)
        {
            if (buffer == null || buffer.Length == 0) return null;

            ushort crc = 0xFFFF;// Initial value
            for (int i = offset; i < length; i++)
            {
                crc ^= buffer[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) > 0)
                    {
                        crc = (ushort)((crc >> 1) ^ 0x8408);// 0x8408 = reverse 0x1021
                    }
                    else
                    {
                        crc = (ushort)(crc >> 1);
                    }
                }
            }

            byte[] ret = BitConverter.GetBytes((ushort)~crc);
            Array.Reverse(ret);
            return ret;
        }


        /// <summary>
        /// CRC-16/XMODEM
        /// </summary>
        public static byte[] CRC16_XMODEM(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return null;

            return CRC16_XMODEM(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// CRC-16/XMODEM
        /// </summary>
        public static byte[] CRC16_XMODEM(byte[] buffer, int offset, int length)
        {
            if (buffer == null || buffer.Length == 0) return null;

            ushort crc = 0;// Initial value
            for (int i = offset; i < length; i++)
            {
                crc ^= (ushort)(buffer[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)
                    {
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    }
                    else
                    {
                        crc = (ushort)(crc << 1);
                    }
                }
            }

            byte[] ret = BitConverter.GetBytes(crc);
            Array.Reverse(ret);
            return ret;
        }


        /// <summary>
        /// CRC-32
        /// </summary>
        public static byte[] CRC32(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return null;

            return CRC32(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// CRC-32
        /// </summary>
        public static byte[] CRC32(byte[] buffer, int offset, int length)
        {
            if (buffer == null || buffer.Length == 0) return null;

            uint crc = 0xFFFFFFFF;// Initial value
            for (int i = offset; i < length; i++)
            {
                crc ^= buffer[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) > 0)
                    {
                        crc = (crc >> 1) ^ 0xEDB88320;// 0xEDB88320 = reverse 0x04C11DB7
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }

            byte[] ret = BitConverter.GetBytes(~crc);
            Array.Reverse(ret);
            return ret;
        }

        #endregion CRC


        #region >> ipv4 int 互转

        /// <summary>
        /// ipv4 to int
        /// </summary>
        /// <param name="ipv4"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static int IPv4ToInt(string ipv4)
        {
            string[] items = ipv4.Split('.');
            if (items.Length == 4)
            {
                return int.Parse(items[0].Trim()) << 24 | int.Parse(items[1].Trim()) << 16 | int.Parse(items[2].Trim()) << 8 | int.Parse(items[3].Trim());
            }

            throw new ArgumentException($"参数异常：{ipv4}");
        }

        /// <summary>
        /// int to ipv4
        /// </summary>
        /// <param name="ipInt"></param>
        /// <returns></returns>
        public static string IntToIPv4(int ipInt)
        {
            var sb = new StringBuilder();
            sb.Append((ipInt >> 24) & 0xFF).Append(".");
            sb.Append((ipInt >> 16) & 0xFF).Append(".");
            sb.Append((ipInt >> 8) & 0xFF).Append(".");
            sb.Append(ipInt & 0xFF);
            return sb.ToString();
        }

        #endregion ipv4 int 互转


        #region >> Other

        /// <summary>
        /// Knuth-Durstenfeld Shuffle打乱算法
        /// </summary>
        public static void KnuthDurstenfeld<T>(List<T> targetList)
        {
            for (int i = targetList.Count - 1; i > 0; i--)
            {
                int exchange = UnityEngine.Random.Range(0, i + 1);
                (targetList[exchange], targetList[i]) = (targetList[i], targetList[exchange]);
            }
        }

        /// <summary>
        /// Knuth-Durstenfeld Shuffle打乱算法
        /// </summary>
        public static void KnuthDurstenfeld<T>(T[] targetList)
        {
            for (int i = targetList.Length - 1; i > 0; i--)
            {
                int exchange = UnityEngine.Random.Range(0, i + 1);
                (targetList[exchange], targetList[i]) = (targetList[i], targetList[exchange]);
            }
        }

        #endregion Other
    }
}