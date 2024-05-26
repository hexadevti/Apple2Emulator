using System.Text;
using Runtime.Abstractions;

namespace Runtime.Overlays;

public class KeyboardOvl : IOverLay
{
     
    public KeyboardOvl()
    {
        Start = 0xc000;
        End = 0xc000;
    }

    public int Start { get; }
    public int End { get; }
    public void Write(ushort address, byte b, Memory memory)
    {
        
    }

    public byte Read(ushort address, Memory memory, State state)
    {
        return memory.KeyPressed;
    }
}