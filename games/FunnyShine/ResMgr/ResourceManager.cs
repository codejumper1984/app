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
        Delay = 2,
        Self = 3
    }

    //使用协程类型
    enum eLoadType
    {
        CorroutineNormal = 1,
        CorroutineHigh,

    }

    //回调函数类型
    delegate int ResUseCallBack(List<UnityEngine.Object> objList);

    //如何本文件访问，其他无法访问

    //引用基类
    public class ResRefBase
    {
        public ResRefBase(int refID, ResUseCallBack callBack)
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
    public class LoadRef:ResRefBase
    {
        public LoadRef(int refID, ResUseCallBack callBack,eResUseCallbackStrategy insStrategy):base(refID,callBack)
        {
            this.insStrategy = insStrategy;
        }
        public eResUseCallbackStrategy insStrategy;
    }

    //资源的实例化引用
    public class InsRef:ResRefBase
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
        public void DoIns()
        {
            CallBack(res.objList);
            res.RemoveRef(RefID);
        }
    }

    internal class FreeUseRef:ResRefBase
    {
        internal FreeUseRef(int nID)
            : base(nID, null)
        {

        }
    }

    public class Resource
    {
        static const int nResIDShitf = 8;
        static int nNextResId = 0;
        public static Resource NewResource(string path, eResClearStrategy eClearFlag)
        {
            Resource res = new Resource(path,eClearFlag, nResIDShitf << nResIDShitf);
            return res;
        }
        public eResClearStrategy eClearFlag;
        // 引用ID
        int nNextRefId = 0;
        //添加加载引用
        public LoadRef AddLoadRef(ResUseCallBack callBack, eResUseCallbackStrategy insStrategy)
        {
            nNextRefId++;
            LoadRef loadRef = new LoadRef(nNextRefId, callBack, insStrategy);
            loadDict[loadRef.RefID] = loadRef;
            return loadRef;
        }
        public InsRef ChgInsRef(int nRefId,ResUseCallBack callBack)
        {
            InsRef insRef = new InsRef(nRefId, callBack, this);
            insDict[insRef.RefID] = insRef;
            return insRef;
        }
        public InsRef AddInsRef(ResUseCallBack callBack)
        {
            nNextRefId++;
            InsRef insRef = new InsRef(nNextRefId, callBack, this);
            insDict[insRef.RefID] = insRef;
            return insRef;
        }
        public FreeUseRef ChgFreeUseRef(int nRef)
        {
            FreeUseRef freeUseRef = new FreeUseRef(nNextRefId);
            freeUseDict[freeUseRef.RefID] = freeUseRef;
            return freeUseRef;
        }
        public FreeUseRef AddFreeUseRef()
        {
            nNextRefId++;
            FreeUseRef freeUseRef = new FreeUseRef(nNextRefId);
            freeUseDict[freeUseRef.RefID] = freeUseRef;
            return freeUseRef;
        }

        public void AddObject(UnityEngine.Object obj)
        {
            objList.Add(obj);
        }

        public ResLoadTask LoadTask
        {
            get;
            set;
        }

        public int RefCount
        {
            get { return loadDict.Count + insDict.Count + freeUseDict.Count; }
        }

        public List<UnityEngine.Object> objList = new List<UnityEngine.Object>();

        internal Dictionary<int, LoadRef> loadDict = new Dictionary<int, LoadRef>();
        internal Dictionary<int, InsRef> insDict = new Dictionary<int, InsRef>();
        internal Dictionary<int, FreeUseRef> freeUseDict = new Dictionary<int, FreeUseRef>();

        public float LastVisitedTime
        {
            get { return lastVisitedTime; }
        }
        private float lastVisitedTime = 0;
        public void UpdateVisitTime()
        {
            lastVisitedTime = Time.time;
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
        public void Release()
        {
            foreach(UnityEngine.Object obj in objList)
            {
                GameObject.Destroy(obj);
            }
            objList.Clear();
        }

        private Resource(string path, eResClearStrategy eClearFlag, int id)
        {
            Path = path;
            ID =id;
            this.eClearFlag = eClearFlag;
        }
        internal void OnLoadFinished()
        {
         
            foreach (KeyValuePair<int, LoadRef> loadRefInfo in loadDict)
            {
                switch (loadRefInfo.Value.insStrategy)
                {
                    case eResUseCallbackStrategy.Immediate:
                        {
                            loadRefInfo.Value.CallBack(objList);
                        }
                        break;
                    case eResUseCallbackStrategy.Delay:
                        {
                            int nRefId = loadRefInfo.Value.RefID;
                            ChgInsRef(nRefId, loadRefInfo.Value.CallBack);
                        }
                        break;
                    case eResUseCallbackStrategy.Self:
                        {
                            int nRefId = loadRefInfo.Value.RefID;
                            ChgFreeUseRef(nRefId);
                        }
                        break;
                }
            }
          
            ResourceManager.singleton.OnResLoadFinished(this);
        }

        static int GetRefId(int nResID)
        {
            return nResID & ~(nResIDShitf - 1);
        }

        internal int RemoveRef(int nId)
        {
            int nRefId = GetRefId(nId);
            if(loadDict.ContainsKey(nRefId))
            {
                loadDict.Remove(nRefId);
                if(loadDict.Count == 0 )
                {
                    ResourceManager.singleton.TryCancelLoad(LoadTask);
                }
                return 0;
            }
            if (insDict.ContainsKey(nRefId))
            {
                insDict.Remove(nRefId);
                return 0;
            }
            if (freeUseDict.ContainsKey(nRefId))
            {
                freeUseDict.Remove(nRefId);
                return 0;
            }
            return -1;
        }
    }

    internal class ResLoadTask : Task
    {
        static int nNextTaskID = 0;
        internal static ResLoadTask NewTask( Resource res, eLoadType flag)
        {
            nNextTaskID++;
            return new ResLoadTask(nNextTaskID,res, flag);
        }
        private Resource res;
        public eLoadType LoadType;
        private ResLoadTask(int nId, Resource res, eLoadType flag)
        {
            this.Id = nId;
            this.LoadType = flag;
            this.res = res;
        }

        public WWW www = null;
        IEnumerator doTask()
        {
           
            if(res != null)
            {
                www = new WWW(res.Path);
                yield return www;
                UnityEngine.Object obj = www.assetBundle.mainAsset;
                if (obj != null)
                {
                    res.AddObject(www.assetBundle.mainAsset);
                    res.OnLoadFinished();
                }
                www = null;
            }
        }

        internal static void CancelLoad(int nLoadTask)
        {
            throw new NotImplementedException();
        }
    }

    public class ResourceManager:Singleton<ResourceManager>
    {
        CorroutineSession normalSession = null;
        Dictionary<string, Resource> pathResDict = new Dictionary<string, Resource>();
        Dictionary<int, Resource> idResDict = new Dictionary<int, Resource>();
        List<InsRef> delayedInsList = new List<InsRef>();

        public void TryCancelLoad(ResLoadTask loadTask)
        {
            if (loadTask.LoadType == ResMgr.eLoadType.CorroutineNormal)
                normalSession.StopTask(loadTask.Id);
        }
        protected int StartLoad(Resource res,ResUseCallBack callBack,eResUseCallbackStrategy insStrategy, eLoadType corroutineType)
        {
            if (normalSession == null)
                return -1;
          
            ResLoadTask loadTask = ResLoadTask.NewTask(res, corroutineType);
            res.LoadTask = loadTask;
            int loadrefId = res.AddLoadRef(callBack, insStrategy).RefID;
            switch (corroutineType)
            {
                case eLoadType.CorroutineNormal:
                    {
                        normalSession.AddTask(loadTask);
                    }
                    break;
            }
            return res.ID + loadrefId;
        }

        public int Init(MonoBehaviour corroutineBehav)
        {
            normalSession = new CorroutineSession(100, corroutineBehav);
            return 0;
        }

        /**
         *延迟实例化
         *创建延迟实例化应用，加入全局延迟实例化队列中
         */
        int DelayIns(Resource res, ResUseCallBack callBack)
        {
            InsRef insRef = res.AddInsRef(callBack);
            delayedInsList.Add(insRef);
            int resID = res.ID + insRef.RefID;
            idResDict[resID] = res;
            return resID;
        }

        int ImmediateIns(Resource res, ResUseCallBack callBack)
        {
            callBack(res.objList);
            return 0;
        }

        int FreeUseRef(Resource res)
        {
            FreeUseRef freeRef = res.AddFreeUseRef();
            int resID = res.ID + freeRef.RefID;
            idResDict[resID] = res;
            return resID;
        }

        public int LoadResource(string path, ResUseCallBack callBack, eLoadType corroutineFlag, eResUseCallbackStrategy insFlag, eResClearStrategy eResClearFlag)
        {
            int resID = 0;
            Resource resouce;
            if (pathResDict.TryGetValue(path, out resouce))
            {
                switch(insFlag)
                {
                    case eResUseCallbackStrategy.Delay:
                        {
                            resID = DelayIns(resouce, callBack);    
                        }
                        break;
                    case eResUseCallbackStrategy.Immediate:
                        {
                            resID = ImmediateIns(resouce, callBack);    
                        }
                        break;
                    case eResUseCallbackStrategy.Self:
                        {
                            resID = FreeUseRef(resouce);
                        }
                        break;

                }
            }
            else
            {
                Resource res = Resource.NewResource(path, eResClearFlag);
                resID = StartLoad(res,callBack, insFlag,corroutineFlag);

                if(resID != 0)
                {
                    pathResDict[res.Path] = res;
                    idResDict[resID] = res;
                }
                
            }
            return resID;
        }

        public int RemoveResouceRef(int nId)
        {
            Resource res;
            if (!idResDict.TryGetValue(nId, out res))
                return -1;
             res.RemoveRef(nId);
             idResDict.Remove(nId);
             return 0;
        }

        public Resource GetResource(int nId)
        {
            Resource res;
            if(!idResDict.TryGetValue(nId,out res))
            {
                res = null;
            }
            res = null;
            return res;
        }

        int nFrameInsMaxCount = 10;
        float resDelMinTime = 60 * 10;
        List<string> releaseResList = new List<string>();
        public void Update()
        {
            int nInsCount = 0;
            for(int i = 0; i < delayedInsList.Count;i++)
            {
                if(nInsCount< nFrameInsMaxCount)
                { 
                    InsRef insRef = delayedInsList[i];
                    insRef.DoIns();
                    nInsCount++;
                }
                else
                {
                    break;
                }
            }
            releaseResList.Clear();
            foreach (KeyValuePair<string, Resource> resInfo in pathResDict)
            {
                Resource res = resInfo.Value;
                if(res.RefCount == 0)
                {
                    if(Time.time - res.LastVisitedTime > resDelMinTime)
                    {
                        releaseResList.Add(res.Path);
                        res.Release();
                    }
                }
            }
            for (int i = 0; i < releaseResList.Count; i++)
            {
                pathResDict.Remove(releaseResList[i]);
            }
        }

        internal void OnResLoadFinished(Resource resource)
        {
            foreach(KeyValuePair<int,InsRef> insRefInfo in resource.insDict)
            {
                delayedInsList.Add(insRefInfo.Value);
            }
        }
    }
}
