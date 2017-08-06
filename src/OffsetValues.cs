using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public class OffsetValues
    {
        public const int GroundFlag = 0x0366;
        public const int GroundSpeed = 0x02B4;
        public const int VerticalSpeed_InAir = 0x02C8;
        public const int Altimeter = 0x3324;
        public const int AltimeterSetting = 0x0C18;

        public const int LvarParam = 0x0D6C;
        public const int LvarName = 0x0D70;

        public const int User = 0x66C0;
    }
}
