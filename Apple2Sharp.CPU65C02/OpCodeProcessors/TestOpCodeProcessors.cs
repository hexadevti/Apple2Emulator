using Apple2Sharp.Mainboard;

namespace Apple2Sharp.CPU65C02
{
    internal static class TestOpCodeProcessors
    {
        public static void Process_TRB(State processorState, Apple2Board mainBoard, ushort address)
        {
            var b = mainBoard.ReadByte(address);
            var result = (byte)(b & processorState.A);
            processorState.Z = result == 0;
            b = (byte)(b & ~processorState.A);
            mainBoard.WriteByte(address, b);
        }
        public static void Process_TSB(State processorState, Apple2Board mainBoard, ushort address)
        {
            var b = mainBoard.ReadByte(address);
            var result = (byte)(b & processorState.A);
            processorState.Z = result == 0;
            b = (byte)(b | processorState.A);
            mainBoard.WriteByte(address, b);
        }

    }
}