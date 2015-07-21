using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Util;
using UnityEngine;

namespace ResMgr
{
    class  ResourceData
    {
        float fLoadedTime;
        float fLatestUsedTime;
        List<UnityEngine.Object> loadedObjs;
        bool bLoaded;
    }

    enum eResClear
    {
        Auto,
        Manual,
    }

    enum eResLoadCourtinue
    {
        Normal,
        High,
    }

    enum eInstancePriority
    {
        Immediate,
        Normal,
        Delayed,
    }

    delegate int InstanceFunc(List<UnityEngine.Object> objList);
    public class Resource
    {
        public List<UnityEngine.Object> objList = new List<UnityEngine.Object>();
        public float LastVisitedTime = 0;
        private int nRefCount = 0;

        public int Id
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        public Resource(string path,int id)
        {
            Path = path;
            Id =id;
        }

        public void AddObj(UnityEngine.Object obj)
        {
            objList.Add(obj);
        }

        public void AddRef()
        {
            nRefCount++;
        }

        public void RemoveRef()
        {
            nRefCount--;
            if(nRefCount == 0)
            {
                Release();
            }
        }

        private void Release()
        {

        }

    }

    public class ResTask : Task
    {
        public string resPath;
        public eResLoadCourtinue corroutineFlag;
        public float VisitTime = 0;
        public ResTask(String resPath, int nId, eResLoadCourtinue flag ,eTaskStatus eStatus)
        {
            this.resPath = resPath;
            this.Id = nId;
            this.corroutineFlag = flag;
            Status = eStatus;
        }

        public WWW www = null;
        IEnumerator doTask()
        {
            www = new WWW(resPath);
            yield return www;
            ResourceManager.singleton.OnResLoad(this);
            www = null;
        }
        public InstanceFunc func = null;
        public eTaskStatus Status
        {
            get;
            set;
        }

    }

    class InstanceTask
    {
        public ResLoadTask task = null;
        public InstanceFunc func = null;
        public ResLoadTask resTask = null;
    }

    enum eTaskStatus
    {
        Load,
        Ins,
    }

    class ResTaskInfo
    {
        eTaskStatus taskStatus;
        int nId;
    }

    public class ResourceManager:Singleton<ResourceManager>
    {
        int nResId = 0;
        public int OnResLoad(ResTask task)
        {
            if(!resouceCacheDict.ContainsKey(task.resPath))
            {
                nResId++;
                Resource res = new Resource(task.resPath, nResId);
                res.AddObj(task.www.assetBundle.mainAsset);
                res.AddRef();
                task.www.assetBundle.Unload(false);
                return nResId;
            }
            return 0;
        }

        MonoBehaviour cortinueBehaviour;
        const int nMaxNormalCortinueNum = 20;
        CorroutineSession normalCorSession = new CorroutineSession(nMaxNormalCortinueNum);
        CorroutineSession highCorSession = new CorroutineSession(0);

        Dictionary<string, Resource> resouceCacheDict = new Dictionary<string, Resource>();

        Dictionary<int, ResTask> resLoadTaskIdDict = new Dictionary<int, ResTask>();
        //List<InstanceTask> insTaskList = new List<InstanceTask>();
        //Dictionary<int, ResTaskInfo> resInfoIdDict = new Dictionary<int, ResTaskInfo>();
        //Dictionary<string, int> resInfoIdPath = new Dictionary<string, int>();


        int nTaskId = 0;

        public int LoadResource(string path,eResLoadCourtinue corroutineFlag, eInstancePriority insFlag, InstanceFunc insFunc)
        {
            if(resouceCacheDict.ContainsKey(path))
            {
                Resource res = resouceCacheDict[path];
                res.AddRef();
                if (insFlag == eInstancePriority.Immediate)
                {
                    insFunc(res.objList);
                    res.RemoveRef();
                    return -1;
                }
                else
                {
                    InstanceTask insTask = new InstanceTask();
                    insTask.func = insFunc;
                    insTask.task = task;
                    insTaskList.Add(insTask);
                    return 0;
                }
            }
            else
            {
                nTaskId++;
                ResTask task = new ResTask(path,nTaskId,corroutineFlag,eTaskStatus.Load);
                task.VisitTime= Time.time;
                normalCorSession.AddTask(task);
            }
            return nTaskId;
        }

        public void RemoveLoad(int nId)
        {
            if(resLoadTaskIdDict.ContainsKey(nId))
            {
                ResLoadTask restask = resLoadTaskIdDict[nId];
                if(restask.corroutineFlag== eResLoadCourtinue.Normal)
                {
                    normalCorSession.StopTask(restask);
                }
            }
        }

        private int nFrameMaxInsCot = 0;
        public void Update()
        {
            for(int nFrameDelayInsCot = 0;nFrameDelayInsCot < nFrameMaxInsCot && nFrameDelayInsCot < insTaskList.Count;nFrameDelayInsCot++)
            {
                InstanceTask instask = insTaskList[nFrameDelayInsCot];
                instask.func(instask.resTask.objList);
                instask.resTask.RemoveRef();
            }
        }
    }
}
