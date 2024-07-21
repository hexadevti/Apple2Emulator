using Apple2Sharp.Mainboard;

namespace Apple2Sharp.CPU65C02
{
    internal static class ShiftAndRollOpCodeProcessors
    {

        public static void Process_ROL(State processorState, Apple2Board mainBoard, ushort? address)
        {
            var value = address == null ? processorState.A : mainBoard.ReadByte(address.Value);

            value = (byte)((byte)(value << 1) | (byte)(value >> 7));
            var carry = (value & 0b0000001) == 1;
            value = (byte)(processorState.C ? value | 1 : value & 0b11111110);
            if (address != null) mainBoard.WriteByte(address.Value, value);

            processorState.Z = value == 0;
            processorState.C = carry;
            processorState.N = value.IsNegative();

            if (address == null)
                processorState.A = value;
        }

        public static void Process_ROR(State processorState, Apple2Board mainBoard, ushort? address)
        {
            var value = address == null ? processorState.A : mainBoard.ReadByte(address.Value);

            value = (byte)((byte)(value >> 1) | (byte)(value << 7));

            var carry = (value & 0b10000000) == 0b10000000;
            value = (byte)(processorState.C ? value | 0b10000000 : value & 0b01111111);

            if (address != null) mainBoard.WriteByte(address.Value, value);

            processorState.Z = value == 0;
            processorState.C = carry;
            processorState.N = value.IsNegative();

            if (address == null)
                processorState.A = value;
        }

        public static void Process_LSR(State processorState, Apple2Board mainBoard, ushort? address)
        {
            var value = address == null ? processorState.A : mainBoard.ReadByte(address.Value);

            var carry = (value & 1) == 1;

            value = (byte)(value >> 1);

            if (address != null) mainBoard.WriteByte(address.Value, value);

            processorState.Z = value == 0;
            processorState.C = carry;
            processorState.N = value.IsNegative();

            if (address == null)
                SetAccToNewValue(processorState, value);
        }

        public static void Process_ASL(State processorState, Apple2Board mainBoard, ushort? address)
        {
            if (address == null)
            {
                Process_ASL_A(processorState);
            }
            else
                Process_ASL_M(processorState, mainBoard, address.Value);
        }

        private static void Process_ASL_M(State processorState, Apple2Board mainBoard, ushort address)
        {
            var currentValue = mainBoard.ReadByte(address);
            var newValue = (byte)(currentValue << 1);
            mainBoard.WriteByte(address, newValue);
            WithFlags(processorState, newValue, currentValue);
        }

        private static void Process_ASL_A(State processorState)
        {
            var currentValue = processorState.A;
            var newValue = (byte)(currentValue << 1);
            SetAccToNewValue(processorState, newValue);
            WithFlags(processorState, newValue, currentValue);
        }

        private static void SetAccToNewValue(State processorState, byte newValue)
        {
            processorState.A = newValue;
        }

        private static void WithFlags(State processorState, byte newValue, byte currentValue)
        {
            processorState.Z = newValue == 0;
            processorState.N = newValue.IsNegative();
            processorState.C = (currentValue & 0x80) == 0x80;
        }
    }
}