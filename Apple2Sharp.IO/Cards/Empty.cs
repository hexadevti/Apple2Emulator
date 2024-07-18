
using System;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Apple2Sharp.Mainboard.Interfaces;

namespace Apple2Sharp.Mainboard.Cards
{
    public class EmptySlot : ICard
    {
        private int _slotNumber;
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


        public EmptySlot()
        {
            _c000ROM = new byte[0x100];
        }

        public void Write(ushort address, byte b, Apple2Board mainBoard)
        {
        }


        public byte Read(ushort address, Apple2Board mainBoard)
        {
            return 0;
        }


    }
}