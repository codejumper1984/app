using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace ResMgr
{
    public class CorroutineSession
    {
        MonoBehaviour corroutineBehaviour;
        private int nActiveCorroutine = 0;
        private int nMaxCorroutine = 0;
        List<Task> taskList = new List<Task>();

        public CorroutineSession(int nMaxCorroutine)
        {
            this.nMaxCorroutine = nMaxCorroutine;
        }

        public void Init()
        {

        }

        public void Release()
        {

        }

        public void Reset()
        {

        }

        public int StopTask(Task task)
        {
            for (int i = 0; i < taskList.Count; i++)
            {
                if(taskList[i].Id == task.Id)
                {
                    taskList.RemoveAt(i);
                    return 1;
                }
            }
            return 0;
        }

        public int AddTask(Task task)
        {
            taskList.Add(task);
            if(nActiveCorroutine < nMaxCorroutine)
            {
                nActiveCorroutine++;
                corroutineBehaviour.StartCoroutine(LoopCortinue());
                return 1;
            }
            return 0;
        }

        private IEnumerator LoopCortinue()
        {
            while(taskList.Count != 0)
            {
                Task task = taskList[ taskList.Count - 1];
                taskList.RemoveAt(taskList.Count - 1);
                yield return task.doTask();
            }
            nActiveCorroutine++;
        }
    }
}
