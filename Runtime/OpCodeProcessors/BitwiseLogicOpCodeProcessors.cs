namespace Runtime.OpCodeProcessors;

internal static class BitwiseLogicOpCodeProcessors
{
    public static void Process_EOR(State processorState, Memory memory, ushort address)
    {
        var value = memory.ReadByte(address);
        var result = (byte)(value ^ processorState.A);
        WithFlags(processorState, result);
    }

    public static void Process_ORA(State processorState, Memory memory, ushort address)
    {
        var value = memory.ReadByte(address);
        var result = (byte)(value | processorState.A);
        WithFlags(processorState, result);
    }

    public static void Process_AND(State processorState, Memory memory, ushort address)
    {
        var b = memory.ReadByte(address);
        var result = (byte)(b & processorState.A);
        WithFlags(processorState, result); 
    }

    private static void WithFlags(State processorState, byte result) 
    {
        processorState.A = result;
        processorState.Z = result == 0;
        processorState.N = result.IsNegative();
    }
}