using System;
using System.Collections;
using System.Text;
using Apple2.Mainboard.Abstractions;

namespace Apple2.Mainboard.Cards
{



    public class LanguageCard : ICard, IRamCard
    {
        public int SlotNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public byte[] C000ROM => throw new NotImplementedException();

        public byte[] CC00ROM { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private byte[,] _memoryBankSwitchedRAM1 = new byte[1, 0x2000];
        private byte[,] _memoryBankSwitchedRAM2_1 = new byte[1, 0x1000];
        private byte[,] _memoryBankSwitchedRAM2_2 = new byte[1, 0x1000];

        public bool MemoryBankBankSelect1_2 { get; set; }

        public bool MemoryBankReadRAM_ROM { get; set; }

        public bool MemoryBankWriteRAM_NoWrite { get; set; }
        public byte[,] MemoryBankSwitchedRAM1
        {
            get { return _memoryBankSwitchedRAM1; }
            set { _memoryBankSwitchedRAM1 = value; }
        }
        public byte[,] MemoryBankSwitchedRAM2_1
        {
            get { return _memoryBankSwitchedRAM2_1; }
            set { _memoryBankSwitchedRAM2_1 = value; }
        }

        public byte[,] MemoryBankSwitchedRAM2_2
        {
            get { return _memoryBankSwitchedRAM2_2; }
            set { _memoryBankSwitchedRAM2_2 = value; }
        }

        public int SelectedBank
        {
            get { return _selectedBank; }
            set { _selectedBank = value; }
        }

        private int _selectedBank = 0;


        public void Write(ushort address, byte b, Apple2Board mainBoard)
        {
            if (address >= 0xd000)
            {
                if (MemoryBankReadRAM_ROM)
                {
                    if (address >= 0xd000 && address < 0xe000)
                    {
                        if (MemoryBankBankSelect1_2)
                            _memoryBankSwitchedRAM2_1[_selectedBank, address - 0xd000] = b;
                        else
                            _memoryBankSwitchedRAM2_2[_selectedBank, address - 0xd000] = b;
                    }
                    else
                        _memoryBankSwitchedRAM1[_selectedBank, address - 0xe000] = b;
                }
            }
            ProcessSwitch(address, b, mainBoard);
        }

        public byte Read(ushort address, Apple2Board mainBoard)
        {
            byte ret = 0;
            if (address >= 0xd000 && address < 0xe000)
            {
                if (MemoryBankBankSelect1_2)
                    ret = _memoryBankSwitchedRAM2_1[_selectedBank, address - 0xd000];
                else
                    ret = _memoryBankSwitchedRAM2_2[_selectedBank, address - 0xd000];
            }
            else if (address >= 0xd000)
                ret = _memoryBankSwitchedRAM1[_selectedBank, address - 0xe000];

            return ProcessSwitch(address, ret, mainBoard);
        }

        private byte ProcessSwitch(ushort address, byte b, Apple2Board mainBoard)
        {
            if (address >= 0xc080 && address < 0xc090)
            {
                var last4bits = (address & 0b00001111);
                BitArray bits = new BitArray(new byte[] { (byte)last4bits });
                MemoryBankBankSelect1_2 = bits[3];
                if (bits[1] && bits[0])
                {
                    MemoryBankReadRAM_ROM = true;
                    MemoryBankWriteRAM_NoWrite = true;
                }
                else if (!bits[1] && bits[0])
                {
                    MemoryBankReadRAM_ROM = false;
                    MemoryBankWriteRAM_NoWrite = true;
                }
                else if (bits[1] && !bits[0])
                {
                    MemoryBankReadRAM_ROM = false;
                    MemoryBankWriteRAM_NoWrite = false;
                }
                else if (!bits[1] && !bits[0])
                {
                    MemoryBankReadRAM_ROM = true;
                    MemoryBankWriteRAM_NoWrite = false;
                }
            }

            return b;

        }
    }
}