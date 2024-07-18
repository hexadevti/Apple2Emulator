using Apple2Sharp.Mainboard;

namespace Apple2Sharp.CPU6502
{
    internal static class CompareOpCodeProcessors
    {
        public static void Process_CPX(State processorState, Apple2Board mainBoard, ushort address) => Compare(processorState, mainBoard.ReadByte(address), processorState.X);
        public static void Process_CPY(State processorState, Apple2Board mainBoard, ushort address) => Compare(processorState, mainBoard.ReadByte(address), processorState.Y);
        public static void Process_CMP(State processorState, Apple2Board mainBoard, ushort address) => Compare(processorState, mainBoard.ReadByte(address), processorState.A);

        public static void Process_BIT(State processorState, Apple2Board mainBoard, ushort address)
        {
            var b = mainBoard.ReadByte(address);
            var result = (byte)(b & processorState.A);


            processorState.Z = result == 0;
            processorState.N = b.IsNegative();
            processorState.V = b.OverflowSet();

        }

        private static void Compare(State processorState, byte value, byte register)
        {
            value = (byte)(value ^ 0xff);
            var carry = register + value + 1;

            var b = (byte)carry;
            processorState.C = carry > 0xff;
            processorState.Z = b == 0;
            processorState.N = ((byte)(b & 0xff)).IsNegative();
        }
    }
}