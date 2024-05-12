namespace Runtime.OpCodeProcessors;

internal static class StoreOpCodeProcessors
{
    public static void Process_STY(State processorState, Memory memory, ushort address)
    {
        memory.Write(address, processorState.Y);
    }

    public static void Process_STX(State processorState, Memory memory, ushort address)
    {
        memory.Write(address, processorState.X);
    }

    public static void Process_STA(State processorState, Memory memory, ushort address)
    {
        memory.Write(address, processorState.A);
    }
}