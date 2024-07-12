using Apple2.Mainboard;

namespace Apple2.CPU
{

    internal static class StoreOpCodeProcessors
    {
        public static void Process_STY(State processorState, Apple2Board mainBoard, ushort address)
        {
            mainBoard.WriteByte(address, processorState.Y);
        }

        public static void Process_STX(State processorState, Apple2Board mainBoard, ushort address)
        {
            mainBoard.WriteByte(address, processorState.X);
        }

        public static void Process_STA(State processorState, Apple2Board mainBoard, ushort address)
        {
            mainBoard.WriteByte(address, processorState.A);
        }
    }
}
