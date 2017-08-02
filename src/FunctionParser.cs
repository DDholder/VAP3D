using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public class FunctionParser
    {
        public string Function = null;
        public List<string> Arguments = new List<string>();

        public FunctionParser(string encodedFunction)
        {
            parseFunction(encodedFunction);
        }

        private bool parseFunction(string encodedFunction)
        {
            int nameSeperatorIndex = encodedFunction.IndexOf(':');
            if (nameSeperatorIndex < 0)
                return false;

            Function = encodedFunction.Substring(0, nameSeperatorIndex);
            string encodedArgs = encodedFunction.Substring(nameSeperatorIndex + 1, encodedFunction.Length - 1);

            String[] args = encodedArgs.Split(';');
            foreach(string arg in args)
            {
                Arguments.Add(arg);
            }
            
            return true;
        }

    }
}
