using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VAP3D
{
    public class AltimeterMonitor : GenericMonitor
    {
        private enum State
        {
            Below10000ft = 0x1,
            Above10000ft = 0x2,

            Invalid = 0x0
        }

        private const int TenThousandFeet = 10000;
        public const long CallThresholdMs = 20000; // 20s timeout between "10,000ft" calls.

        private bool mIsFeet = true;
        private State mState = State.Invalid;
        private Stopwatch mStopwatch = new Stopwatch();

        private bool isOutsideCallThreshold()
        {
            if (!mStopwatch.IsRunning)
                return true;

            if (mStopwatch.ElapsedMilliseconds > CallThresholdMs)
                return true;

            return false;
        }

        public AltimeterMonitor(short altimeterSetting)
        {
            if (altimeterSetting == 2)
            {
                mIsFeet = false;
            }
        }

        public override void valueChanged(object value, dynamic vaProxy)
        {
            int altimeterReading = (int)value;
            int altimeterInFeet = mIsFeet ? altimeterReading : (int)(altimeterReading * 3.28084f);

            if (mState == State.Invalid)
            {
                if (altimeterInFeet > TenThousandFeet)
                {
                    mState = State.Above10000ft;
                }
                else
                {
                    mState = State.Below10000ft;
                }
            }
            else if (mState == State.Below10000ft && altimeterInFeet > TenThousandFeet)
            {
                if (vaProxy.CommandExists("_VAP3D_TenThousandFeet") && isOutsideCallThreshold())
                {
                    vaProxy.ExecuteCommand("_VAP3D_TenThousandFeet");
                    mStopwatch.Restart();
                }

                mState = State.Above10000ft;
            }
            else if (mState == State.Above10000ft && altimeterInFeet <= TenThousandFeet)
            {
                if (vaProxy.CommandExists("_VAP3D_TenThousandFeet") && isOutsideCallThreshold())
                {
                    vaProxy.ExecuteCommand("_VAP3D_TenThousandFeet");
                    mStopwatch.Restart();
                }

                mState = State.Below10000ft;
            }

            processGenericWatch(altimeterInFeet, vaProxy);
        }

        protected override Type getDataType()
        {
            return typeof(int);
        }

        public override Type getOffsetDataType()
        {
            return typeof(int);
        }
    }
}
