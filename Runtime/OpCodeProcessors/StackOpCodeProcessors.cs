using Runtime.Internal;

namespace Runtime.OpCodeProcessors;

internal static class StackOpCodeProcessors
{
    public static void Process_PHP(State processorState, Memory memory)
    {
        StackFunctions.PushToStack(processorState, memory, I6502StateExtensions.ReadStateRegister(processorState, true));
    }

    public static void Process_PLP(State processorState, Memory memory)
    {
        (processorState, var sr) = StackFunctions.PullFromStack(processorState, memory);
        RegisterFunctions.WriteStateRegister(processorState, sr);
    }

    public static void Process_PLA(State processorState, Memory memory)
    {
        (processorState, var a) = StackFunctions.PullFromStack(processorState, memory);
        processorState.A = a;
        processorState.Z = a == 0;
        processorState.N = a.IsNegative();
    }

    public static void Process_PHA(State processorState, Memory memory)
    {
        StackFunctions.PushToStack(processorState, memory, processorState.A);
    }
}