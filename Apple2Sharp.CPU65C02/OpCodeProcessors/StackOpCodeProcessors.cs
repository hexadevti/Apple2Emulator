using Apple2Sharp.Mainboard;

namespace Apple2Sharp.CPU65C02
{
    internal static class StackOpCodeProcessors
    {
        public static void Process_PHP(State processorState, Apple2Board mainBoard)
        {
            StackFunctions.PushToStack(processorState, mainBoard, I6502StateExtensions.ReadStateRegister(processorState, true));
        }

        public static void Process_PLP(State processorState, Apple2Board mainBoard)
        {
            var sr = StackFunctions.PullFromStack(processorState, mainBoard);
            RegisterFunctions.WriteStateRegister(processorState, sr);
        }

        public static void Process_PLA(State processorState, Apple2Board mainBoard)
        {
            var a = StackFunctions.PullFromStack(processorState, mainBoard);
            processorState.A = a;
            processorState.Z = a == 0;
            processorState.N = a.IsNegative();
        }
        public static void Process_PLX(State processorState, Apple2Board mainBoard)
        {
            var a = StackFunctions.PullFromStack(processorState, mainBoard);
            processorState.X = a;
            processorState.Z = a == 0;
            processorState.N = a.IsNegative();
        }
        public static void Process_PLY(State processorState, Apple2Board mainBoard)
        {
            var a = StackFunctions.PullFromStack(processorState, mainBoard);
            processorState.Y = a;
            processorState.Z = a == 0;
            processorState.N = a.IsNegative();
        }

        public static void Process_PHA(State processorState, Apple2Board mainBoard)
        {
            StackFunctions.PushToStack(processorState, mainBoard, processorState.A);
        }

        public static void Process_PHX(State processorState, Apple2Board mainBoard)
        {
            StackFunctions.PushToStack(processorState, mainBoard, processorState.X);
        }

        public static void Process_PHY(State processorState, Apple2Board mainBoard)
        {
            StackFunctions.PushToStack(processorState, mainBoard, processorState.Y);
        }
    }
}