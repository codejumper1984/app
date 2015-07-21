using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util
{
    public class Singleton<T> where T:new()
    {
        protected Singleton()
        {

        }
        static T instance = new T();
        public static T singleton
        {
            get { return instance; }
        }
    }
}
