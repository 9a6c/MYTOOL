namespace MYTOOL
{
    /// <summary>
    /// 单例模板
    /// </summary>
    public class SingletonTemplate<T> where T : class, new()
    {
        protected static T instance;

        protected SingletonTemplate() { }

        private static readonly object locker = new object();

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        instance ??= new T();
                    }
                }

                return instance;
            }
        }


        public virtual void Init(params object[] args) { }

        public virtual void DeInit() { }
    }
}