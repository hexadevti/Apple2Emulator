
using System.Text;
using Runtime.Abstractions;

namespace Runtime;

public class TextPage : IOverLay
{
       
    public TextPage()
    {
        Start = 0x0400;
        End = 0x07ff;
    }

    public int Start { get; }
    public int End { get; }

    public void Write(ushort address, byte b)
    {
        if (b == 155) return;
        b = (byte)(b & 0b01111111);
        var format = Encoding.ASCII.GetString(new[] { b });

        if (b == 13)
        {
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine(address.ToString("X") + ": " + b.ToString("X") + " -> " + format);

        }
        
        //Console.Write(format);

    }
    
    public byte Read(ushort address, Memory memory)
    {
        return 0;
    }
}