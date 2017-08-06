using System;
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

        void beginOffsetBatch();
        void endOffsetBatch();

        void readOffset(int offset, Type dataType, string destinationVariable);
        void writeOffset(int offset, Type dataType, string sourceVariable);
        void readStringOffset(int offset, int length, string destinationVariable);
        void writeStringOffset(int offset, int length, string sourceVariable);

        void readLvar(string lvarName, Type lvarType, string destinationVariable);
        void writeLvar(string lvarName, Type lvarType, string sourceVariable);

        void readMjc(int idcode, Type dataType, string destinationVariable);
        void writeMjc(int idcode, Type dataType, string sourceVariable);

        void initialise(dynamic vaProxy);
        void shutdown();

        bool isConnected();
    }
}
