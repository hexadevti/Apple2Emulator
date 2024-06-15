using System.Reflection;
using System.Text;
using Runtime.Abstractions;

namespace Runtime.Overlays;

public class DiskIISlot6Ovl : IOverLay
{
    public byte[] InterfaceROM = new byte[0x100];
     
    public DiskIISlot6Ovl()
    {
        Start = 0xc600;
        End = 0xc6ff;

        string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (assemblyPath != null)
            assemblyPath += "/";

        var rom = File.ReadAllBytes(assemblyPath + "roms/diskinterface.rom");
        for (int i = 0; i < rom.Length; i++)
        {
            InterfaceROM[i] = rom[i];
        }
    }

    public int Start { get; }
    public int End { get; }
    public void Write(ushort address, byte b, Memory memory)
    {
        
    }

    public byte Read(ushort address, Memory memory, State state)
    {
        return InterfaceROM[address - 0xc600];
    }
}