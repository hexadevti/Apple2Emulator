using System.Text;
using Runtime.Abstractions;

namespace Runtime.Overlays;

public class AppleScreenOvl : IOverLay
{
       
    public AppleScreenOvl()
    {
        Start = 0xD012;
        End = 0xD012;
        
    }

    public int Start { get; }
    public int End { get; }

    public void Write(ushort address, byte b, Memory memory)
    {
        if (b == 155) return;
        b = (byte)(b & 0b01111111);
           
        if (b == 13)
        {
            Console.WriteLine();
        }
        else
        {
            var format = Encoding.ASCII.GetString(new[] { b });
            Console.Write(format);
            
            
        }
    }
    
    public byte Read(ushort address, Memory memory)
    {
        return 0;
    }
}