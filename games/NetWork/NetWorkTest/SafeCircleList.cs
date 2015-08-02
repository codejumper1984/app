using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace NetWorkTest
{
    [TestClass]
    public class SafeCircleList
    {
        [TestMethod]
        public void TestMethodEnQueue()
        {
            int capacity = 2;
            NetWork.Util.SafeCircleList<int> m_safeList = new NetWork.Util.SafeCircleList<int>(capacity + 1);
            Assert.AreEqual(m_safeList.Capacity(), capacity);

            m_safeList.EnQueue(1);
            Assert.IsTrue(!m_safeList.IsEmpty());
            Assert.IsTrue(!m_safeList.IsFull());
            Assert.AreEqual(m_safeList.Capacity(), capacity - 1);

            m_safeList.EnQueue(1);
            Assert.AreEqual(m_safeList.Capacity(), capacity - 2);
            Assert.IsTrue(!m_safeList.IsEmpty());
            Assert.IsTrue(m_safeList.IsFull());
            Assert.IsFalse(m_safeList.EnQueue(1));
        }

        [TestMethod]
        public void TestMethodDeque()
        {
            int capacity = 2;
            NetWork.Util.SafeCircleList<int> m_safeList = new NetWork.Util.SafeCircleList<int>(capacity + 1);

            Assert.IsFalse(m_safeList.DeQueue());
            m_safeList.EnQueue(1);
            m_safeList.EnQueue(1);

            Assert.IsTrue(m_safeList.DeQueue());
            Assert.AreEqual(1, m_safeList.Capacity());
            Assert.IsTrue(!m_safeList.IsEmpty());
            Assert.IsTrue(!m_safeList.IsFull());

            Assert.IsTrue(m_safeList.DeQueue());
            Assert.AreEqual(2,m_safeList.Capacity());
            Assert.IsTrue(m_safeList.IsEmpty());
            Assert.IsTrue(!m_safeList.IsFull());

            Assert.IsFalse(m_safeList.DeQueue());

        }

        [TestMethod]
        public void TestMethodDequeEnqueMix()
        {
            int capacity = 2;
            NetWork.Util.SafeCircleList<int> m_safeList = new NetWork.Util.SafeCircleList<int>(capacity + 1);

            Assert.IsFalse(m_safeList.DeQueue());
            m_safeList.EnQueue(1);
            m_safeList.EnQueue(1);
            m_safeList.DeQueue();
            m_safeList.DeQueue();
            m_safeList.EnQueue(1);
            Assert.AreEqual(m_safeList.Capacity(), 1);
            Assert.AreEqual(m_safeList.Size(), 1);
            m_safeList.DeQueue();
            Assert.AreEqual(m_safeList.Capacity(), 2);
            Assert.AreEqual(m_safeList.Size(), 0);



        }
    }
}
