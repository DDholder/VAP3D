using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public class Watcher
    {
        public enum WatchCondition
        {
            LessThan,
            LessThanEqualTo,
            EqualTo,
            GreaterThanEqualTo,
            GreaterThan,
            Not
        }

        public object referenceValue;
        public WatchCondition condition;
        public string label;
        public bool processed;
    }
}
