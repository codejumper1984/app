using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetWork.Util;

namespace NetWork.Msg
{
    class DecodeException:Exception
    {

    }
    class BinaryDecoder: Decoder
    {
        BinaryStream stream = new BinaryStream();
        public override bool Decode(BinaryStream stream)
        {
            stream.HeadPosition(0);
            while(stream.Size() > 0)
            {
                stream.MarkHead();
                UInt32 nMsgType = 0;
                UInt32 nMsgLen = 0;
                try
                {
                    nMsgType = stream.Decode_uint32();
                    nMsgLen = stream.Decode_uint32();
                }
                catch (StreamOverflowException e)
                {
                    //缺失头部的不完整包
                    stream.RollBackHeadAndAlign();
                    break;
                }
                if (stream.Size() > nMsgLen)
                {
                    stream.HeadPosition(stream.Head + (int)nMsgLen);
                }
                else
                {
                    //不是完整地包裹
                    int nMarkedHead = stream.MarkedHead;
                    int nOldHead = stream.Head;
                    stream.RollBackHeadAndAlign();
                    int nNeedLen = (int)nMsgLen + nOldHead - nMarkedHead + 1;
                    stream.TryResize(nNeedLen);//如果当前buffer装不下消息，扩大
                    break;
                }
            }

            return true;
        }
        private void DecodeMsg(int nMsgType)
        {

        }
        public override bool Decode(byte[] data, int len)
        {
            return true;
        }
    }
}
