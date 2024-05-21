using Runtime.Abstractions;
using Runtime.Overlays;
using System.Globalization;
using System.Runtime.ExceptionServices;

namespace Runtime;

public class Memory
{
    public Dictionary<ushort, byte> memory = new Dictionary<ushort, byte>();
    public const ushort _ResetVector = 65532;
    public const ushort _IRQVector = 0xFFFE;
    private const byte ZeroByte = (byte)0;
    private readonly IList<IOverLay> overlays;
    public Dictionary<byte, bool[,]> charSet = new Dictionary<byte, bool[,]>();

    public byte KeyPressed { get; set; }

    public Softswitches softswitches { get; set; }



    public bool UpdateScreen { get; set;}
    public Memory(ushort size)
    {
        overlays = new List<IOverLay>();
        Random rnd = new Random();
         byte[] b = new byte[size];
         rnd.NextBytes(b);
        for (ushort i = 0; i < size;i++)
        {
            //memory[i] = b[i];
            memory[i] = 0x00;
        }
    }

    public void LoadChars(byte[] rom)
    {
        byte[] chars = new byte[] { 
                                    0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xcb, 0xcc, 0xcd, 0xce, 0xcf,
                                    0xd0, 0xd1, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xdb, 0xdc, 0xdd, 0xde, 0xdf,

                                    0xa0, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 0xaa, 0xab, 0xac, 0xad, 0xae, 0xaf,
                                    0xb0, 0xb1, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb, 0xbc, 0xbd, 0xbe, 0xbf,

                                    0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f,
                                    0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c, 0x5d, 0x5e, 0x5f,

                                    0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f,
                                    0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f,


                                    0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x8b, 0x8c, 0x8d, 0x8e, 0x8f,
                                    0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0x9b, 0x9c, 0x9d, 0x9e, 0x9f,
                                   
                                    0xe0, 0xe1, 0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec, 0xed, 0xee, 0xef,
                                    0xf0, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 0xfa, 0xfb, 0xfc, 0xfd, 0xfe, 0xff,

                                    0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f,
                                    0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f,

                                    0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f,
                                    0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f,

                                     };
        ushort id = 0;
        foreach (var item in chars)
        {
            
            bool[,] charboolItem = new bool[8,7];

            for (int charLayer = 0; charLayer < 8; charLayer++)
            {
                byte charItem = rom[id];
                bool[] bitsLayer = ConvertByteToBoolArray(charItem);
                for (int charBits = 1; charBits<8;  charBits++) 
                {
                    if (bitsLayer[0])
                        charboolItem[charLayer, charBits-1] = !bitsLayer[charBits];
                    else
                        charboolItem[charLayer, charBits-1] = bitsLayer[charBits];
                }
                id++;
            }

            charSet.Add(item, charboolItem);

            
        }

    }

    public void ImportImage(string image, ushort address)
    {
        for (int i = 0; i < 0x1fff; i = i + 1)
        {
            memory[(ushort)(address + i)] = byte.Parse(image.Substring(i*2, 2), NumberStyles.HexNumber);
        }
    }

    public bool[] ConvertByteToBoolArray(byte b)
    {
        // prepare the return result
        bool[] result = new bool[8];

        // check each bit in the byte. if 1 set to true, if 0 set to false
        for (int i = 0; i < 8; i++)
            result[i] = (b & (1 << i)) != 0;

        // reverse the array
        Array.Reverse(result);

        return result;
    }

    private IOverLay? GetOverlay(ushort address) => 
        overlays.FirstOrDefault(x => x.Start <= address && x.End >= address);

    public void RegisterOverlay(IOverLay overlay)
    {
        overlays.Add(overlay);
    }

    public byte ReadByte(ushort address)
    {
        var overLay = GetOverlay(address);
        byte value;
        if (overLay != null)
            value = overLay.Read(address, this);
        else
            value = memory[address];
        return value;
    }

    public ushort? ReadAddressLLHH(ushort? address)
    {
        if (address!=null)
            return (ushort)(memory[(ushort)(address.Value+1)] << 8 | memory[address.Value]);
        else 
            return null;
    }
    
    private static ushort ProcessIndex(ushort address, State processorState, string[] addressMode)
    {
        if (addressMode.Length <= 1) return address;

        return addressMode[1] switch
        {
            "X" => (ushort)(address + processorState.X),
            "Y" => (ushort)(address + processorState.Y),
            _ => address
        };
    }
    public ushort? ReadAddressHHLL(ushort? address)
    {
        if (address!=null)
        {
            return (ushort)(memory[address.Value] << 8 | memory[(ushort)(address.Value+1)]);
        }   
        else 
            return null;
    }

    public ushort? ReadZeroPageAddress(ushort? address)
    {
        if (address.HasValue)
            return (ushort?)memory[address.Value];
        else 
            return null;
    }

    public void WriteAt(ushort i, byte[] code)
    {
        foreach (var b in code)
        {
            Write(i, b);
            i++;
        }
    }
    
    public void Write(ushort address, byte value)
    {
        var overLay = GetOverlay(address);
        if (overLay != null)
            overLay.Write(address, value, this);
        memory[address] = value;
    }

    public void SetResetVector(ushort wordAddress)
    {
        var bytes = BitConverter.GetBytes(wordAddress);
        memory[_ResetVector] = bytes[0];
        memory[_ResetVector+1] = bytes[1];
    }

    public void SetIRQVector(ushort wordAddress)
    {
        var bytes = BitConverter.GetBytes(wordAddress);
        memory[_IRQVector] = bytes[0];
        memory[_IRQVector + 1] = bytes[1];
    }

    public ushort GetResetVector()
    {
        var bytes = new byte[]
        {
            memory.ContainsKey(_ResetVector) ? memory[_ResetVector] : ZeroByte,
            memory.ContainsKey(_ResetVector + 1) ? memory[_ResetVector + 1] : ZeroByte
        };

        return BitConverter.ToUInt16(bytes);
    }

    public ushort GetIRQVector()
    {
        var bytes = new byte[]
        {
            memory.ContainsKey(_IRQVector) ? memory[_IRQVector] : ZeroByte,
            memory.ContainsKey(_IRQVector + 1) ? memory[_IRQVector + 1] : ZeroByte
        };

        return BitConverter.ToUInt16(bytes);
    }

}

public class Softswitches
{   
    public bool Graphics_Text { get; set; }
    public bool TextPage1_Page2 { get; set; }
    public bool DisplayFull_Split { get; set;}
    public bool LoRes_HiRes { get; set; }
    // public bool C050 { get; set; } // Display Graphics
    // public bool C051 { get; set; } // Display Text
    // public bool C052 { get; set; } // Display Full Screen
    // public bool C053 { get; set; } // Display Split Screen
    // public bool C054 { get; set; } // Display Page 1
    // public bool C055 { get; set; } // If 80STORE Off: Display Page 2, If 80STORE On: Read/Write Aux Display Mem
    // public bool C056 { get; set; } // Display LoRes Graphics
    // public bool C057 { get; set; } // Display HiRes Graphics

}
