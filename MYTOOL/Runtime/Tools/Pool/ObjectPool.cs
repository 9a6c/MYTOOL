using System.Collections.Generic;

namespace MYTOOL.Pool
{
    public class ObjectPool<T> where T : new()
    {
        private readonly Stack<T> _stack = new Stack<T>();
        private readonly System.Action<T> _actionOnGet;
        private readonly System.Action<T> _actionOnRelease;

        public int CountAll { get; private set; }

        public int CountInactive { get { return _stack.Count; } }

        public int CountActive
        {
            get
            {
                return CountAll - CountInactive;
            }
        }

        public ObjectPool(System.Action<T> actionOnGet, System.Action<T> actionOnRelease)
        {
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
        }

        public T Get()
        {
            T element;
            if (_stack.Count == 0)
            {
                element = new T();
                CountAll++;
            }
            else
            {
                element = _stack.Pop();
            }

            if (_actionOnGet != null)
            {
                _actionOnGet(element);
            }

            return element;
        }

        public void Release(T element)
        {
            if (_stack.Count > 0 && ReferenceEquals(_stack.Peek(), element))
            {
                UnityEngine.Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            }
            if (_actionOnRelease != null)
            {
                _actionOnRelease(element);
            }

            _stack.Push(element);
        }
    }

    public static class ListPool<T>
    {
        private static readonly ObjectPool<List<T>> _listPool = new ObjectPool<List<T>>(null, Clear);
        private static void Clear(List<T> l) { l.Clear(); }

        public static List<T> Get()
        {
            return _listPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            _listPool.Release(toRelease);
        }
    }

    public static class Pool<T> where T : new()
    {
        private static readonly ObjectPool<T> _objectPool = new ObjectPool<T>(null, null);

        public static T Get()
        {
            return _objectPool.Get();
        }

        public static void Release(T element)
        {
            _objectPool.Release(element);
        }
    }
}