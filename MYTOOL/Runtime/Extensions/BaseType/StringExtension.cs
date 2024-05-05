using System;
using System.Text;
using MYTOOL.Tools;

namespace MYTOOL
{
    public static class StringExtension
    {
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        /// <summary>
        /// 将字符串格式化成指定的基本数据类型
        /// </summary>
        public static bool ParseToType<T>(this string value, out object returnValue)
        {
            return ParseToType(value, typeof(T), out returnValue);
        }
        ///<summary>
        /// 将字符串格式化成指定的基本数据类型
        ///</summary>
        public static bool ParseToType(this string value, Type destinationType, out object returnValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                returnValue = default;
                return true;
            }

            switch (Type.GetTypeCode(destinationType))
            {
                case TypeCode.Boolean:
                    {
                        returnValue = bool.Parse(value);
                        return true;
                    }
                case TypeCode.Char:
                    {
                        returnValue = char.Parse(value);
                        return true;
                    }
                case TypeCode.SByte:
                    {
                        returnValue = sbyte.Parse(value);
                        return true;
                    }
                case TypeCode.Byte:
                    {
                        returnValue = byte.Parse(value); ;
                        return true;
                    }
                case TypeCode.Int16:
                    {
                        returnValue = short.Parse(value);
                        return true;
                    }
                case TypeCode.UInt16:
                    {
                        returnValue = ushort.Parse(value);
                        return true;
                    }
                case TypeCode.Int32:
                    {
                        returnValue = int.Parse(value); ;
                        return true;
                    }
                case TypeCode.UInt32:
                    {
                        returnValue = uint.Parse(value);
                        return true;
                    }
                case TypeCode.Int64:
                    {
                        returnValue = long.Parse(value);
                        return true;
                    }
                case TypeCode.UInt64:
                    {
                        returnValue = ulong.Parse(value); ;
                        return true;
                    }
                case TypeCode.Single:
                    {
                        returnValue = float.Parse(value);
                        return true;
                    }
                case TypeCode.Double:
                    {
                        returnValue = double.Parse(value);
                        return true;
                    }
                case TypeCode.Decimal:
                    {
                        returnValue = decimal.Parse(value);
                        return true;
                    }
                case TypeCode.DateTime:
                    {
                        returnValue = DateTime.Parse(value);
                        return true;
                    }
                case TypeCode.String:
                    {
                        returnValue = value;
                        return true;
                    }
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                default:
                    throw new NotSupportedException("不支持该类型");
            }
        }

        /// <summary>
        /// IsNullOrEmpty
        /// </summary>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// IsNullOrWhiteSpace
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 当不为null,且不为空。
        /// </summary>
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static string ToPercentageString(this float value, int decimalDigits = 0)
        {
            return decimalDigits switch
            {
                0 => value.ToString("P0"),
                1 => value.ToString("P1"),
                2 => value.ToString("P2"),
                _ => value.ToString($"P{decimalDigits}"),
            };
        }

        #region >> Format

        public static string Format(this string text, object arg0)
        {
            return string.Format(text, arg0);
        }

        public static string Format(this string text, object arg0, object arg1)
        {
            return string.Format(text, arg0, arg1);
        }

        public static string Format(this string text, object arg0, object arg1, object arg2)
        {
            return string.Format(text, arg0, arg1, arg2);
        }

        public static string Format(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        #endregion Format


        #region >> 转换

        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        public static byte[] ToByteArray(this string value, Encoding encoding = null)
        {
            encoding ??= DefaultEncoding;
            return encoding.GetBytes(value);
        }

        /// <summary>
        /// 字节数组转字符串
        /// </summary>
        public static string ByteToString(this byte[] buffer, Encoding encoding = null)
        {
            if (buffer == null) return string.Empty;

            encoding ??= DefaultEncoding;
            return encoding.GetString(buffer);
        }

        /// <summary>
        /// Hex字符串转字节数组
        /// </summary>
        public static byte[] HexToByteArray(this string hexString)
        {
            byte[] outputByteArray = new byte[hexString.Length / 2];
            for (int x = 0; x < outputByteArray.Length; x++)
            {
                int i = Convert.ToInt32(hexString.Substring(x * 2, 2), 16);
                outputByteArray[x] = (byte)i;
            }

            return outputByteArray;
        }

        /// <summary>
        /// 字节数组转Hex字符串
        /// </summary>
        public static string ToHexString(this byte[] buffer, int startIndex = 0)
        {
            return BitConverter.ToString(buffer, startIndex).Replace("-", string.Empty);
        }

        #endregion


        #region >> Base64加解密

        /// <summary>
        /// Base64加密
        /// </summary>
        public static string Base64Encrypt(this byte[] buffer)
        {
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        public static byte[] Base64Decrypt(this string value)
        {
            try
            {
                return Convert.FromBase64String(value);
            }
            catch
            {
                return null;
            }
        }

        #endregion Base64加解密


        #region >> MD5摘要

        /// <summary>
        /// 从字符串获取MD5值
        /// </summary>
        /// <param name="value"></param>
        /// <returns>MD5值</returns>
        public static string MD5Hash(this string value)
        {
            return Utility.MD5Hash(value);
        }

        /// <summary>
        /// 验证MD5值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool VerifyMD5Hash(this string value, string hash)
        {
            return Utility.VerifyMD5Hash(value, hash);
        }

        #endregion MD5摘要


        #region >> SHA256

        /// <summary>
        /// SHA256
        /// </summary>
        /// <param name="value"></param>
        /// <returns>16进制字符串</returns>
        public static string SHA256(this string value)
        {
            return Utility.SHA256(value).ToHexString();
        }

        #endregion SHA256


        #region >> SHA512

        /// <summary>
        /// SHA512
        /// </summary>
        /// <param name="value"></param>
        /// <returns>16进制字符串</returns>
        public static string SHA512(this string value)
        {
            return Utility.SHA512(value).ToHexString();
        }

        #endregion SHA512
    }
}