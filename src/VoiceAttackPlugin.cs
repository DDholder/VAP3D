using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace VAP3D
{
    public class VoiceAttackPlugin
    {
        public class SESSIONSTATE
        {
            public static string KEY_FSUIPCFACTORY = "FSUIPCFactory";
            public static string KEY_FSUIPCINTERFACE = "FSUIPCInterface";
        }

        public static void SetFSUIPCFactory(dynamic vaProxy, IFSUIPCFactory factory)
        {
            vaProxy.SessionState.Add(SESSIONSTATE.KEY_FSUIPCFACTORY, factory);
        }

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
            IFSUIPCFactory factory = null;
            if (!vaProxy.SessionState.ContainsKey(SESSIONSTATE.KEY_FSUIPCFACTORY))
            {
                factory = new DefaultFSUIPCFactory();
                vaProxy.SessionState.Add(SESSIONSTATE.KEY_FSUIPCFACTORY, factory);
            }
            else
            {
                factory = vaProxy.SessionState[SESSIONSTATE.KEY_FSUIPCFACTORY];
            }

            IFSUIPCInterface fsuipcInterface = factory.createFSUIPCInterface(
                new FSUIPCImpl(), 
                new DefaultOffsetFactory());

            fsuipcInterface.initialise(vaProxy);

            vaProxy.SessionState.Add(SESSIONSTATE.KEY_FSUIPCINTERFACE, fsuipcInterface);
        }

        public static void VA_Exit1(dynamic vaProxy)
        {
            IFSUIPCInterface fsuipcInterface = vaProxy.SessionState[SESSIONSTATE.KEY_FSUIPCINTERFACE];
            fsuipcInterface.shutdown();
        }

        public static void VA_Invoke1(dynamic vaProxy)
        {
            IFSUIPCInterface fsuipcInterface = vaProxy.SessionState[SESSIONSTATE.KEY_FSUIPCINTERFACE];

            string context = vaProxy.Context;

            FunctionParser parser = new FunctionParser();
            if(!parser.parseFunction(context))
            {
                vaProxy.WriteToLog("VA:P3D Error: Failed to parse function: " + context, "red");
                return;
            }

            MethodInfo callMethod = fsuipcInterface.GetType().GetMethod(parser.Function);
            callMethod.Invoke(fsuipcInterface, BindingFlags.Default, null, parser.Arguments.ToArray(), null);
        }
    }
}
