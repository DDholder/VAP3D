using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public class DefaultFSUIPCFactory : IFSUIPCFactory
    {
        public IFSUIPCInterface createFSUIPCInterface(IFSUIPC fsuipc, IOffsetFactory offsetFactory)
        {
            return new FSUIPCInterface(fsuipc, offsetFactory);
        }
    }
}
