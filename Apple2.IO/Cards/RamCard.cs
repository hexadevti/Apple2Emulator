using System;
using System.Collections;
using System.Text;
using Apple2.Mainboard.Interfaces;

namespace Apple2.Mainboard.Cards
{

    public class RamCard : ICard, IRamCard
    {
        private int _slotNumber = 1;
        private int _banks = 8;
        private byte[] _c000ROM;
        public int SlotNumber
        {
            get { return _slotNumber; }
            set { _slotNumber = value; }
        }

        public byte[] C000ROM
        {
            get { return _c000ROM; }
        }

        public byte[] CC00ROM { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private byte[,] _memoryBankSwitchedRAM1 = new byte[8, 0x2000];
        private byte[,] _memoryBankSwitchedRAM2_1 = new byte[8, 0x1000];
        private byte[,] _memoryBankSwitchedRAM2_2 = new byte[8, 0x1000];

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

        public RamCard(int slotNumber, int banks = 8)
        {
            _slotNumber = slotNumber;
            _banks = banks;
            _c000ROM = Tools.EmptyMemory(0x100, 0);
            MemoryBankBankSelect1_2 = true;
            MemoryBankReadRAM_ROM = false;
            MemoryBankWriteRAM_NoWrite = false;
        }

        public void Write(ushort address, byte b, Apple2Board mainBoard)
        {
            if (address >= 0xd000)
            {
                // if (MemoryBankReadRAM_ROM)
                // {
                    if (address >= 0xd000 && address < 0xe000)
                    {
                        if (MemoryBankBankSelect1_2)
                            _memoryBankSwitchedRAM2_1[_selectedBank, address - 0xd000] = b;
                        else
                            _memoryBankSwitchedRAM2_2[_selectedBank, address - 0xd000] = b;
                    }
                    else
                        _memoryBankSwitchedRAM1[_selectedBank, address - 0xe000] = b;
                //}
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
            if (address >= 0xc080 + _slotNumber * 0x10 && address < 0xc090 + _slotNumber * 0x10)
            {
                if (address == 0xc080 + _slotNumber * 0x10)
                {
                    MemoryBankBankSelect1_2 = true;
                    MemoryBankReadRAM_ROM = true;
                    MemoryBankWriteRAM_NoWrite = false;

                }
                else if (address == 0xc081 + _slotNumber * 0x10)
                {
                    MemoryBankBankSelect1_2 = true;
                    MemoryBankReadRAM_ROM = false;
                    MemoryBankWriteRAM_NoWrite = true;
                }
                else if (address == 0xc082 + _slotNumber * 0x10)
                {
                    MemoryBankBankSelect1_2 = true;
                    MemoryBankReadRAM_ROM = false;
                    MemoryBankWriteRAM_NoWrite = false;
                }
                else if (address == 0xc083 + _slotNumber * 0x10)
                {
                    MemoryBankBankSelect1_2 = true;
                    MemoryBankReadRAM_ROM = true;
                    MemoryBankWriteRAM_NoWrite = true;
                }
                else if (address == 0xc088 + _slotNumber * 0x10)
                {
                    MemoryBankBankSelect1_2 = false;
                    MemoryBankReadRAM_ROM = true;
                    MemoryBankWriteRAM_NoWrite = false;

                }
                else if (address == 0xc089 + _slotNumber * 0x10)
                {
                    MemoryBankBankSelect1_2 = false;
                    MemoryBankReadRAM_ROM = false;
                    MemoryBankWriteRAM_NoWrite = true;
                }
                else if (address == 0xc08a + _slotNumber * 0x10)
                {
                    MemoryBankBankSelect1_2 = false;
                    MemoryBankReadRAM_ROM = false;
                    MemoryBankWriteRAM_NoWrite = false;
                }
                else if (address == 0xc08b + _slotNumber * 0x10)
                {
                    MemoryBankBankSelect1_2 = false;
                    MemoryBankReadRAM_ROM = true;
                    MemoryBankWriteRAM_NoWrite = true;
                }
                else if (_banks > 0 && address == 0xc084 + _slotNumber * 0x10)
                    _selectedBank = 0;
                else if (_banks > 1 && address == 0xc085 + _slotNumber * 0x10)
                    _selectedBank = 1;
                else if (_banks > 2 && address == 0xc086 + _slotNumber * 0x10)
                    _selectedBank = 2;
                else if (_banks > 3 && address == 0xc087 + _slotNumber * 0x10)
                    _selectedBank = 3;
                else if (_banks > 4 && address == 0xc08c + _slotNumber * 0x10)
                    _selectedBank = 4;
                else if (_banks > 5 && address == 0xc08d + _slotNumber * 0x10)
                    _selectedBank = 5;
                else if (_banks > 6 && address == 0xc08e + _slotNumber * 0x10)
                    _selectedBank = 6;
                else if (_banks > 7 && address == 0xc08f + _slotNumber * 0x10)
                    _selectedBank = 7;

            }

            return b;

        }
    }
}