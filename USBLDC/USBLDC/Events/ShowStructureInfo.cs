using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USBLDC.Structure;

namespace USBLDC.Events
{
    public class ShowStructureInfo
    {
        public StructureInterface Info { get; private set; }
        public int Id { get; set; }
        public ShowStructureInfo(StructureInterface info, int id)
        {
            Id = id;
            Info = info;
        }
    }
}
