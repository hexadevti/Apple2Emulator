
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Runtime.Abstractions;

namespace Runtime.Overlays;

public class Cols80Card
{
    int slot = 3;
    public void Write(ushort address, byte b, Memory memory)
    {
        ProcessC0xx(address, memory);
    }


    public byte Read(ushort address, Memory memory, State state)
    {
        ProcessC0xx(address, memory);
        return 0;
    }

    private void ProcessC0xx(ushort address, Memory memory)
    {
        if (address == 0xc080 + slot * 0x10)
        {
            memory.softswitches.cols80PageSelect = 0;
        }
        else if (address == 0xc084 + slot * 0x10)
        {
            memory.softswitches.cols80PageSelect = 1;
        }
        else if (address == 0xc088 + slot * 0x10)
        {
            memory.softswitches.cols80PageSelect = 2;
        }
        else if (address == 0xc08c  + slot * 0x10)
        {
            memory.softswitches.cols80PageSelect = 3;
        }
    }

    
}
