using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork.Msg
{
    class Decoder
    {
        const int nMaxMsgBufferLen = 1024 * 1024;
        Byte[] m_MsgBuffer = new Byte[nMaxMsgBufferLen];
        Byte[] m_ProtocalBuffer = new Byte[nMaxMsgBufferLen];
        int nOffset = 0;
        int nMsgLen = 0;

        public bool Decode(byte[] inputBuffer, int nDataLen)
        {
            int nNeedByte = nMsgLen - nOffset;
            int nBufferOffset = 0;
            if( nBufferOffset + nNeedByte <= nDataLen )
            {
                // can decode one msg
                // copy
                CopyByte(inputBuffer, nBufferOffset, m_MsgBuffer, nOffset, nNeedByte);
                nBufferOffset += nNeedByte;
                nOffset += nNeedByte;

                // decode
                GenHeadMsg();
                nOffset = 0;
            }

            //Copy Left
            int nLeftInputByte = nDataLen - nNeedByte;
            if (nLeftInputByte < LeftDequeSize())
            {
                CopyByte(m_MsgBuffer, nBufferOffset, m_MsgBuffer, nOffset, nDataLen - nNeedByte);
                nOffset += nNeedByte;
                IteProcMsg();
            }
            else
            {
                // LeftMsg Toll Long
                return false;
            }
            return true;
        }

        private void IteProcMsg()
        {
            int nBeginIdx = 0;
            int nMsgLen = GetMsgLen(nBeginIdx);
            while(nMsgLen > 0 && nBeginIdx + nMsgLen < nOffset)
            {
                GenMsg(nBeginIdx, nBeginIdx + nMsgLen);
                nBeginIdx += nMsgLen;
                nMsgLen = GetMsgLen(nBeginIdx);
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

        private void GenHeadMsg()
        {
            GenMsg(0,nOffset);
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
