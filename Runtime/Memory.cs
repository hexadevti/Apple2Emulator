using Runtime.Abstractions;

namespace Runtime;

public class Memory
{
    public Dictionary<ushort, byte> memory = new Dictionary<ushort, byte>();
    public const ushort _ResetVector = 65532;
    public const ushort _IRQVector = 0xFFFE;
    private const byte ZeroByte = (byte)0;
    private readonly IList<IOverLay> overlays;
    

    public Memory(ushort size)
    {
        overlays = new List<IOverLay>();
        for (ushort i = 0; i < size;i++)
        {
            memory[i] = (byte)0;
        }
        memory[size] = (byte)0;
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
            value = overLay.Read(address);
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
            overLay.Write(address, value);
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
