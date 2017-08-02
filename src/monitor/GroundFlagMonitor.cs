using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public class GroundFlagMonitor : GenericMonitor
    {
        private enum State
        {
            OnGround,
            InAir,

            Invalid
        }

        private State mState = State.Invalid;

        public override void valueChanged(object value, dynamic vaProxy)
        {
            short groundFlag = (short)value;

            if (mState == State.Invalid)
            {
                if (groundFlag == 0)
                {
                    mState = State.InAir;
                }
                else
                {
                    mState = State.OnGround;
                }
            }

            processGenericWatch(groundFlag, vaProxy);
        }

        protected override Type getDataType()
        {
            return typeof(short);
        }

        public override Type getOffsetDataType()
        {
            return typeof(short);
        }
    }
}
