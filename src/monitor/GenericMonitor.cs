using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public abstract class GenericMonitor : IMonitor
    {
        protected List<Watcher> mWatchers = new List<Watcher>();

        protected bool doCompare(Watcher.WatchCondition condition, dynamic a, dynamic b)
        {
            switch (condition)
            {
                case Watcher.WatchCondition.EqualTo:
                    return a == b;
                case Watcher.WatchCondition.GreaterThan:
                    return a > b;
                case Watcher.WatchCondition.GreaterThanEqualTo:
                    return a >= b;
                case Watcher.WatchCondition.LessThan:
                    return a < b;
                case Watcher.WatchCondition.LessThanEqualTo:
                    return a <= b;
                case Watcher.WatchCondition.Not:
                    return a != b;
            }

            return false;
        }

        protected void processGenericWatch(object value, dynamic vaProxy)
        {
            for (int i = 0; i < mWatchers.Count; ++i)
            {
                Watcher watch = mWatchers[i];
                if (!watch.processed)
                {
                    Type t = getDataType();
                    dynamic ourValue = Convert.ChangeType(value, t);
                    dynamic refVal = Convert.ChangeType(watch.referenceValue, t);

                    if (doCompare(watch.condition, ourValue, refVal))
                    {
                        vaProxy.ExecuteFunction("_VAP3D_Watcher_" + watch.label);
                        watch.processed = true;
                    }
                }
            }
        }

        public void addGenericWatcher(object value, Watcher.WatchCondition condition, string identifier)
        {
            Watcher watch = new Watcher();
            watch.referenceValue = value;
            watch.condition = condition;
            watch.label = identifier;
            watch.processed = false;

            mWatchers.Add(watch);
        }

        public abstract void valueChanged(object value, dynamic vaProxy);

        protected abstract Type getDataType();

        public abstract Type getOffsetDataType();
    }
}
