using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork
{
    public class SafeCircleList<T>
    {
        const int DEFAULT_LIST_SIZE = 8;

        public SafeCircleList(int nListSize = DEFAULT_LIST_SIZE)
        {
            buffer = new T[nListSize];
            nFrontIdx = 0;
            nEndIdx = 0;
            this.nListSize = nListSize;
        }

        public bool IsEmpty()
        {
            return nFrontIdx == nEndIdx;
        }

        public bool IsFull()
        {
            return (nEndIdx + 1) % nListSize == nFrontIdx;
        }

        public T Front()
        {
            if (!IsEmpty())
                return buffer[nFrontIdx];
            return default(T);
        }

        public T End()
        {
            if (!IsEmpty())
                return buffer[nEndIdx - 1];
            return default(T);
        }

        public bool Push(T t)
        {
            if(!IsFull())
            {
                buffer[nEndIdx] = t;
                IncEnd();
                return true;
            }
            return false;
        }

        private void IncEnd()
        {
            nEndIdx = (nEndIdx + 1) % nListSize;
        }

        private void IncFront()
        {
            nFrontIdx = (nFrontIdx + 1) % nListSize;
        }

        // cirle datas by array
        T[] buffer = null;

        // beginning index of the Data
        int nFrontIdx;

        // index after end index of the Data
        int nEndIdx;

        // data size
        int nListSize;
    }
}
