using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork.Util
{
    class StreamOverflowException:Exception
    {

    }
    class BinaryStream
    {
        const int DEFAULT_DATA_LEN = 1024 * 1024;

        byte[] m_Data = null;
        public byte[] Data
        {
            get
            {
                return m_Data;
            }
        }


        private int nMarkPos = 0;
        public int MarkedHead
        {
            get{return nMarkPos;}
        }

        private int nBeginIdx;
        public int Head
        {
            get { return nBeginIdx; }
        }

        private int nEndIdx;
        public int End
        {
            get { return nEndIdx; }
        }


        public void MarkHead()
        {
            nMarkPos = nBeginIdx;
        }

        public void RoolbackHead()
        {
            nBeginIdx = nMarkPos;
        }

        public void RollBackHeadAndAlign()
        {
            HeadPosition(0);
            Clear(0,MarkedHead - 1);
        }

        public BinaryStream(int nDatalen = DEFAULT_DATA_LEN)
        {
            m_Data = new byte[nDatalen];
            nBeginIdx = 0;
            nEndIdx = 0;
        }

        public void Begin()
        {

        }

        public void HeadPosition(int nPos)
        {
            nBeginIdx = nPos;
        }

        public int Capacity()
        {
            return m_Data.Length - nEndIdx;
        }

        public int Size()
        {
            return nEndIdx - nBeginIdx;
        }

        public void Clear()
        {
            nBeginIdx = nEndIdx = 0;
        }
        public void TryResize(int nLen)
        {
            if(nLen > m_Data.Length)
            {
                byte[] newData = new byte[nLen];
                Array.Copy(m_Data, 0, newData, 0, nEndIdx);
                m_Data = newData;
            }
        }

        public bool Clear(int nBIdx, int nEIdx)
        {
            if(nEIdx > nBIdx)
            {
                if (nBIdx < Size() )
                {
                    int nCopyLen = nEndIdx - nEIdx - 1;
                    if(nEIdx < Size())
                    {
                        int nToIdx = nBeginIdx + nBIdx;
                        int nFromIdx = nBeginIdx + nEIdx + 1;
                        if (nToIdx < m_Data.Length && nFromIdx < m_Data.Length)
                        {
                            Array.Copy(m_Data, nFromIdx, m_Data, nToIdx, nCopyLen);
                        }
                    }
                    else
                    {
                        nCopyLen = nEndIdx - (nBeginIdx + nBIdx);
                    }
                    nEndIdx -= nCopyLen;
                    return true;
                }
            }
            return false;
        }

        public void Decode_int32()
        {
            uint val = 0;
            int shift = 24;
            for(int i = 0; i < 4;i++)
            {
                uint onebyte = Decode_byte();
                val += (onebyte << shift);
                shift -= 8;
            }
        }

        public UInt32 Decode_uint32()
        {
            uint val = 0;
            int shift = 24;
            for(int i = 0; i < 4;i++)
            {
                uint onebyte = Decode_byte();
                val += (onebyte << shift);
                shift -= 8;
            }
            return val;
        }

        public byte Decode_byte()
        {
            if(nBeginIdx >= nEndIdx)
            {
                throw new StreamOverflowException();
            }
            else
            {
                return m_Data[nBeginIdx++];
            }
        }
    }
}
