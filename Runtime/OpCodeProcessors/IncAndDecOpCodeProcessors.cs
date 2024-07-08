namespace Runtime.OpCodeProcessors;

internal static class IncAndDecOpCodeProcessors
{
    public static void Process_DEX(State processorState)
    {
        var value = (byte)(processorState.X - 1);
        SetX(processorState, value);
        WithFlags(processorState, value);
    }

    public static void Process_DEY(State processorState)
    {
        var value = (byte)(processorState.Y - 1);
        SetY(processorState, value);
        WithFlags(processorState, value);
    }

    public static void Process_INX(State processorState)
    {
        var value = (byte)(processorState.X + 1);
        SetX(processorState, value);
        WithFlags(processorState, value);
    }

    public static void Process_INY(State processorState)
    {
        var value = (byte)(processorState.Y + 1);
        SetY(processorState, value);
        WithFlags(processorState, value);
    }

    public static void Process_DEC(State processorState, MainBoard mainBoard, ushort address)
    {
        var value = mainBoard.ReadByte(address);
        value = (byte)(value - 1);
        mainBoard.Write(address, value);
        WithFlags(processorState, value);
    }

    public static void Process_INC(State processorState, MainBoard mainBoard, ushort address)
    {
        var value = mainBoard.ReadByte(address);
        var result = (byte)(value + 1);
        mainBoard.Write(address, result);
        WithFlags(processorState, result);
    }

    private static void WithFlags(State processorState, byte value) 
    {
        processorState.Z = value == 0;
        processorState.N = value.IsNegative();
    }
    private static void SetX(State processorState, byte value) 
    {
        processorState.X = value;
    }
    private static void SetY(State processorState, byte value) 
    {
        processorState.Y = value;
    }
}