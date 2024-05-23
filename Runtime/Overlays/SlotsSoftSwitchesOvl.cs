
using System.Text;
using Runtime.Abstractions;

namespace Runtime.Overlays;

public class SlotsSoftSwitchesOvl : IOverLay
{
       
    public SlotsSoftSwitchesOvl()
    {
        Start = 0xc090;
        End = 0xc0ff;
    }

    public int Start { get; }
    public int End { get; }

    int slot = 6;
    int pointer = 0;

    byte track = 1;
    byte sector = 1;

    List<byte> selectedSector = new List<byte>();

    public void Write(ushort address, byte b, Memory memory)
    {
    }
    
    public byte Read(ushort address, Memory memory)
    {
        int slotOffset = slot * 0x10;    
        if (address == 0xc08c + slotOffset)
        {
            var newsector = memory.ReadByte(0x3d);
            var newtrack = memory.ReadByte(0x41); 
            if (newsector > 15)
                newsector = sector;
            if (sector != newsector || track != newtrack)
            {
                sector = newsector;
                track = newtrack;
                selectedSector = new List<byte>() { 0xff, 0xff, 0xff };
                selectedSector.AddRange(new List<byte>() { 0xd5, 0xaa, 0x96 }); // Prologe
                selectedSector.AddRange(memory.drive.EncodeByte(0x2f).ToList()); // Volume
                selectedSector.AddRange(memory.drive.EncodeByte(track).ToList()); // Track
                selectedSector.AddRange(memory.drive.EncodeByte(sector).ToList()); // Sector
                selectedSector.AddRange(memory.drive.Checksum(0x2f, track,sector).ToList()); // Checksum
                selectedSector.AddRange(new List<byte>() { 0xff, 0xff, 0xd5, 0xaa, 0xad }); // Epilogue
                selectedSector.AddRange(memory.drive.Encode6_2(track, sector));
                selectedSector.AddRange(new List<byte>() { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }); // Gap
                pointer=0;
            }
            if (pointer > selectedSector.Count-1)
                pointer=0;
            // Le Byte disco
            return selectedSector[pointer++];
        }
        
        return 0;
    }
}