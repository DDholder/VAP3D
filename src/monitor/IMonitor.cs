using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public interface IMonitor
    {
        void valueChanged(object value, dynamic vaProxy);
        void addGenericWatcher(object value, Watcher.WatchCondition condition, string identifier);
        Type getOffsetDataType();
    }
}
