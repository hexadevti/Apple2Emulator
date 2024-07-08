using System.Collections;
using System.Text;
using Runtime.Abstractions;

namespace Runtime.Cards;

public class LanguageCard : ICard
{
    public int SlotNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public byte[] C000ROM => throw new NotImplementedException();

    public byte[] CC00ROM { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public byte[] memoryBankSwitchedRAM1 = new byte[0x3000];
    public byte[] memoryBankSwitchedRAM2_1 = new byte[0x1000];
    public byte[] memoryBankSwitchedRAM2_2 = new byte[0x1000];

    public void Write(ushort address, byte b, MainBoard mainBoard)
    {
        if (address >= 0xd000)
        {
            if (mainBoard.softswitches.MemoryBankReadRAM_ROM)
            {
                if (address >= 0xd000 && address < 0xe000)
                {
                    if (mainBoard.softswitches.MemoryBankBankSelect1_2)
                        this.memoryBankSwitchedRAM2_1[address - 0xd000] = b;
                    else
                        this.memoryBankSwitchedRAM2_2[address - 0xd000] = b;
                }
                else
                    this.memoryBankSwitchedRAM1[address - 0xe000] = b;
            }
        }
        ProcessSwitch(address, b, mainBoard, null);
    }

    public byte Read(ushort address, MainBoard mainBoard, State state)
    {
        byte ret = 0;
        if (address >= 0xd000 && address < 0xe000)
        {
            if (mainBoard.softswitches.MemoryBankBankSelect1_2)
                ret = memoryBankSwitchedRAM2_1[address - 0xd000];
            else
                ret = memoryBankSwitchedRAM2_2[address - 0xd000];
        }
        else if (address >= 0xd000)
            ret = memoryBankSwitchedRAM1[address - 0xe000];

        return ProcessSwitch(address, ret, mainBoard, state);
    }

    private byte ProcessSwitch(ushort address, byte b, MainBoard mainBoard, State? state)
    {
        if (address >= 0xc080 && address < 0xc090)
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

        return b;

    }
}