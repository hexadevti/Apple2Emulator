using System.Collections;
using System.Text;
using Runtime.Abstractions;

namespace Runtime.Cards;

public class LanguageCard : ICard
{
    public int SlotNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public byte[] C000ROM => throw new NotImplementedException();

    public byte[] CC00ROM { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Write(ushort address, byte b, MainBoard mainBoard)
    {
        ProcessSwitch(address, b, mainBoard, null);
    }

    public byte Read(ushort address, MainBoard mainBoard, State state)
    {
        return ProcessSwitch(address, 0x00, mainBoard, state);
    }

    private byte ProcessSwitch(ushort address, byte b, MainBoard mainBoard, State? state)
    {
        if (address >= 0xc080)
        {
            var last4bits = (address & 0b00001111);
            BitArray bits = new BitArray(new byte[] { (byte)last4bits });
            mainBoard.softswitches.MemoryBankBankSelect1_2 = bits[3];
            if (bits[1] && bits[0])
            {
                mainBoard.softswitches.MemoryBankReadRAM_ROM = true;
                mainBoard.softswitches.MemoryBankWriteRAM_NoWrite = true;
            }
            else if (!bits[1] && bits[0])
            {
                mainBoard.softswitches.MemoryBankReadRAM_ROM = false;
                mainBoard.softswitches.MemoryBankWriteRAM_NoWrite = true;
            }
            else if (bits[1] && !bits[0])
            {
                mainBoard.softswitches.MemoryBankReadRAM_ROM = false;
                mainBoard.softswitches.MemoryBankWriteRAM_NoWrite = false;
            }
            else if (!bits[1] && !bits[0])
            {
                mainBoard.softswitches.MemoryBankReadRAM_ROM = true;
                mainBoard.softswitches.MemoryBankWriteRAM_NoWrite = false;
            }
        }

        return 0;

    }
}