using System.Text;
using Runtime.Abstractions;

namespace Runtime.Overlays;

public class EmptySlot4Ovl : IOverLay
{
     
    public EmptySlot4Ovl()
    {
        Start = 0xc400;
        End = 0xc4ff;
    }

    public int Start { get; }
    public int End { get; }
    public void Write(ushort address, byte b, Memory memory)
    {
        
    }

    public byte Read(ushort address, Memory memory, State state)
    {
        Random rnd = new Random();
        byte[] b = new byte[1];
        rnd.NextBytes(b);
        return b[0];
    }
}