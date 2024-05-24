using System.Text;
using Runtime.Abstractions;

namespace Runtime.Overlays;

public class CpuSoftswitchesOvl : IOverLay
{
     
    public CpuSoftswitchesOvl()
    {

        Start = 0xc000;
        End = 0xc08f;
          
    }

    public int Start { get; }
    public int End { get; }
    public void Write(ushort address, byte b, Memory memory)
    {
    }

    public byte Read(ushort address, Memory memory, State state)
    {
        
        if (address==0xc010)
            memory.KeyPressed = 0x00;
        if (address==0xc050)
            memory.softswitches.Graphics_Text = true;
        if (address==0xc051)
            memory.softswitches.Graphics_Text = false;
        if (address==0xc052)
            memory.softswitches.DisplayFull_Split = true;
        if (address==0xc053)
            memory.softswitches.DisplayFull_Split = false;
        if (address==0xc054)
            memory.softswitches.TextPage1_Page2 = true;
        if (address==0xc055)
            memory.softswitches.TextPage1_Page2 = false;
        if (address==0xc056)
            memory.softswitches.LoRes_HiRes = true;
        if (address==0xc057)
            memory.softswitches.LoRes_HiRes = false;


        return 0x00;
    }
}