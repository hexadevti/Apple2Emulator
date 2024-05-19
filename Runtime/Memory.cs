using Runtime.Abstractions;
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

    public bool UpdateScreen { get; set;}
    public Memory(ushort size)
    {
        overlays = new List<IOverLay>();
        for (ushort i = 0; i < size;i++)
        {
            memory[i] = (byte)0;
        }
        memory[size] = (byte)0;
    }

    public void LoadChars(byte[] rom)
    {
        byte[] chars = new byte[] { 0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xcb, 0xcc, 0xcd, 0xce, 0xcf,
                                    0xd0, 0xd1, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xdb, 0xdc, 0xdd, 0xde, 0xdf,
                                    0xa0, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 0xaa, 0xab, 0xac, 0xad, 0xae, 0xaf,
                                    0xb0, 0xb1, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb, 0xbc, 0xbd, 0xbe, 0xbf,
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
                    charboolItem[charLayer, charBits-1] = bitsLayer[charBits];
                }
                id++;
            }

            charSet.Add(item, charboolItem);

            
        }

    }

    private static bool[] ConvertByteToBoolArray(byte b)
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
