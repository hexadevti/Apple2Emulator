namespace Runtime;

internal static class StackFunctions
{

    public static (State, byte) PullFromStack(State processorState, Memory memory)
    {
        processorState.S = (byte)(processorState.S + 1);
        var pulledValue = memory.ReadByte(GetCurrentStackAddress(processorState));
        return (processorState, pulledValue);
    }


    public static void PushToStack(State processorState, Memory memory, ushort value) =>
        BitConverter.GetBytes(value).Reverse()
            .Aggregate(processorState, (current, b) => PushToStack(processorState, memory, b));

    public static State PushToStack(State processorState, Memory memory, byte value)
    {
        memory.Write(GetCurrentStackAddress(processorState), value);
        processorState.S = (byte)(processorState.S - 1);
        return processorState;
    }

    private static ushort GetCurrentStackAddress(State processorState)
    {
        return BitConverter.ToUInt16(new byte []{ processorState.S, 0x01 });
    }
}