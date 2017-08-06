using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSUIPC;

namespace VAP3D
{
    class FSUIPCImpl : IFSUIPC
    {
        public void Open()
        {
            FSUIPCConnection.Open();
        }

        public void Process()
        {
            FSUIPCConnection.Process();
        }

        public void Close()
        {
            FSUIPCConnection.Close();
        }
    }
}
