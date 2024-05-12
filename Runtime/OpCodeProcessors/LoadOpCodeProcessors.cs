namespace Runtime.OpCodeProcessors;

internal static class LoadOpCodeProcessors
{
     public static void Process_LDY(State processorState, Memory memory, ushort address)
     {
        var b = memory.ReadByte(address);
        processorState.Y = b;
        processorState.Z = b == 0;
        processorState.N = RegisterFunctions.IsNegative(b);
     }

     public static void Process_LDX(State processorState, Memory memory, ushort address)
     {
        var b = memory.ReadByte(address);
        processorState.X = b;
        processorState.Z = b == 0;
        processorState.N = RegisterFunctions.IsNegative(b);
     }

     public static void Process_LDA(State processorState, Memory memory, ushort address)
     {
        var b = memory.ReadByte(address);
        processorState.A = b;
        processorState.Z = b == 0;
        processorState.N = RegisterFunctions.IsNegative(b);
     }
}