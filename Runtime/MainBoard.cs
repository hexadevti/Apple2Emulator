using Runtime.Abstractions;
using Runtime.Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Threading.Channels;

namespace Runtime
{
    public class MainBoard
    {
        public object displayLock = new object();

        //private readonly IList<IOverLay> overlays;
        public Dictionary<byte, bool[,]> charSet;

        public bool adjust1Mhz = true;
        public double clockSpeed = 0;
        public byte[] baseRAM = new byte[0xc000];

        public byte[] ROM = new byte[0x4000];
        public byte KeyPressed { get; set; }
        public Softswitches softswitches = new Softswitches();

        public State state { get; set; }
        public Queue<byte[]> clickBuffer = new Queue<byte[]>(100);
        public int cpuCycles { get; set; }

        public ICard slot0;
        public ICard slot1;
        public ICard slot2;
        public ICard slot3;
        public ICard slot4;
        public ICard slot5;
        public ICard slot6;
        public ICard slot7;
        CpuSoftswitches cpuSoftswitches = new CpuSoftswitches();

        public Queue<string> screenLog = new Queue<string>();
        public Queue<bool> cycleWait = new Queue<bool>();
        public int audioJumpInterval = 25;

        public bool videoColor = true;

        public int audioBuffer { get; set; }

        public int SlotRamEnable = 0;


        public MainBoard(State state)
        {
            //overlays = new List<IOverLay>();
            this.state = state;
        }

        public void ClearBaseRAM()
        {
            Random rnd = new Random();
            byte[] b = new byte[0xbfff];
            rnd.NextBytes(b);
            for (ushort i = 0; i < b.Length; i++)
            {
                baseRAM[i] = b[i];
            }
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

        public void LoadROM(ushort startAddress, byte[] rom)
        {
            for (int i = 0; i < rom.Length; i++)
            {
                ROM[startAddress - 0xd000 + i] = rom[i];
            }
        }



        public byte ReadByte(ushort address)
        {
            byte ret = 0;
            if (address < 0xc000)
            {
                ret = baseRAM[address];
            }
            else if (address >= 0xd000)
            {
                if (slot0 is IRamCard && ((IRamCard)slot0).MemoryBankReadRAM_ROM)
                {
                    ret = slot0.Read(address, this, state);
                }
                else if (slot1 is IRamCard && ((IRamCard)slot1).MemoryBankReadRAM_ROM)
                {
                    ret = slot1.Read(address, this, state);
                }
                else if (slot2 is IRamCard && ((IRamCard)slot2).MemoryBankReadRAM_ROM)
                {
                    ret = slot2.Read(address, this, state);
                }
                else if (slot3 is IRamCard && ((IRamCard)slot3).MemoryBankReadRAM_ROM)
                {
                    ret = slot3.Read(address, this, state);
                }
                else if (slot4 is IRamCard && ((IRamCard)slot4).MemoryBankReadRAM_ROM)
                {
                    ret = slot4.Read(address, this, state);
                }
                else
                {
                    ret = ROM[address - 0xd000];
                }

            }
            else if (address >= 0xc800) // Extended ROM Area
            {

                //TODO: Put condition to redirect to correct slot
                ret = slot3.Read(address, this, state);
            }
            else if (address >= 0xc700)
            {
                ret = slot7.C000ROM[address - 0xc700];
            }
            else if (address >= 0xc600)
            {
                ret = slot6.C000ROM[address - 0xc600];
            }
            else if (address >= 0xc500)
            {
                ret = slot5.C000ROM[address - 0xc500];
            }
            else if (address >= 0xc400)
            {
                ret = slot4.C000ROM[address - 0xc400];
            }
            else if (address >= 0xc300)
            {
                ret = slot3.C000ROM[address - 0xc300];
            }
            else if (address >= 0xc200)
            {
                ret = slot2.C000ROM[address - 0xc200];
            }
            else if (address >= 0xc100)
            {
                ret = slot1.C000ROM[address - 0xc100];
            }
            else if (address >= 0xc080)
            {
                if (address >= 0xc0f0) // Slot 7
                    ret = slot7.Read(address, this, state);
                else if (address >= 0xc0e0) // Slot 6
                    ret = slot6.Read(address, this, state);
                else if (address >= 0xc0d0) // Slot 5
                    ret = slot5.Read(address, this, state);
                else if (address >= 0xc0c0) // Slot 4
                    ret = slot4.Read(address, this, state);
                else if (address >= 0xc0b0) // Slot 3
                    ret = slot3.Read(address, this, state);
                else if (address >= 0xc0a0) // Slot 2
                    ret = slot2.Read(address, this, state);
                else if (address >= 0xc090) // Slot 1
                    ret = slot1.Read(address, this, state);
                else if (address >= 0xc080) // Slot 0
                    ret = slot0.Read(address, this, state);
            }
            else if (address >= 0xc000)
            {
                ret = cpuSoftswitches.Read(address, this, state);
            }

            return ret;
        }


        public void WriteByte(ushort address, byte value)
        {
            if (address < 0xc000)
            {
                baseRAM[address] = value;
            }
            else if (address >= 0xd000)
            {
                if (slot0 is IRamCard && ((IRamCard)slot0).MemoryBankReadRAM_ROM && ((IRamCard)slot0).MemoryBankWriteRAM_NoWrite)
                {
                    slot0.Write(address, value, this);
                }
                else if (slot1 is IRamCard && ((IRamCard)slot1).MemoryBankReadRAM_ROM && ((IRamCard)slot1).MemoryBankWriteRAM_NoWrite)
                {
                    slot1.Write(address, value, this);
                }
                else if (slot2 is IRamCard && ((IRamCard)slot2).MemoryBankReadRAM_ROM && ((IRamCard)slot2).MemoryBankWriteRAM_NoWrite)
                {
                    slot2.Write(address, value, this);
                }
                else if (slot3 is IRamCard && ((IRamCard)slot3).MemoryBankReadRAM_ROM && ((IRamCard)slot3).MemoryBankWriteRAM_NoWrite)
                {
                    slot3.Write(address, value, this);
                }
                else if (slot4 is IRamCard && ((IRamCard)slot4).MemoryBankReadRAM_ROM && ((IRamCard)slot4).MemoryBankWriteRAM_NoWrite)
                {
                    slot4.Write(address, value, this);
                }
            }
            else if (address >= 0xc800) // Slots reserved ROM 
            {
                //TODO: Condition to select right slot
                slot3.Write(address, value, this); //cols80RAM[address - 0xcc00 + softswitches.cols80PageSelect * 0x200] = value;
            }
            else if (address >= 0xc080) // Slots SoftSwitches
            {
                if (address >= 0xc0f0) // Slot 7
                    slot7.Write(address, value, this);
                else if (address >= 0xc0e0) // Slot 6
                    slot6.Write(address, value, this);
                else if (address >= 0xc0d0) // Slot 5
                    slot5.Write(address, value, this);
                else if (address >= 0xc0c0) // Slot 4
                    slot6.Write(address, value, this);
                else if (address >= 0xc0b0) // Slot 3
                    slot3.Write(address, value, this);
                else if (address >= 0xc0a0) // Slot 2
                    slot2.Write(address, value, this);
                else if (address >= 0xc090) // Slot 1
                    slot1.Write(address, value, this);
                else if (address >= 0xc080) // Slot 0
                    slot0.Write(address, value, this);
            }
            else if (address >= 0xc000)
            {
                cpuSoftswitches.Write(address, value, this);
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