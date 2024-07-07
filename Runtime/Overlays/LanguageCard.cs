using System.Collections;
using System.Text;
using Runtime.Abstractions;

namespace Runtime.Overlays;

public class LanguageCard
{
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