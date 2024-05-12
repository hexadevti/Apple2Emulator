namespace Runtime.OpCodeProcessors;

internal static class MathOpCodeProcessors
{

    public static void Process_ADC(State processorState, Memory memory, ushort address) 
    {
        ProcessAdc(processorState, ReadByte(address, memory));
    }

    public static void Process_SBC(State processorState, Memory memory, ushort address) 
    {
        ProcessAdc(processorState, ReadAndInvertByte(address, memory));
    }

    private static void ProcessAdc(State processorState, byte value)
    {
        SetAccAndFlags(processorState, value, GetAddToAccWithCarryResult(processorState, value));
    }

    private static void SetAccAndFlags(State processorState, byte value, int result) 
    {
        processorState.A = (byte)result;
        processorState.C = result > byte.MaxValue;
        processorState.V = RegisterFunctions.IsOverflow(value, processorState.A, (byte)result);
        processorState.Z = (byte)result == 0;
        processorState.N = ((byte)result).IsNegative();
    }
    private static int GetAddToAccWithCarryResult(State processorState, byte value) => 
        processorState.A + value + RegisterFunctions.ReadCarryFlag(processorState);

    private static byte ReadAndInvertByte(ushort address, Memory memory) 
    {
        return (byte)~ReadByte(address, memory);

    } 

    private static byte ReadByte(ushort address, Memory memory)
    {
        return memory.ReadByte(address);
    }
}