using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USBLDC.Structure
{
    public interface StructureInterface
    {
        bool Parse(byte[] bytes);
        byte[] SavePakage();

    }

    enum TypeId
    {
        SONARCONFIG=0x0001,
        ADINFO = 0x0004,
        RawPos = 0x0007,        
        Pose = 0x0009,
        GPS = 0x0011,
        AjustPos = 0x0099,
    }
}
