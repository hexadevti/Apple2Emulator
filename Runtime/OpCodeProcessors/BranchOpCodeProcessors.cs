namespace Runtime.OpCodeProcessors
{
    internal static class BranchOpCodeProcessors
    {
        public static void Process_BPL(State processorState, ushort address)
        {
            if (!processorState.N)
                Process_JMP(processorState, address);
        }

        public static void Process_BVC(State processorState, ushort address)
        {
            if (!processorState.V)
                Process_JMP(processorState, address);
        }

        public static void Process_BVS(State processorState, ushort address)
        {
            if (processorState.V)
                Process_JMP(processorState, address);
        }

        public static void Process_BMI(State processorState, ushort address)
        {
            if (processorState.N)
                Process_JMP(processorState, address);
        }

        public static void Process_BCS(State processorState, ushort address)
        {
            if (processorState.C)
                Process_JMP(processorState, address);
        }

        public static void Process_BNE(State processorState, ushort address)
        {
            if (!processorState.Z)
                Process_JMP(processorState, address);
        }

        public static void Process_BCC(State processorState, ushort address)
        {
            if (!processorState.C)
                Process_JMP(processorState, address);
        }

        public static void Process_BEQ(State processorState, ushort address)
        {
            if (processorState.Z)
                Process_JMP(processorState, address);
        }

        public static void Process_JMP(State processorState, ushort address)
        {
            processorState.PC = address;
        }

    }
}