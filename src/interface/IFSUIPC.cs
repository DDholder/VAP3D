using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public interface IFSUIPC
    {
        void Open();
        void Process();
        void Close();
    }
}
