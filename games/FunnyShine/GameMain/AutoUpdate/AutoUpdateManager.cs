using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMain.AutoUpdate
{
    public enum AutoUpdateStatus
    {
        None = 0,
        Preparing = 1,
        DownloadingUpdate,
        DownloadingPatch,
        UnzipPatch,
        DownloadServerlist
        Finished,
    }

    public class AutoUpdateManager:Util.Singleton<AutoUpdateManager>
    {
        AutoUpdateStatus updateStatus = AutoUpdateStatus.None;
        HttpDownloadRequest request = null;

        void DownloadPatch(String patchUrl, String patchSavePath)
        {
            updateStatus = AutoUpdateStatus.DownloadingPatch;
            Progress = 0;
            request = HttpDownloadSession.CreateDownload("", "");
        }

        void DownloadServerlist(String serlistUrl, String savePath)
        {
            updateStatus = AutoUpdateStatus.DownloadingPatch;
            Progress = 0;
            request = HttpDownloadSession.CreateDownload("", "");
        }

        void UnZipPatch()
        {

        }

        int Progress{
            get;
            set;
        }

        public void Update()
        {
            switch(updateStatus)
            {
                case AutoUpdateStatus.DownloadingPatch:
                    {
                        //UpdateProgress;
                    }

            }

        }
    }
}
