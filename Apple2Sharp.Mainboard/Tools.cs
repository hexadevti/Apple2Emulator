using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.ExceptionServices;


namespace Apple2Sharp.Mainboard
{
    public static class Tools
    {
        public static byte[] LoadROM(byte[] rom, int offset = 0)
        {
            byte[] ret = new byte[0x100];
            for (int i = 0; i < 0xff; i++)
            {
                ret[i] = rom[i + offset];
            }
            return ret;
        }

        public static Dictionary<byte, bool[,]> Load80Chars(byte[] rom)
        {
            Dictionary<byte, bool[,]> ret = new Dictionary<byte, bool[,]>();

            ushort id = 0;
            for (byte i = 0; i < 0x80; i++)
            {
                bool[,] charboolItem = new bool[9, 8];

                for (int charLayer = 0; charLayer < 16; charLayer++)
                {
                    if (charLayer < 9)
                    {
                        byte charItem = rom[id];
                        bool[] bitsLayer = ConvertByteToBoolArray(charItem);
                        for (int charBits = 0; charBits < 8; charBits++)
                        {
                            charboolItem[charLayer, charBits] = bitsLayer[charBits];
                        }
                    }
                    id++;
                }

                ret.Add(i, charboolItem);
            }

            return ret;
        }

        public static bool[] ConvertByteToBoolArray(byte b, bool reverse = true)
        {
            // prepare the return result
            bool[] result = new bool[8];

            // check each bit in the byte. if 1 set to true, if 0 set to false
            for (int i = 0; i < 8; i++)
                result[i] = (b & (1 << i)) != 0;
            // reverse the array
            if (reverse)
                Array.Reverse(result);

            return result;
        }

        public static byte[] LoadExtendedSlotsROM(ushort startAddress, byte[] rom)
        {
            byte[] ret = new byte[0x800];
            for (int i = 0; i < rom.Length; i++)
            {
                ret[startAddress - 0xc800 + i] = rom[i];
            }
            return ret;

        }

        public static byte[] ImportStringHexData(string image, ushort size)
        {
            byte[] ret = new byte[size];
            for (int i = 0; i < size; i = i + 1)
            {
                ret[i] = byte.Parse(image.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return ret;
        }

        public static byte[] EmptyMemory(ushort size, byte? content = null)
        {
            byte[] ret = new byte[size];

            Random rnd = new Random();
            byte[] b = new byte[0xbfff];
            rnd.NextBytes(b);
            for (ushort i = 0; i < size; i++)
            {
                ret[i] = content != null ? content.Value : b[i];
            }
            return ret;
        }

    }
}