
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography;
using System.Text;
using Apple2Sharp.Mainboard.Interfaces;

namespace Apple2Sharp.Mainboard.Cards
{
    public class HdCard : ICard
    {
        public bool Empty { get { return false; } }
        private int pointer = 0;
        private int _slotNumber = 7;
        private byte[] _c000ROM;
        public DiskDrive drive1 { get; set; }

        public bool Drive1_2 { get; set; }

        public bool DriveQ6H_L { get; set; }
        public bool DriveQ7H_L { get; set; }
        public bool DriveMotorON_OFF { get; set; }
        public byte Command { get; set; }
        public byte Status { get; set; }
        public byte UnitNumber { get; set; }
        public ushort MemoryBuffer { get; set; }
        public ushort BlockNumber { get; set; }
        public ushort DiskImageSize { get; set; }

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

        Dictionary<string, List<byte>> output = new Dictionary<string, List<byte>>();
        public HdCard(int slotNumber, byte[] c000ROM, string disk1, string disk2)
        {
            _slotNumber = slotNumber;
            _c000ROM = c000ROM;
            this.drive1 = new DiskDrive(disk1);
        }

        public void Write(ushort address, byte b, Apple2Board mainBoard)
        {
            ProcessSwitchc080(address, b, mainBoard, false);
        }

        public byte Read(ushort address, Apple2Board mainBoard)
        {
            return ProcessSwitchc080(address, 0, mainBoard, true);
        }

        private byte ProcessSwitchc080(ushort address, byte b, Apple2Board mainBoard, bool Read_Write)
        {

            if (address == 0xc080 + _slotNumber * 0x10)
            {
                switch (Command)
                {
                    case 0x01:
                        Status = 0xb7;
                        Status = LoadBlock(MemoryBuffer, BlockNumber, mainBoard);
                        return Status;
                    default:
                        break;
                }
            }
            else if (address == 0xc081 + _slotNumber * 0x10)
            {
                return Status; //0xb7 - busy; 0 - ok?
            }
            else if (address == 0xc082 + _slotNumber * 0x10)
            {
                if (Read_Write)
                {
                    return Command;
                }
                else
                {
                    Command = b;
                }
            }
            else if (address == 0xc083 + _slotNumber * 0x10)
            {
                if (Read_Write)
                {
                    return UnitNumber;
                }
                else
                {
                    UnitNumber = b;
                }
            }
            else if (address == 0xc084 + _slotNumber * 0x10)
            {
                if (Read_Write)
                {
                    return (byte)(MemoryBuffer & 0x00ff);
                }
                else
                {
                    MemoryBuffer = (ushort)(MemoryBuffer & 0xff00 | b);
                }
            }
            else if (address == 0xc085 + _slotNumber * 0x10)
            {
                if (Read_Write)
                {
                    return (byte)(MemoryBuffer & 0xff00);
                }
                else
                {
                    MemoryBuffer = (ushort)(MemoryBuffer & 0x00ff | b << 8);
                }
            }
            else if (address == 0xc086 + _slotNumber * 0x10)
            {
                if (Read_Write)
                {
                    return (byte)(BlockNumber & 0x00ff);
                }
                else
                {
                    BlockNumber = (ushort)(BlockNumber & 0xff00 | b);
                }
            }
            else if (address == 0xc087 + _slotNumber * 0x10)
            {
                if (Read_Write)
                {
                    return (byte)(BlockNumber & 0xff00);
                }
                else
                {
                    BlockNumber = (ushort)(BlockNumber & 0x00ff | b << 8);
                }
            }
            else if (address == 0xc088 + _slotNumber * 0x10)
            {
                // next byte
            }
            else if (address == 0xc089 + _slotNumber * 0x10)
            {
                return (byte)(this.drive1.GetBlockQty() & 0x00ff);
            }
            else if (address == 0xc08a + _slotNumber * 0x10)
            {
                return (byte)(this.drive1.GetBlockQty() & 0xff00);
            }


            return 0;
        }
    
        private byte LoadBlock(ushort address, ushort block, Apple2Board mainBoard)
        {
            int i = 0;
            try
            {
                foreach (byte b in this.drive1.GetBlockData(block))
                {
                    mainBoard.WriteByte((ushort)(address + i), b);
                    i++;
                }
                return 0;
            }
            catch
            {
                return 0xb0;
            }
        }
        
    }
}