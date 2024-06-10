using System.Text;
using Runtime.Abstractions;

namespace Runtime.Overlays;

public class EmptySlot1Ovl : IOverLay
{
     
    public EmptySlot1Ovl()
    {
        Start = 0xc100;
        End = 0xc1ff;
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