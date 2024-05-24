namespace Runtime.Abstractions;

public interface IOverLay
{
    int Start { get; }
    int End { get; }
    void Write(ushort address, byte b, Memory memory);
    byte Read(ushort address, Memory memory, State state);
}