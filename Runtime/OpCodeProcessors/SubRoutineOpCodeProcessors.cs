namespace Runtime.OpCodeProcessors;

internal static class SubRoutineOpCodeProcessors
{
    public static void Process_RTS(State processorState, Memory memory)
    {
       (processorState, var lowByte) = StackFunctions.PullFromStack(processorState, memory);
        (processorState, var highByte) = StackFunctions.PullFromStack(processorState, memory);
        
        var address = BitConverter.ToUInt16(new[] { lowByte, highByte });
        BranchOpCodeProcessors.Process_JMP(processorState, (ushort)(address+1));
    }

    public static void Process_JSR(State processorState, Memory memory, ushort address)
    {
        StackFunctions.PushToStack(processorState, memory, (ushort)(processorState.PC-1));
        BranchOpCodeProcessors.Process_JMP(processorState, address);
    }

    public static void Process_RTI(State processorState, Memory memory)
    {
        (processorState, var sr) = StackFunctions.PullFromStack(processorState, memory);
        (processorState, var h) = StackFunctions.PullFromStack(processorState, memory);
        (processorState, var l) = StackFunctions.PullFromStack(processorState, memory);
       
        var returnAddress = BitConverter.ToUInt16(new byte[] { h, l });
        processorState.PC = returnAddress;
        RegisterFunctions.WriteStateRegister(processorState, sr);
    }

    public static void Process_BRK(State processorState, Memory memory)
    {
        var returnAddress = BitConverter.GetBytes((ushort)(processorState.PC + 1));
        
        StackFunctions.PushToStack(processorState, memory, returnAddress[1]);
        StackFunctions.PushToStack(processorState, memory, returnAddress[0]);
        StackFunctions.PushToStack(processorState, memory, I6502StateExtensions.ReadStateRegister(processorState));
        processorState.PC = memory.GetIRQVector();
        processorState.I = true;
    }
}