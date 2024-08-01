using System;
using System.Collections.Generic;
using System.Threading;
using Apple2Sharp.Mainboard.Interfaces;

namespace Apple2Sharp.Mainboard
{
    public class Apple2Board
    {
        public object displayLock = new object();
        public Dictionary<byte, bool[,]> charSet = new Dictionary<byte, bool[,]>();
        public Dictionary<byte, bool[,]> altCharSet = new Dictionary<byte, bool[,]>();

        public Dictionary<byte, bool[,]> CharSet80 = new Dictionary<byte, bool[,]>();

        public bool adjust1Mhz = true;
        public double clockSpeed = 0;
        public byte[] baseZP = new byte[0x200];
        public byte[] baseRAM = new byte[0xc0000];
        public byte[] ROM = new byte[0x3000];
        public byte[] AppleIIeInternalROM = new byte[0x1000];
        public byte KeyPressedBuffer { get; set; }
        public Softswitches softswitches = new Softswitches();
        public Queue<byte[]> clickBuffer = new Queue<byte[]>(100);
        public int cpuCycles { get; set; }
        public ICard[] slots = new ICard[8];
        public Queue<string> screenLog = new Queue<string>();
        public Queue<bool> cycleWait = new Queue<bool>();
        public int audioJumpInterval = 25;
        public int selectedC8Slot = 0;
        public bool videoColor = true;
        public bool scanLines = true;
        public bool idealized = false;
        public bool joystick = false;
        public bool appleIIe = false;
        public int timerpdl0;
        public int timerpdl1;
        public int timerpdl2;
        public int timerpdl3;


        public int IIEAuxBanks;
        private byte[] IIEmemoryBankSwitchedRAM1 = new byte[0x2000];
        private byte[] IIEmemoryBankSwitchedRAM2_1 = new byte[0x1000];
        private byte[] IIEmemoryBankSwitchedRAM2_2 = new byte[0x1000];
        public byte[,] auxZP = new byte[0, 0x200];
        public byte[,] auxRAM = new byte[0, 0xc0000];

        private byte[,] IIEAuxBankSwitchedRAM1 = new byte[0, 0x2000];
        private byte[,] IIEAuxBankSwitchedRAM2_1 = new byte[0, 0x1000];
        private byte[,] IIEAuxBankSwitchedRAM2_2 = new byte[0, 0x1000];


        public bool videoMain_Aux = true;

        public Apple2Board()
        {
            IIEAuxBanks = 32;
            IIEmemoryBankSwitchedRAM1 = new byte[0x2000];
            IIEmemoryBankSwitchedRAM2_1 = new byte[0x1000];
            IIEmemoryBankSwitchedRAM2_2 = new byte[0x1000];
            IIEAuxBankSwitchedRAM1 = new byte[IIEAuxBanks, 0x2000];
            IIEAuxBankSwitchedRAM2_1 = new byte[IIEAuxBanks, 0x1000];
            IIEAuxBankSwitchedRAM2_2 = new byte[IIEAuxBanks, 0x1000];
            auxRAM = new byte[IIEAuxBanks, 0xc000];
            auxZP = new byte[IIEAuxBanks,0x200];
            charSet = new Dictionary<byte, bool[,]>();
            altCharSet = new Dictionary<byte, bool[,]>();
            baseRAM = new byte[0xc000];
            baseZP =  new byte[0x200];
        }

        public void ClearBaseRAM()
        {
            Random rnd = new Random();
            byte[] b = new byte[0xbfff];
            rnd.NextBytes(b);
            for (ushort j = 0; j < IIEAuxBanks; j++)
            {
                for (ushort i = 0; i < b.Length; i++)
                {
                    if (j == 0)
                        baseRAM[i] = b[i];
                    auxRAM[j,i] = b[i];
                }
            }
            IIEmemoryBankSwitchedRAM1 = new byte[0x2000];
            IIEmemoryBankSwitchedRAM2_1 = new byte[0x1000];
            IIEmemoryBankSwitchedRAM2_2 = new byte[0x1000];
            softswitches.IIeExpansionCardBank = 0;
            IIEAuxBankSwitchedRAM1 = new byte[IIEAuxBanks, 0x2000];
            IIEAuxBankSwitchedRAM2_1 = new byte[IIEAuxBanks, 0x1000];
            IIEAuxBankSwitchedRAM2_2 = new byte[IIEAuxBanks, 0x1000];
            baseZP = new byte[0x200];
            auxZP = new byte[IIEAuxBanks,0x200];
        }

        public void LoadChars(byte[] rom)
        {
            charSet = new Dictionary<byte, bool[,]>();
            byte[] chars = new byte[] {

                                    0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xcb, 0xcc, 0xcd, 0xce, 0xcf,
                                    0xd0, 0xd1, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xdb, 0xdc, 0xdd, 0xde, 0xdf,
                                    0xe0, 0xe1, 0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec, 0xed, 0xee, 0xef,
                                    0xf0, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 0xfa, 0xfb, 0xfc, 0xfd, 0xfe, 0xff,

                                    0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f,
                                    0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c, 0x5d, 0x5e, 0x5f,
                                    0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f,
                                    0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f,

                                    0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x8b, 0x8c, 0x8d, 0x8e, 0x8f,
                                    0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0x9b, 0x9c, 0x9d, 0x9e, 0x9f,
                                    0xa0, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 0xaa, 0xab, 0xac, 0xad, 0xae, 0xaf,
                                    0xb0, 0xb1, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb, 0xbc, 0xbd, 0xbe, 0xbf,

                                    0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f,
                                    0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f,
                                    0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f,
                                    0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f,

                                    //0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f,
                                    //0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f,
                                    //0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f,
                                    //0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f,

                                    //0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f,
                                    //0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c, 0x5d, 0x5e, 0x5f,
                                    //0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f,
                                    //0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f,

                                    //0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x8b, 0x8c, 0x8d, 0x8e, 0x8f,
                                    //0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0x9b, 0x9c, 0x9d, 0x9e, 0x9f,
                                    //0xa0, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 0xaa, 0xab, 0xac, 0xad, 0xae, 0xaf,
                                    //0xb0, 0xb1, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb, 0xbc, 0xbd, 0xbe, 0xbf,

                                    //0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xcb, 0xcc, 0xcd, 0xce, 0xcf,
                                    //0xd0, 0xd1, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xdb, 0xdc, 0xdd, 0xde, 0xdf,
                                    //0xe0, 0xe1, 0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec, 0xed, 0xee, 0xef,
                                    //0xf0, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 0xfa, 0xfb, 0xfc, 0xfd, 0xfe, 0xff,

                                     };


            ushort id = 0;
            foreach (var item in chars)
            {

                bool[,] charboolItem = new bool[8, 7];

                for (int charLayer = 0; charLayer < 8; charLayer++)
                {
                    byte charItem = rom[id];
                    bool[] bitsLayer = Tools.ConvertByteToBoolArray(charItem);
                    for (int charBits = 1; charBits < 8; charBits++)
                    {
                        if (bitsLayer[0])
                            charboolItem[charLayer, charBits - 1] = !bitsLayer[charBits];
                        else
                            charboolItem[charLayer, charBits - 1] = bitsLayer[charBits];
                    }
                    id++;
                }

                charSet.Add(item, charboolItem);
            }
        }

        public void LoadIIeChars(byte[] rom)
        {
            charSet = new Dictionary<byte, bool[,]>();
            altCharSet = new Dictionary<byte, bool[,]>();
            ushort id = 0;
            for (int i = 0; i <= 0xff; i++)
            {
                bool[,] charboolItem = new bool[8, 7];
                for (int charLayer = 0; charLayer < 8; charLayer++)
                {
                    byte charItem = rom[id];
                    bool[] bitsLayer = Tools.ConvertByteToBoolArray(charItem, false);
                    for (int charBits = 1; charBits < 8; charBits++)
                    {
                        charboolItem[charLayer, charBits - 1] = !bitsLayer[charBits];
                    }
                    id++;
                }
                charSet.Add((byte)i, charboolItem);
            }
            for (int i = 0; i <= 0xff; i++)
            {
                bool[,] charboolItem = new bool[8, 7];
                for (int charLayer = 0; charLayer < 8; charLayer++)
                {
                    byte charItem = rom[id];
                    bool[] bitsLayer = Tools.ConvertByteToBoolArray(charItem, false);
                    for (int charBits = 1; charBits < 8; charBits++)
                    {
                        charboolItem[charLayer, charBits - 1] = !bitsLayer[charBits];
                    }
                    id++;
                }
                altCharSet.Add((byte)i, charboolItem);
            }
        }
        public void LoadROM(ushort startAddress, byte[] rom)
        {
            for (int i = 0; i < rom.Length; i++)
            {
                ROM[startAddress - 0xd000 + i] = rom[i];
            }
        }

        public void LoadAppleIIeInternalROM(ushort startAddress, byte[] rom)
        {
            for (int i = 0; i < 0x1000; i++)
            {
                AppleIIeInternalROM[startAddress - 0xc000 + i] = rom[i];
            }
            for (int i = 0x1000; i < rom.Length; i++)
            {
                ROM[startAddress - 0xd000 + i] = rom[i];
            }
        }


        public byte ReadByte(ushort address)
        {
            byte ret = 0;
            if (address < 0x0200)
            {
                if (appleIIe)
                {
                    if (softswitches.AltZPOn_Off)
                        ret = auxZP[softswitches.IIeExpansionCardBank,address];
                    else
                        ret = baseZP[address];
                }
                else
                    ret = baseZP[address];
            }
            else if (address < 0xc000)
            {
                if (appleIIe)
                {
                    
                    if (!softswitches.Store80On_Off)
                    {
                        if (softswitches.RAMReadOn_Off)
                            ret = auxRAM[softswitches.IIeExpansionCardBank,address];
                        else
                            ret = baseRAM[address];
                    }
                    else // softswitches.Store80On_Off
                    {
                        if (address >= 0x0400 && address < 0x0800)
                        {
                            if (!softswitches.Page1_Page2) // Page 2
                                ret = auxRAM[0,address];
                            else                           // Page 1
                                ret = baseRAM[address];
                        } // Text Pages
                        else if (address >= 0x2000 && address < 0x4000) // Graphics Pages
                        {
                            if (softswitches.LoRes_HiRes)
                            {
                                if (softswitches.RAMReadOn_Off)
                                    ret = auxRAM[softswitches.IIeExpansionCardBank,address];
                                else
                                    ret = baseRAM[address];
                            }
                            else
                            {
                                if (!softswitches.Page1_Page2) // Page 2
                                    ret = auxRAM[softswitches.IIeExpansionCardBank,address];
                                else                           // Page 1
                                    ret = baseRAM[address];
                            }
                        }
                        else
                        {
                            if (softswitches.RAMReadOn_Off)
                                ret = auxRAM[softswitches.IIeExpansionCardBank,address];
                            else
                                ret = baseRAM[address];
                        }
                    }
                
                }
                else
                {
                    ret = baseRAM[address];
                }
            }
            else if (address >= 0xc000 && address <= 0xc079)
            {
                ret = softswitches.Read(address, this);
            }
            else if (address >= 0xd000)
            {
                bool read = false;
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0 && appleIIe)
                    {
                        if (softswitches.IIEMemoryBankReadRAM_ROM)
                        {
                            if (address >= 0xd000 && address < 0xe000)
                            {
                                if (softswitches.IIEMemoryBankBankSelect1_2)
                                {
                                    if (softswitches.AltZPOn_Off)
                                        ret = IIEAuxBankSwitchedRAM2_1[softswitches.IIeExpansionCardBank, address - 0xd000];
                                    else
                                        ret = IIEmemoryBankSwitchedRAM2_1[address - 0xd000];
                                }
                                else
                                {
                                    if (softswitches.AltZPOn_Off)
                                        ret = IIEAuxBankSwitchedRAM2_2[softswitches.IIeExpansionCardBank, address - 0xd000];
                                    else
                                        ret = IIEmemoryBankSwitchedRAM2_2[address - 0xd000];
                                }
                            }
                            else if (address >= 0xd000)
                            {
                                if (softswitches.AltZPOn_Off)
                                    ret = IIEAuxBankSwitchedRAM1[softswitches.IIeExpansionCardBank, address - 0xe000];
                                else
                                    ret = IIEmemoryBankSwitchedRAM1[address - 0xe000];
                            }
                            read = true;
                            break;
                        }
                    }
                    else
                    {
                        if (slots[i] is IRamCard && ((IRamCard)slots[i]).MemoryBankReadRAM_ROM)
                        {
                            ret = slots[i].Read(address, this);
                            read = true;
                            break;
                        }
                    }
                }
                if (!read)
                {
                    ret = ROM[address - 0xd000];
                }

            }
            else if (address >= 0xc080 && address < 0xc100)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (i == 0 && appleIIe)
                    {
                        if (address >= 0xc080 + (0x10 * i) && address < 0xc080 + (0x10 * (i+1))) 
                            ret = softswitches.Read(address, this);
                    }
                    else
                    {
                        if (address >= 0xc080 + (0x10 * i) && address < 0xc080 + (0x10 * (i+1))) 
                            ret = slots[i].Read(address, this);    
                    }
                }
            }
            else if (softswitches.IntCXRomOn_Off)
            {
                if (address >= 0xc100 && address < 0xd000)
                {
                    ret = AppleIIeInternalROM[address - 0xc000];
                }
            }
            else
            {
                if (address >= 0xc800) // Extended ROM Area
                {
                    if (softswitches.IntC8RomOn_Off)
                    {
                        ret = AppleIIeInternalROM[address - 0xc000];
                    }
                    else
                    {
                        ret = slots[selectedC8Slot].Read(address, this);
                    }
                }
                else if (address >= 0xc100)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        if (i == 3)
                        {   
                            if (address >= 0xc000 + (0x100 * i) && address < 0xc000 + (0x100 * (i+1)))
                            {
                                if (slots[i].Empty && !softswitches.SlotC3RomOn_Off)
                                {
                                    ret = AppleIIeInternalROM[address - 0xc000];
                                    softswitches.IntC8RomOn_Off = true;
                                }
                                else 
                                {
                                    ret = slots[i].C000ROM[address - (0xc000 + (0x100 * i))];
                                    softswitches.IntC8RomOn_Off = false;
                                    selectedC8Slot = i;
                                }
                            }
                        }
                        else
                        {
                            if (address >= 0xc000 + (0x100 * i) && address < 0xc000 + (0x100 * (i+1)))
                            {
                                ret = slots[i].C000ROM[address - (0xc000 + (0x100 * i))];
                                softswitches.IntC8RomOn_Off = false;
                                selectedC8Slot = i;
                            }
                        }
                    }
                }
            }
            
            return ret;
        }

        public void WriteByte(ushort address, byte value)
        {
            if (address < 0x0200)
            {
                if (appleIIe)
                {
                    if (softswitches.AltZPOn_Off)
                        auxZP[softswitches.IIeExpansionCardBank,address] = value;
                    else
                        baseZP[address] = value;
                }
                else
                    baseZP[address] = value;
            }
            else if (address < 0xc000)
            {
                if (appleIIe)
                {
                    if (!softswitches.Store80On_Off)
                    {
                        if (softswitches.RAMWriteOn_Off)
                            auxRAM[softswitches.IIeExpansionCardBank,address] = value;
                        else
                            baseRAM[address] = value;
                    }
                    else // softswitches.Store80On_Off
                    {
                        if (address >= 0x0400 && address < 0x0800) // Text Pages
                        {
                            if (!softswitches.Page1_Page2)
                                auxRAM[softswitches.IIeExpansionCardBank,address] = value;
                            else
                                baseRAM[address] = value;
                        }
                        else if (address >= 0x2000 && address < 0x4000) // Graphics Pages
                        {
                            if (softswitches.LoRes_HiRes)
                            {
                                if (softswitches.RAMWriteOn_Off)
                                    auxRAM[softswitches.IIeExpansionCardBank,address] = value;
                                else
                                    baseRAM[address] = value;
                            }
                            else
                            {
                                if (!softswitches.Page1_Page2) // Page 2
                                    auxRAM[softswitches.IIeExpansionCardBank,address] = value;
                                else                           // Page 1
                                    baseRAM[address] = value;
                            }

                        }
                        else
                        {
                            if (softswitches.RAMWriteOn_Off)
                                auxRAM[softswitches.IIeExpansionCardBank,address] = value;
                            else
                                baseRAM[address] = value;
                        }
                    }
                    
                }
                else
                {
                    baseRAM[address] = value;
                }
            }
            else if (address >= 0xc000 && address <0xc079)
            {
                softswitches.Write(address, value, this);
            }
            else if (address >= 0xd000)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0 && appleIIe)
                    {
                        if (address >= 0xd000 && address < 0xe000)
                        {
                            if (softswitches.IIEMemoryBankBankSelect1_2)
                            {
                                if (softswitches.AltZPOn_Off)
                                    IIEAuxBankSwitchedRAM2_1[softswitches.IIeExpansionCardBank, address - 0xd000] = value;
                                else
                                    IIEmemoryBankSwitchedRAM2_1[address - 0xd000] = value;
                            }
                            else
                            {
                                if (softswitches.AltZPOn_Off)
                                    IIEAuxBankSwitchedRAM2_2[softswitches.IIeExpansionCardBank, address - 0xd000] = value;
                                else
                                    IIEmemoryBankSwitchedRAM2_2[address - 0xd000] = value;
                            }
                        }
                        else
                        {
                            if (softswitches.AltZPOn_Off)
                                IIEAuxBankSwitchedRAM1[softswitches.IIeExpansionCardBank, address - 0xe000] = value;
                            else
                                IIEmemoryBankSwitchedRAM1[address - 0xe000] = value;
                        }
                    }
                    else
                    {
                        if (slots[i] is IRamCard && ((IRamCard)slots[i]).MemoryBankWriteRAM_NoWrite)
                        {
                            slots[i].Write(address, value, this);
                        }
                    }
                }
            }
            else if (address >= 0xc800) // Slots reserved ROM 
            {
                slots[selectedC8Slot].Write(address, value, this); 
            }
            else if (address >= 0xc080) // Slots SoftSwitches
            {
                
                for (int i = 0; i < 8; i++)
                {
                    if (i == 0 && appleIIe)
                    {
                        if (address >= 0xc080 + (0x10 * i) && address < 0xc080 + (0x10 * (i+1))) 
                            softswitches.Write(address, value, this);
                    }
                    else
                    {
                        if (address >= 0xc080 + (0x10 * i) && address < 0xc080 + (0x10 * (i+1))) 
                            slots[i].Write(address, value, this);
                    }
                }
            }
        }

        public ushort? ReadAddressLLHH(ushort? address)
        {
            return (ushort)(ReadByte((ushort)(address.Value + 1)) << 8 | ReadByte(address.Value));
        }

        public byte? ReadZeroPageAddress(ushort? address)
        {
            return (byte?)ReadByte(address.Value);
        }

        public ushort GetIRQVector()
        {
            var bytes = new byte[]
            {
            ReadByte(0xFFFE),
            ReadByte(0xFFFE + 1)
            };

            return BitConverter.ToUInt16(bytes);
        }

        public byte[] MemoryDump(ushort startAddress, ushort endAddress)
        {
            byte[] ret = new byte[endAddress - startAddress];
            for (ushort i = startAddress; i < endAddress; i++)
            {
                ret[i - startAddress] = ReadByte(i);
            }
            return ret;
        }

    }
}