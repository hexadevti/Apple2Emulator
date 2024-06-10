using System.Text;
using Runtime.Abstractions;

namespace Runtime.Overlays;

public class EmptySlot7Ovl : IOverLay
{
     
    public EmptySlot7Ovl()
    {
        Start = 0xc700;
        End = 0xc7ff;
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