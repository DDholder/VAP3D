using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public class VerticalSpeedMonitor : GenericMonitor
    {
        private bool mCalledOutPositiveRate = false;

        public override void valueChanged(object value, dynamic vaProxy)
        {
            int verticalSpeedMSFixed = (int)value;
            float verticalSpeedMS = verticalSpeedMSFixed / 256.0f;

            float verticalSpeed = verticalSpeedMS * 60 * 3.28084f;
            if (verticalSpeed > 100.0f && !mCalledOutPositiveRate)
            {
                if (vaProxy.CommandExists("_VAP3D_PositiveRate"))
                {
                    vaProxy.ExecuteCommand("_VAP3D_PositiveRate");
                }
                mCalledOutPositiveRate = true;
            }

            processGenericWatch(verticalSpeed, vaProxy);
        }

        protected override Type getDataType()
        {
            return typeof(float);
        }

        public override Type getOffsetDataType()
        {
            return typeof(int);
        }
    }
}
