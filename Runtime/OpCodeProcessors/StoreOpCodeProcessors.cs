namespace Runtime.OpCodeProcessors
{

    internal static class StoreOpCodeProcessors
    {
        public static void Process_STY(State processorState, MainBoard mainBoard, ushort address)
        {
            mainBoard.WriteByte(address, processorState.Y);
        }

        public static void Process_STX(State processorState, MainBoard mainBoard, ushort address)
        {
            mainBoard.WriteByte(address, processorState.X);
        }

        public static void Process_STA(State processorState, MainBoard mainBoard, ushort address)
        {
            mainBoard.WriteByte(address, processorState.A);
        }
    }
}
