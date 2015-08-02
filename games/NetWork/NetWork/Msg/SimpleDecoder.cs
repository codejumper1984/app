using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetWork.Util;

namespace NetWork.Msg
{
    abstract class Decoder
    {
        public List<Msg> DecodedMsg = new List<Msg>();
        public abstract bool Decode(Byte[] data, int nDataLen);
        public abstract bool Decode(BinaryStream stream);
    }

    class SimpleDecoder: Decoder
    {
        const int nMaxMsgBufferLen = 1024 * 1024;
        Byte[] m_MsgBuffer = new Byte[nMaxMsgBufferLen];
        Byte[] m_ProtocalBuffer = new Byte[nMaxMsgBufferLen];

        int nOffset = 0;

        public override bool Decode(BinaryStream stream)
        {
            return true;
        }

        public override bool Decode(byte[] inputBuffer, int nDataLen)
        {
            int nLeftBuffer = m_MsgBuffer.Length - nOffset;
            if (nLeftBuffer < nDataLen)
            {
                EnLargeBuffer(nDataLen - nLeftBuffer);
            }
            CopyByte(inputBuffer, 0, m_MsgBuffer, nOffset, nDataLen);
            nOffset += nDataLen;
            IteProcMsg();
            return true;
        }

        public void EnLargeBuffer(int nEnlargeLen)
        {
            int nNewLen = m_MsgBuffer.Length  + nEnlargeLen;
            Byte[] newMsgBuffer = new byte[nNewLen];
            if(nOffset > 0)
            {
                CopyByte(m_MsgBuffer, 0, newMsgBuffer, 0, nOffset );
            }
        }

        private void IteProcMsg()
        {
            int nBeginIdx = 0;
            int nMsgLen = GetMsgLen(nBeginIdx);
            while(nMsgLen > 0 && nBeginIdx + nMsgLen <= nOffset)
            {
                GenMsg(nBeginIdx, nBeginIdx + nMsgLen);
                nBeginIdx += nMsgLen;
                nMsgLen = GetMsgLen(nBeginIdx);
            }

            if(nMsgLen > 0 )
            {
                // Msg Not Complete
                nOffset = nOffset - nBeginIdx;
                if (nOffset > 0)
                {
                    // From back to front, Copy Order!!
                    CopyByte(m_MsgBuffer, 0, m_MsgBuffer, nBeginIdx, nOffset - 1);
                }
            }
        }

        private int GetMsgLen(int nBeginidx)
        {
            // Little Endian
            int nVal = 0;
            if (nBeginidx + 4 < m_MsgBuffer.Length)
            {
                int nLeftSht = 0;
                for (int i = 0; i < 4; i++)
                {
                    nVal += m_MsgBuffer[nBeginidx + i] << nLeftSht;
                    nLeftSht += 8;
                }
            }
            return nVal;
        }

        private int LeftDequeSize()
        {
            return m_MsgBuffer.Length - nOffset;
        }

        private void GenMsg(int nBeginIdx, int nEndIdx)
        {
            CopyByte(m_MsgBuffer,nBeginIdx,m_ProtocalBuffer,0, nEndIdx - nBeginIdx + 1);
        }

        private int CopyByte(Byte[] src, int nSrcOffset,Byte[] dest, int nDestOffest,int nCopyLen)
        {
            for( int i = 0; i < nCopyLen;i++)
            {
                if(nSrcOffset + i < src.Length && nDestOffest + i < dest.Length)
                {
                    dest[nDestOffest + i] = src[nSrcOffset + i];
                }
                else
                {
                    return nCopyLen - i;
                }
            }
            return nCopyLen;
        }
    }
}
