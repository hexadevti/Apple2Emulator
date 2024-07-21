using Apple2Sharp.Mainboard;

namespace Apple2Sharp.CPU65C02
{
    internal static class MathOpCodeProcessors
    {

        public static void Process_ADC(State processorState, Apple2Board mainBoard, ushort address)
        {
            ProcessAdc(processorState, ReadByte(address, mainBoard));
        }

        public static void Process_SBC(State processorState, Apple2Board mainBoard, ushort address)
        {
            if (processorState.D)
                ProcessSbc(processorState, ReadByte(address, mainBoard));
            else
                ProcessAdc(processorState, ReadAndInvertByte(address, mainBoard));
        }

        private static void ProcessAdc(State processorState, byte value)
        {
            SetAccAndFlags(processorState, value, GetAddToAccWithCarryResult(processorState, value));
        }

        private static void ProcessSbc(State processorState, byte value)
        {
            GetSbtToAccWithCarryResult(processorState, value);
        }

        private static void SetAccAndFlags(State processorState, byte value, int result)
        {
            processorState.extraCycles++;
            processorState.V = !(((processorState.A ^ value) & 0x80) > 0);
            if (processorState.D)
            {
                if (result >= 0xA0) 
                {
                    processorState.C = true;
                    if (result >= 0x180)
                        processorState.V = false;
                    result += 0x60;
                }
                else 
                {
                    processorState.C = false;
                    if (result < 0x80)
                        processorState.V = false;
                }					
            }
            else
            {
                if (result >= 0x100)
                {
                    processorState.C = true;
                    if (result >= 0x180) 
                        processorState.V = false;
                }
                else
                {
                    processorState.C = false;
                    if (result < 0x80) processorState.V = false;
                }
            }
            processorState.A = (byte)(result & 0xFF);
            processorState.Z = (byte)result == 0;
            processorState.N = ((byte)processorState.A).IsNegative();
        }

        private static int GetAddToAccWithCarryResult(State processorState, byte value)
        {
            if (processorState.D)
            {
                var ret = 0;
                ret = (processorState.A & 0x0F) + (value & 0x0F) + RegisterFunctions.ReadCarryFlag(processorState);
                if (ret >= 0x0a)
                    ret = 0x10 | ((ret + 6) & 0x0f);
                ret += (processorState.A & 0xf0) + (value & 0xf0);
                return ret;
            }
            else
                return value + processorState.A + RegisterFunctions.ReadCarryFlag(processorState);
        }

        private static void GetSbtToAccWithCarryResult(State processorState, byte value)
        {
            var ret = 0;
            processorState.extraCycles++;
            processorState.V = !(((processorState.A ^ value) & 0x80) > 0);
            if (processorState.D)
            {
                byte value2 = (byte)(0x0F + (processorState.A & 0x0F) - (value & 0x0F) + RegisterFunctions.ReadCarryFlag(processorState));
                if (value2 < 0x10)
                {
                    ret = 0;
                    value2 -= 0x06;
                }
                else
                {
                    ret = 0x10;
                    value2 -= 0x10;
                }
                ret += 0xF0 + (processorState.A & 0xF0) - (value & 0xF0);

                if (ret < 0x100)
                {
                    processorState.C = false;
                    if (ret < 0x80)
                        processorState.V = false;
                    ret -= 0x60;
                }
                else
                {
                    processorState.C = true;
                    if (ret >= 0x180)
                        processorState.V = false;
                }

                ret += value2;

            }
            else
            {
                ret = (byte)(0xff + processorState.A - value + RegisterFunctions.ReadCarryFlag(processorState));
                if (ret < 0x100)
                {
                    processorState.C = false;
                    if (ret < 0x80)
                        processorState.V = false;
                }
                else
                {
                    processorState.C = true;
                    if (ret >= 0x180)
                        processorState.V = false;
                }
            }
                processorState.A = (byte)(ret & 0xFF);
                processorState.N = ((processorState.A) & 0x80) > 0;
                processorState.Z = !(((processorState.A) & 0xFF) > 0);
        }

        private static byte ReadAndInvertByte(ushort address, Apple2Board mainBoard)
        {
            return (byte)~ReadByte(address, mainBoard);

        }

        private static byte ReadByte(ushort address, Apple2Board mainBoard)
        {
            return mainBoard.ReadByte(address);
        }
    }
}