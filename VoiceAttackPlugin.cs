using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace VAP3D
{
    public class VoiceAttackPlugin
    {
        public static string VA_DisplayName()
        {
            return "VA:P3D Plugin";
        }

        public static string VA_DisplayInfo()
        {
            return "My VoiceAttack Plugin\r\n\r\nThis is just a sample.\r\n\r\n2016 VoiceAttack";
        }

        public static Guid VA_Id()
        {
            return new Guid("{20a8e798-1848-4878-92e3-865f4ec48063}");
        }

        public static void VA_StopCommand()
        {
        }

        public static void VA_Init1(dynamic vaProxy)
        {
            FSUIPCInterface fsuipcInterface = new FSUIPCInterface();
            fsuipcInterface.initialise(vaProxy);

            vaProxy.SessionState.Add("FSUIPCInterface", fsuipcInterface);
        }

        public static void VA_Exit1(dynamic vaProxy)
        {
            FSUIPCInterface fsuipcInterface = vaProxy.SessionState["FSUIPCInterface"];
            fsuipcInterface.shutdown();
        }

        public static void VA_Invoke1(dynamic vaProxy)
        {
            FSUIPCInterface fsuipcInterface = vaProxy.SessionState["FSUIPCInterface"];

            string context = vaProxy.Context;

            ParsedFunction func = parseFunction(context);
            fsuipcInterface.call(func.Function, func.ArgList);
        }
    }
}
