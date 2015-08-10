using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMain.Platform
{
    class Interface
    {
        virtual string GetServerListPath()
        {
            return "";
        }

        virtual string GetPatchDownloadPath()
        {
            return "";
        }

        virtual string GetPatchUnzipPath()
        {
            return "";
        }

    }
}
