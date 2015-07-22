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
        public float fLoadedTime;
        public float fLatestUsedTime;
        public List<UnityEngine.Object> loadedObjs;
        public bool bLoaded;
        public int loadTaskID;
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
        public int LoadTaskID = 0;
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

    public class ResLoadTask : Task
    {
        private int resouceId;
        public eResLoadCourtinue corroutineFlag;
        public ResLoadTask(int nId, int nResId, eResLoadCourtinue flag)
        {
            this.Id = nId;
            this.corroutineFlag = flag;
            this.resouceId = nResId;
        }

        public WWW www = null;
        IEnumerator doTask()
        {
            Resource res = ResourceManager.singleton.GetResource(resouceId);
            if(res != null)
            {
                www = new WWW(res.Path);
                yield return www;
                UnityEngine.Object obj = www.assetBundle.mainAsset;
                if (obj != null)
                {
                    res.AddObj(www.assetBundle.mainAsset);
                }
                else
                {
                    ResourceManager.singleton.OnResLoadFailed(resouceId);
                }

                www = null;
            }
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

        Dictionary<string, int> resouceCacheDict = new Dictionary<string, Resource>();

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
