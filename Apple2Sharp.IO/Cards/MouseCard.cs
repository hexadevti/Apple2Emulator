using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Apple2Sharp.Mainboard.Interfaces;
using System.Drawing.Imaging;
using System.Drawing;

using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

namespace Apple2Sharp.Mainboard.Cards
{
    public class MouseCard : ICard
    {
        private int _slotNumber = 4;
        public bool Empty { get { return false; } }
        private byte[] _c000ROM;
        private byte[] _cc00ROM;
        public byte[] extendedSlotsROM = new byte[0x800];
        public bool lastMouseButton = false;
        public int SlotNumber
        {
            get { return _slotNumber; }
            set { _slotNumber = value; }
        }

        public byte[] C000ROM
        {
            get { return _c000ROM; }
        }

        public byte[] CC00ROM
        {
            get { return _cc00ROM; }
            set { _cc00ROM = value; }
        }

        public MouseCard(int slotNumber, byte[] c000ROM, Apple2Board apple2Board)
        {
            _slotNumber = slotNumber;
            _c000ROM = c000ROM;
            apple2Board.baseRAM[0x778 + slotNumber] = 0;
//            _cc00ROM = cc00ROM;
        }
        public void Write(ushort address, byte b, Apple2Board mainBoard)
        {
            ProcessC0xx(address, b, mainBoard, false);
        }

        public byte Read(ushort address, Apple2Board mainBoard)
        {
            return ProcessC0xx(address, 0, mainBoard, true);
        }

        private byte ProcessC0xx(ushort address, byte b, Apple2Board mainBoard, bool Read_Write)
        {
            if (address == 0xc080 + _slotNumber * 0x10)
            {
            }
            else if (address == 0xc081 + _slotNumber * 0x10)
            {
            }
            else if (address == 0xc082 + _slotNumber * 0x10)
            {
                if (Read_Write)
                {
                    mainBoard.baseRAM[0x478 + _slotNumber] = mainBoard.mouseXLo;
                    mainBoard.baseRAM[0x578 + _slotNumber] = mainBoard.mouseXHi;
                    mainBoard.baseRAM[0x4f8 + _slotNumber] = mainBoard.mouseYLo;
                    mainBoard.baseRAM[0x5f8 + _slotNumber] = mainBoard.mouseYHi;

                    byte actualMouseButton = (byte)((mainBoard.mouseButton ? 0x80 : 0x00) + (lastMouseButton ? 0x40 : 0x00));
                    lastMouseButton = mainBoard.mouseButton;
                    mainBoard.baseRAM[0x778 + _slotNumber] = (byte)actualMouseButton;
                }
            }
            else if (address == 0xc083 + _slotNumber * 0x10)
            {
                //if (Read_Write)
                //    return 63;
            }
            else if (address == 0xc084 + _slotNumber * 0x10)
            {
                //if (Read_Write)
                //    return 63;
            }
            else if (address == 0xc085 + _slotNumber * 0x10)
            {
                //if (Read_Write)
                //    return 63;
            }
            else if (address == 0xc086 + _slotNumber * 0x10)
            {
                //if (Read_Write)
                //    return 63;
            }
            else if (address == 0xc087 + _slotNumber * 0x10)
            {
                //if (Read_Write)
                //    return 63;
            } 
            else if (address == 0xc08a + _slotNumber * 0x10)
            {
                //if (Read_Write)
                //    return 63;
            }

            return 0;
        }
    }
}