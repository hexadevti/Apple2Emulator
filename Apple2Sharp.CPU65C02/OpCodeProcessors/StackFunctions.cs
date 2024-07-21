using System;
using System.Linq;
using Apple2Sharp.CPU65C02;
using Apple2Sharp.Mainboard;

namespace Runtime
{
    internal static class StackFunctions
    {

        public static byte PullFromStack(State processorState, Apple2Board mainBoard)
        {
            processorState.S = (byte)(processorState.S + 1);
            var pulledValue = mainBoard.ReadByte(GetCurrentStackAddress(processorState));
            return pulledValue;
        }


        public static void PushToStack(State processorState, Apple2Board mainBoard, ushort value) =>
            BitConverter.GetBytes(value).Reverse()
                .Aggregate(processorState, (current, b) => PushToStack(processorState, mainBoard, b));

        public static State PushToStack(State processorState, Apple2Board mainBoard, byte value)
        {
            mainBoard.WriteByte(GetCurrentStackAddress(processorState), value);
            processorState.S = (byte)(processorState.S - 1);
            return processorState;
        }

        private static ushort GetCurrentStackAddress(State processorState)
        {
            return BitConverter.ToUInt16(new byte[] { processorState.S, 0x01 });
        }
    }
}