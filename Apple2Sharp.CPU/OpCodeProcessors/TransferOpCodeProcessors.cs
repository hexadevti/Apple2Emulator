namespace Apple2Sharp.CPU
{


    internal static class TransferOpCodeProcessors
    {
        public static void Process_TYA(State processorState)
        {
            var value = processorState.Y;
            processorState.Z = value == 0;
            processorState.N = value.IsNegative();
            processorState.A = value;
        }

        public static void Process_TXS(State processorState)
        {
            var value = processorState.X;
            processorState.S = value;
        }

        public static void Process_TXA(State processorState)
        {
            var value = processorState.X;
            processorState.Z = value == 0;
            processorState.N = value.IsNegative();
            processorState.A = value;
        }

        public static void Process_TSX(State processorState)
        {
            var value = processorState.S;
            processorState.Z = value == 0;
            processorState.N = value.IsNegative();
            processorState.X = value;
        }

        public static void Process_TAX(State processorState)
        {
            var value = processorState.A;
            processorState.Z = value == 0;
            processorState.N = value.IsNegative();
            processorState.X = value;
        }

        public static void Process_TAY(State processorState)
        {
            var value = processorState.A;
            processorState.Z = value == 0;
            processorState.N = value.IsNegative();
            processorState.Y = value;
        }
    }
}