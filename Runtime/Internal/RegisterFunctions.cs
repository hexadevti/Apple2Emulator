namespace Runtime.Internal
{


    internal static class RegisterFunctions
    {
        public static void IncrementProgramCounter(State processorState, int bytes = 1)
        {
            processorState.PC = (ushort)(processorState.PC + bytes);
        }

        public static bool IsNegative(this byte value1)
        {
            return (value1 & 128) == 128;
        }

        public static bool OverflowSet(this byte value1)
        {
            return (value1 & 64) == 64;
        }

        public static byte ReadCarryFlag(State processorState) =>
            processorState.C ? (byte)0x01 : (byte)0x00;

        public static void WriteStateRegister(State processorState, byte sr)
        {
            processorState.C = (sr & 0x01) == 0x01;
            processorState.Z = (sr & 0x02) == 0x02;
            processorState.I = (sr & 0x04) == 0x04;
            processorState.D = (sr & 0x08) == 0x08;
            // processorState.B = (sr & 0x10) == 0x10;
            processorState.V = (sr & 0x40) == 0x40;
            processorState.N = (sr & 0x80) == 0x80;
        }

        public static bool IsOverflow(byte value1, byte value2, byte result)
        {
            return (value1.IsNegative() == value2.IsNegative())
                   & (value1.IsNegative() != result.IsNegative());
        }
    }
}