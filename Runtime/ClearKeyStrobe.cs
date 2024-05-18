using System.Text;
using Runtime.Abstractions;

namespace Runtime;

public class ClearKeyStrobe : IOverLay
{
     
    public ClearKeyStrobe()
    {

        Start = 0xc010;
        End = 0xc010;
          
    }

    public int Start { get; }
    public int End { get; }
    public void Write(ushort address, byte b)
    {
    }

    public byte Read(ushort address, Memory memory)
    {
        memory.KeyPressed = 0x00;
        return 0x00;
    }
}