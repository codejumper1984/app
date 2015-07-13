using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetWork.Util;
using ProtoBuf;
using System.IO;

namespace NetWork.Msg
{
    enum eMsgType
    {
        MsgType_Test = 0,
        MsgType_Login = 1,
    }

    class Testmsg: Msg
    {
        public Testmsg()
        {
            nMsgType = (int)eMsgType.MsgType_Test;
        }
    }

    class MsgPool: Singleton<MsgPool>
    {
        public MsgPool()
        {
            MsgDef.PtcC2GReq_Test msg = new MsgDef.PtcC2GReq_Test();
        }

        List<Msg> pool = new List<Msg>();
        Dictionary<int, Msg> m_TypeMsgDict = new Dictionary<int, Msg>();

        public void RegisterPool()
        {
            m_TypeMsgDict[(int)eMsgType.MsgType_Login] = new Testmsg();
        }
        public Msg DecMsg(int nType,MemoryStream stream)
        {
            Msg result = null;
            switch((eMsgType)nType)
            {
                case eMsgType.MsgType_Login:
                    {
                        Testmsg msg = GetMsg(nType) as Testmsg;
                        if(msg != null)
                            Serializer.Serialize<Testmsg>(stream,msg);
                        result = msg;
                    }
                    break;
            }
            return result;
        }
        public Msg GetMsg(int nType) 
        {
            if(m_TypeMsgDict.ContainsKey(nType))
            {
                return m_TypeMsgDict[nType]; 
            }
            return null;
        }
    }
}
