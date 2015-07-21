using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ResMgr
{
    public abstract class Task
    {
        public abstract IEnumerator doTask();
        public bool IsRunning
        {
            get;
            set;
        }
        public int Id
        {
            get;
            set;
        }
    }
}
