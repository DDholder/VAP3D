using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public class Mjc : Lvar
    {
        private const int ACK_SUCCESS = 9991999;
        private const int ACK_ERROR = 9992999;

        private IFSUIPC m_fsuipc = null;
        private IOffsetFactory m_offsetFactory = null;
        private dynamic m_vaProxy = null;

        public Mjc(IFSUIPC fsuipc, IOffsetFactory offsetFactory, dynamic vaProxy)
        {
            m_fsuipc = fsuipc;
            m_offsetFactory = offsetFactory;
            m_vaProxy = vaProxy;
        }

        public bool read(int idcode, Type dataType, string destinationVariable, Action<string> errorFunc)
        {
            if (!dataType.IsPrimitive)
            {
                errorFunc("readMjc: Only primitive data types are supported");
                return false;
            }

            IOffset<int> lvarParamAddress = m_offsetFactory.createOffset<int>(OffsetValues.LvarParam, true);
            IOffset<string> lvarName = m_offsetFactory.createOffset<string>(OffsetValues.LvarName, 40, true);

            int lvarReadLocation = OffsetValues.User;
            int lvarWriteLocation = OffsetValues.User + 4; // Only need 4 bytes for read location (idcode)

            int ackCode = 0;

            // Set up MJC_VAR_READ_CODE and wait for success
            {
                IOffset<int> readOffset = m_offsetFactory.createOffset<int>(lvarReadLocation);
                IOffset<int> writeOffset = m_offsetFactory.createOffset<int>(lvarWriteLocation);

                // Write mjcCode to our user region, ready for consumption
                readOffset.Value = idcode;
                m_fsuipc.Process();

                // Tell FSUIPC where to read the mjcCode
                lvarParamAddress.Value = (int)SizeMask.INT32 | lvarReadLocation;
                // Write the code to MJC_VAR_READ_CODE L:var
                lvarName.Value = "::MJC_VAR_READ_CODE";
                m_fsuipc.Process();

                int ackAttempts = 0;
                do
                {
                    ackAttempts++;
                    // Tell FSUIPC where to write the mjcCode result
                    lvarParamAddress.Value = (int)SizeMask.INT32 | lvarWriteLocation;
                    // Read from MJC_VAR_READ_CODE L:var
                    lvarName.Value = ":MJC_VAR_READ_CODE";
                    m_fsuipc.Process();

                    ackCode = writeOffset.Value;
                } while (ackCode != ACK_SUCCESS && ackCode != ACK_ERROR && ackAttempts < 50);

                if (ackCode == ACK_ERROR)
                {
                    errorFunc("readMjc: Variable with id '" + idcode.ToString() + "' not found");
                    return false;
                }

                if (ackAttempts >= 50)
                {
                    errorFunc("readMjc: Failed waiting for ACK code to be set");
                    return false;
                }
            }

            // Now read MJC_VAR_READ_VALUE
            int numBytes = Utilities.numBytesFromType(dataType);
            IOffset valueWriteOffset = m_offsetFactory.createOffsetForType(lvarWriteLocation, dataType);

            int hiword = (int)geSizeMaskForType(dataType);
            if ((SizeMask)hiword == SizeMask.INVALID)
            {
                errorFunc("readMjc: FSUIPC does not support Lvar data type " + dataType.Name);
                return false;
            }

            // Now read MJC_VAR_READ_VALUE
            lvarParamAddress.Value = hiword | lvarWriteLocation;
            lvarName.Value = ":MJC_VAR_READ_VALUE";
            m_fsuipc.Process();

            Utilities.setVariableValue(dataType, Utilities.getOffsetValue(valueWriteOffset),
                destinationVariable, m_vaProxy);

            return true;
        }

        public bool write(int idcode, Type dataType, string sourceVariable, Action<string> errorFunc)
        {
            if (!dataType.IsPrimitive)
            {
                errorFunc("readMjc: Only primitive data types are supported");
                return false;
            }

            IOffset<int> lvarParamAddress = m_offsetFactory.createOffset<int>(OffsetValues.LvarParam, true);
            IOffset<string> lvarName = m_offsetFactory.createOffset<string>(OffsetValues.LvarName, 40, true);

            int lvarReadLocation = OffsetValues.User;
            int lvarWriteLocation = OffsetValues.User + 4; // Only need 4 bytes for read location (idcode)

            // Write the value
            int numBytes = Utilities.numBytesFromType(dataType);
            IOffset valueReadOffset = m_offsetFactory.createOffsetForType(lvarReadLocation, dataType);

            object val = Utilities.getVariableValue(dataType, sourceVariable, m_vaProxy);
            Utilities.setOffsetValue(valueReadOffset, val);

            int hiword = (int)geSizeMaskForType(dataType);
            if ((SizeMask)hiword == SizeMask.INVALID)
            {
                errorFunc("readMjc: FSUIPC does not support Lvar data type " + dataType.Name);
                return false;
            }

            // Now write MJC_VAR_WRITE_VALUE
            lvarParamAddress.Value = hiword | lvarReadLocation;
            lvarName.Value = "::MJC_VAR_WRITE_VALUE";
            m_fsuipc.Process();

            int ackCode = 0;

            // Set up MJC_VAR_WRITE_CODE and wait for success
            {
                IOffset<int> readOffset = m_offsetFactory.createOffset<int>(lvarReadLocation);
                IOffset<int> writeOffset = m_offsetFactory.createOffset<int>(lvarWriteLocation);

                // Write mjcCode to our user region, ready for consumption
                readOffset.Value = idcode;
                m_fsuipc.Process();

                // Tell FSUIPC where to read the mjcCode
                lvarParamAddress.Value = (int)SizeMask.INT32 | lvarReadLocation;
                // Write the code to MJC_VAR_READ_CODE L:var
                lvarName.Value = "::MJC_VAR_WRITE_CODE";
                m_fsuipc.Process();

                int ackAttempts = 0;
                do
                {
                    ackAttempts++;
                    // Tell FSUIPC where to write the mjcCode result
                    lvarParamAddress.Value = (int)SizeMask.INT32 | lvarWriteLocation;
                    // Read from MJC_VAR_READ_CODE L:var
                    lvarName.Value = ":MJC_VAR_WRITE_CODE";
                    m_fsuipc.Process();

                    ackCode = writeOffset.Value;
                } while (ackCode != ACK_SUCCESS && ackCode != ACK_ERROR && ackAttempts < 50);

                if (ackCode == ACK_ERROR)
                {
                    errorFunc("readMjc: Variable with id '" + idcode.ToString() + "' not found");
                    return false;
                }

                if (ackAttempts >= 50)
                {
                    errorFunc("readMjc: Failed waiting for ACK code to be set");
                    return false;
                }
            }
            
            return true;
        }
    }
}
