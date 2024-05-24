
using System.Text;
using Runtime.Abstractions;

namespace Runtime.Overlays;

public class TextPageOvl : IOverLay
{
       
    public TextPageOvl()
    {
        Start = 0x0400;
        End = 0x07ff;
    }

    public int Start { get; }
    public int End { get; }

    public void Write(ushort address, byte b, Memory memory)
    {
        memory.UpdateScreen = true;
    }
    
    public byte Read(ushort address, Memory memory, State state)
    {
        return 0;
    }
}