using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public class GroundSpeedMonitor : GenericMonitor
    {
        private bool mCalledOut80Kts = false;

        public override void valueChanged(object value, dynamic vaProxy)
        {
            int speedFixedPoint = (int)value;

            double speedMS = ((double)speedFixedPoint / 65535.0);
            double speedKts = speedMS * 1.943844;
            if (speedKts > 80.0 && !mCalledOut80Kts)
            {
                if (vaProxy.CommandExists("_VAP3D_EightyKnots"))
                {
                    vaProxy.ExecuteCommand("_VAP3D_EightyKnots");
                }
                mCalledOut80Kts = true;
            }

            processGenericWatch(speedKts, vaProxy);
        }

        protected override Type getDataType()
        {
            return typeof(double);
        }

        public override Type getOffsetDataType()
        {
            return typeof(int);
        }
    }
}
