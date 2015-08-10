using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System;
namespace GameMain.AutoUpdate
{
public class HttpDownloadSession
{

    public static HttpDownloadRequest CreateDownload(string url, string savePath)
    {

        HttpDownloadRequest request = new HttpDownloadRequest(url, savePath);

        Thread requestThread = new Thread(new ParameterizedThreadStart(request.DoHttpRequestSyn));

        requestThread.Start();

        return request;

    }

}
    public class HttpDownloadRequest
    {
        string url;
        string savePath;

        internal HttpDownloadRequest(string url, string savePath)
        {
            this.url = url;
            this.savePath = savePath;
        }

        const int MaxReadLen = 1024;
        public void DoHttpRequestSyn(System.Object obj)
        {
            try
            {

                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;

                request.Timeout = 5000;

                request.ReadWriteTimeout = 1000;

                HttpWebResponse httpResponse = request.GetResponse() as HttpWebResponse;


                long nContentLen = httpResponse.ContentLength;

                Stream inputStream = httpResponse.GetResponseStream();
                byte[] buffer = new byte[MaxReadLen];
                using (FileStream outputStream = new FileStream(savePath, FileMode.Create))
                {

                    int nTotalReadLen = 0;

                    int nReadLen = 0;

                    while ((nReadLen = inputStream.Read(buffer, 0, buffer.Length)) != 0)
                    {

                        nTotalReadLen += nReadLen;

                        outputStream.Write(buffer, 0, nReadLen);

                    }

                }
            }

            catch (Exception e)
            {



            }
        }
    }
}
