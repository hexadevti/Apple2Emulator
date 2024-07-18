using Apple2Sharp.Mainboard;

namespace Apple2Sharp.CPU6502
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
            if (processorState.D)
            {
                processorState.Z = !(((byte)(processorState.A + value + RegisterFunctions.ReadCarryFlag(processorState)) & 0xFF) > 0);
                processorState.N = ((byte)result).IsNegative();
                processorState.V = (((processorState.A ^ result) & 0x80) > 0) && !(((processorState.A ^ value) & 0x80) > 0);
                if ((result & 0x1F0) > 0x90)
                    result += 0x60;
                processorState.C = ((result & 0xFF0) > 0xF0);
                processorState.A = (byte)(result & 0xFF);
            }
            else
            {
                processorState.V = RegisterFunctions.IsOverflow(value, processorState.A, (byte)result);
                processorState.A = (byte)result;
                processorState.C = result > byte.MaxValue;
                processorState.Z = (byte)result == 0;
                processorState.N = ((byte)result).IsNegative();
            }
        }

        private static int GetAddToAccWithCarryResult(State processorState, byte value)
        {
            if (processorState.D)
            {
                var ret = 0;
                ret = (processorState.A & 0x0F) + (value & 0x0F) + RegisterFunctions.ReadCarryFlag(processorState);
                if (ret > 0x09)
                    ret += 0x06;
                if (ret <= 0x0F)
                    ret = (ret & 0x0F) + (processorState.A & 0xF0) + (value & 0xF0);
                else
                    ret = (ret & 0x0F) + (processorState.A & 0xF0) + (value & 0xF0) + 0x10;
                return ret;
            }
            else
                return value + processorState.A + RegisterFunctions.ReadCarryFlag(processorState);
        }

        private static void GetSbtToAccWithCarryResult(State processorState, byte value)
        {
            var ret = 0;
            ushort value2 = (ushort)(processorState.A - value - (processorState.C ? (byte)0x00 : (byte)0x01));

            if (processorState.D)
            {
                ret = (processorState.A & 0x0F) - (value & 0x0F) - (processorState.C ? (byte)0x00 : (byte)0x01);
                if ((ret & 0x10) > 0)
                    ret = ((ret - 0x06) & 0x0F) | ((processorState.A & 0xF0) - (value & 0xF0) - 0x10);
                else
                    ret = (ret & 0x0F) | ((processorState.A & 0xF0) - (value & 0xF0));
                if ((ret & 0x100) > 0)
                    ret -= 0x60;

                processorState.C = (value2 < 0x100);
                processorState.N = (((value2 & 0xFF) & 0x80) > 0);
                processorState.Z = !(((value2 & 0xFF) & 0xFF) > 0);
                processorState.V = (((processorState.A ^ value2) & 0x80) > 0) && (((processorState.A ^ value) & 0x80) > 0);
                processorState.A = (byte)(ret & 0xFF);

            }
            else
            {
                ret = value2;
                processorState.C = (ret < 0x100);
                processorState.V = (((processorState.A & 0x80) != (value & 0x80)) && ((processorState.A & 0x80) != (ret & 0x80)));
                processorState.A = (byte)(ret & 0xFF);
                processorState.N = ((processorState.A) & 0x80) > 0;
                processorState.Z = !(((processorState.A) & 0xFF) > 0);
            }
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