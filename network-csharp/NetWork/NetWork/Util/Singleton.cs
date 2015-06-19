using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork.Util
{
    public class Singleton<T> where T : new()
    {
        protected Singleton()
        {
        }
        private static T instance = new T();
        public static T Instance()
        {
            return instance;
        }
    }
}
