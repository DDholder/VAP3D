using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public class FunctionParser
    {
        public string Function = null;
        public List<object> Arguments = new List<object>();

        public FunctionParser()
        {
        }

        private object reflectArgument(string function, string arg, int index)
        {
            MethodInfo minfo = typeof(IFSUIPCInterface).GetMethod(function);
            if (minfo == null)
                return null;

            ParameterInfo[] info = minfo.GetParameters();
            if (index >= info.Length)
                return null;

            ParameterInfo param = info[index];

            if (param.ParameterType == typeof(int) && param.Name == "offset") // bit of a hack, but it will have to do
            {
                return Convert.ToInt32(arg, 16);
            }
            else
            {
                return Convert.ChangeType(arg, param.ParameterType);
            }
        }

        public bool parseFunction(string encodedFunction)
        {
            int nameSeperatorIndex = encodedFunction.IndexOf(':');
            if (nameSeperatorIndex < 0)
                return false;

            Function = encodedFunction.Substring(0, nameSeperatorIndex);

            // Check that the function exists
            MethodInfo minfo = typeof(IFSUIPCInterface).GetMethod(Function);
            if (minfo == null)
                return false;

            if (nameSeperatorIndex < encodedFunction.Length - 1)
            {
                int startIdx = nameSeperatorIndex + 1;
                int endIdx = encodedFunction.Length;
                string encodedArgs = encodedFunction.Substring(startIdx, endIdx - startIdx);

                String[] args = encodedArgs.Split(';');
                for (int i = 0; i < args.Length; ++i)
                {
                    string arg = args[i];
                    if (arg.Length > 0 && System.Text.RegularExpressions.Regex.IsMatch(arg, "^[A-Za-z0-9_]*$"))
                    {
                        object reflectedArg = reflectArgument(Function, arg, i);
                        if (reflectedArg != null)
                        {
                            Arguments.Add(reflectedArg);
                        }
                        else
                        {
                            return false;
                        }
                    } 
                }
            }
            
            return true;
        }

    }
}
