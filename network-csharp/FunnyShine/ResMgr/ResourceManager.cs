using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Util;
using UnityEngine;

namespace ResMgr
{

    //加载状态
    enum eResLoadStatus
    {
        Loading = 1,
        Loaded,
    }

    //资源清理策略
    enum eResClearStrategy
    {
        Auto,
        Manual,
    }
    
    //资源加载回调策略
    enum eResUseCallbackStrategy
    {
        Immediate = 1,
        Delay = 2
    }

    //使用协程类型
    enum eLoadCourtinueType
    {
        Normal,
        High,
    }

    //回调函数类型
    delegate int ResUseCallBack(List<UnityEngine.Object> objList);

    //如何本文件访问，其他无法访问

    //引用基类
    public class RefBase
    {
        public RefBase(int refID, ResUseCallBack callBack)
        {
            this.refID = refID;
            this.callBack = callBack;
        }
        private ResUseCallBack callBack;
        public ResUseCallBack CallBack
        {
            get { return callBack; }
        }

        private int refID;
        public int RefID
        {
            get { return RefID; }
        }
    }

    //资源的加载引用
    public class LoadRef:RefBase
    {
        public LoadRef(int refID, ResUseCallBack callBack,eResUseCallbackStrategy insStrategy):base(refID,callBack)
        {
            this.insStrategy = insStrategy;
        }
        eResUseCallbackStrategy insStrategy;
    }

    //资源的实例化引用
    public class InsRef:RefBase
    {
        public InsRef(int refID, ResUseCallBack callBack,Resource res):base(refID,callBack)
        {
            this.res = res;
        }
        private Resource res;
        public Resource resource
        {
            get { return res; }
        }
    }

    public class Resource
    {
        static int nNextResId = 0;
        public static Resource NewResource(string path)
        {
            Resource res =  new Resource(path, nNextResId << 8);
            return res;
        }

        int nNextRefId = 0;
        public void AddLoadRef(ResUseCallBack callBack,eResUseCallbackStrategy insStrategy)
        {
            nNextRefId++;
            loadList.Add(new LoadRef(nNextRefId, callBack, insStrategy));
        }

        public int LoadTaskID
        {
            get;
            set;
        }

        private List<UnityEngine.Object> objList = new List<UnityEngine.Object>();

        List<LoadRef> loadList = new List<LoadRef>();
        List<InsRef> insList = new List<InsRef>();

        private float LastVisitedTime = 0;
        public void UpdateVisitTime()
        {
            LastVisitedTime = Time.time;
        }

        public int ID
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        private Resource(string path,int id)
        {
            Path = path;
            ID =id;
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

    public class ResourceManager:Singleton<ResourceManager>
    {
        protected int NewResLoadTask(Resource res,eLoadCourtinueType corroutineType)
        {
            ResLoadTask task = new ResLoadTask(res);
            return 0;
        }

        protected Resource NewResourceAndLoad(String path,ResUseCallBack callBack,eResUseCallbackStrategy insStrategy, eLoadCourtinueType corroutineType)
        {
            Resource res = Resource.NewResource(path);
            res.LoadTaskID = NewResLoadTask(res,corroutineType);
            res.AddLoadRef(callBack, insStrategy);
            return res;
        }

        int nResId = 0;

        public int LoadResource(string path,eLoadCourtinueType corroutineFlag, eResUseCallbackStrategy insFlag, ResUseCallBack callBack)
        {
            return 0;
        }

        public void RemoveLoad(int nId)
        {
        }

        public void Update()
        {
        }
    }
}
