using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetWork.Link
{
    interface LinkFactory
    {
        Link CreateLink();
    }
    public class AyncTcpLinkFactory: NetWork.Util.Singleton<AyncTcpLinkFactory>, LinkFactory
    {
        public Link CreateLink()
        {
            return new AsyncLink();
        }
    }
}
