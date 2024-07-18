

using System;
using Apple2Sharp.Mainboard;

namespace Apple2Sharp.CPU6502
{



    internal static class SubRoutineOpCodeProcessors
    {
        public static void Process_RTS(State processorState, Apple2Board mainBoard)
        {
            var lowByte = StackFunctions.PullFromStack(processorState, mainBoard);
            var highByte = StackFunctions.PullFromStack(processorState, mainBoard);

            var address = BitConverter.ToUInt16(new[] { lowByte, highByte });
            BranchOpCodeProcessors.Process_JMP(processorState, (ushort)(address + 1));
        }

        public static void Process_JSR(State processorState, Apple2Board mainBoard, ushort address)
        {
            StackFunctions.PushToStack(processorState, mainBoard, (ushort)(processorState.PC - 1));
            BranchOpCodeProcessors.Process_JMP(processorState, address);
        }

        public static void Process_RTI(State processorState, Apple2Board mainBoard)
        {
            var sr = StackFunctions.PullFromStack(processorState, mainBoard);
            var h = StackFunctions.PullFromStack(processorState, mainBoard);
            var l = StackFunctions.PullFromStack(processorState, mainBoard);

            var returnAddress = BitConverter.ToUInt16(new byte[] { h, l });
            processorState.PC = returnAddress;
            RegisterFunctions.WriteStateRegister(processorState, sr);
        }

        public static void Process_BRK(State processorState, Apple2Board mainBoard)
        {
            var returnAddress = BitConverter.GetBytes((ushort)(processorState.PC + 1));

            StackFunctions.PushToStack(processorState, mainBoard, returnAddress[1]);
            StackFunctions.PushToStack(processorState, mainBoard, returnAddress[0]);
            StackFunctions.PushToStack(processorState, mainBoard, I6502StateExtensions.ReadStateRegister(processorState));
            processorState.PC = mainBoard.GetIRQVector();
            processorState.I = true;
        }
    }
}