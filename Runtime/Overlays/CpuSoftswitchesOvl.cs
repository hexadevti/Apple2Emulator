using System.Collections;
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
        ProcessSwitch(address, b, memory, null);
    }

    public byte Read(ushort address, Memory memory, State state)
    {
        return ProcessSwitch(address, 0x00, memory, state);
    }

    private byte ProcessSwitch(ushort address, byte b, Memory memory, State? state)
    {
        if (address == 0xc000)
            return memory.KeyPressed;
        if (address == 0xc010)
            memory.KeyPressed = b;
        if (address == 0xc030)
            memory.softswitches.SoundClick = true;
        if (address == 0xc050)
            memory.softswitches.Graphics_Text = true;
        if (address == 0xc051)
            memory.softswitches.Graphics_Text = false;
        if (address == 0xc052)
            memory.softswitches.DisplayFull_Split = true;
        if (address == 0xc053)
            memory.softswitches.DisplayFull_Split = false;
        if (address == 0xc054)
        {
            lock (memory.displayLock)
            {
                memory.softswitches.TextPage1_Page2 = true;
            }
        }
        if (address == 0xc055)
        {
            lock (memory.displayLock)
            {
                memory.softswitches.TextPage1_Page2 = false;
            }
        }
        if (address == 0xc056)
            memory.softswitches.LoRes_HiRes = true;
        if (address == 0xc057)
            memory.softswitches.LoRes_HiRes = false;

        if (address >= 0xc080)
        {
            var last4bits = (address & 0b00001111);
            BitArray bits = new BitArray(new byte[] { (byte)last4bits });
            memory.softswitches.MemoryBankBankSelect1_2 = bits[3];
            if (bits[1] && bits[0])
            {
                memory.softswitches.MemoryBankReadRAM_ROM = true;
                memory.softswitches.MemoryBankWriteRAM_NoWrite = true;
            }
            else if (!bits[1] && bits[0])
            {
                memory.softswitches.MemoryBankReadRAM_ROM = false;
                memory.softswitches.MemoryBankWriteRAM_NoWrite = true;
            }
            else if (bits[1] && !bits[0])
            {
                memory.softswitches.MemoryBankReadRAM_ROM = false;
                memory.softswitches.MemoryBankWriteRAM_NoWrite = false;
            }
            else if (!bits[1] && !bits[0])
            {
                memory.softswitches.MemoryBankReadRAM_ROM = true;
                memory.softswitches.MemoryBankWriteRAM_NoWrite = false;
            }
        }

        return 0;

    }
}