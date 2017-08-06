using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3DUnitTests
{
    public class MyVAProxy
    {
        public Dictionary<String, object> SessionState;
        public string Context;

        public MyVAProxy()
        {
            SessionState = new Dictionary<string, object>();
        }

        public virtual void WriteToLog(String message, String colour) { }

        public virtual void SetInt(String varName, long value) { }
        public virtual long GetInt(String varName) { return 0L; }

        public virtual void SetDecimal(String varName, decimal value) { }
        public virtual decimal GetDecimal(String varName) { return 0.0M;  }

        public virtual void SetBoolean(String varName, bool value) { }
        public virtual bool GetBoolean(String varName) { return false; }
    }
}
