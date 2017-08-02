﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public interface IFSUIPCInterface
    {
        void beginMonitoringEvents();
        void stopMonitoringEvents();
        void readOffset(int offsetAddress, int numBytes, string destinationVariable);
        void writeOffset(int offsetAddress, int numBytes, string sourceVariable);

        void initialise(dynamic vaProxy);
        void shutdown();
    }
}
