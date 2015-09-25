using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USBLDC.Structure;

namespace USBLDC.Events
{
    public class ShowGPSInfo
    {
        public StructureInterface Info { get; private set; }

        public ShowGPSInfo(StructureInterface info)
        {
            Info = info;
        }
    }
}
