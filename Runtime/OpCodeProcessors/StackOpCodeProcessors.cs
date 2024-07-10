using Runtime.Internal;

namespace Runtime.OpCodeProcessors
{
    internal static class StackOpCodeProcessors
    {
        public static void Process_PHP(State processorState, MainBoard mainBoard)
        {
            StackFunctions.PushToStack(processorState, mainBoard, I6502StateExtensions.ReadStateRegister(processorState, true));
        }

        public static void Process_PLP(State processorState, MainBoard mainBoard)
        {
            var sr = StackFunctions.PullFromStack(processorState, mainBoard);
            RegisterFunctions.WriteStateRegister(processorState, sr);
        }

        public static void Process_PLA(State processorState, MainBoard mainBoard)
        {
            var a = StackFunctions.PullFromStack(processorState, mainBoard);
            processorState.A = a;
            processorState.Z = a == 0;
            processorState.N = a.IsNegative();
        }

        public static void Process_PHA(State processorState, MainBoard mainBoard)
        {
            StackFunctions.PushToStack(processorState, mainBoard, processorState.A);
        }
    }
}