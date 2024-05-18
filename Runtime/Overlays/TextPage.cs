
using System.Text;
using Runtime.Abstractions;

namespace Runtime.Overlays;

public class TextPage : IOverLay
{
       
    public TextPage()
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
    
    public byte Read(ushort address, Memory memory)
    {
        return 0;
    }
}