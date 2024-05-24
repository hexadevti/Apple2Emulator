
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
    List<byte> selectedTrack = new List<byte>();

    int trackSize = 5856;
    byte[] rawdisktrack = new byte[5856];

    public void Write(ushort address, byte b, Memory memory)
    {
    }

    public byte Read(ushort address, Memory memory, State state)
    {
        int slotOffset = slot * 0x10;
        if (address == 0xc08c + slotOffset)
        {
            if (memory.softswitches.DriveQ6H_L == false || memory.softswitches.DriveQ7H_L == false)
            {
                byte newtrack = 0;
                if (state.PC > 0xc000 + slot * 0x100 && state.PC < (0xc000 + slot  * 0x100) + 0x100)
                {
                    newtrack = 0; 
                }
                else
                {
                    newtrack = memory.ReadByte(0x478);
                }

                if ((int)newtrack > 34)
                {
                    newtrack = 0; 
                }

                if (track != newtrack)
                {
                    selectedTrack = new List<byte>();
                    track = newtrack;
                    foreach (byte isec in new byte[] { 0xa, 0xb, 0xc, 0xd, 0xe, 0xf, 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9})
                    {
                        var sec = (byte)isec; // memory.drive.translateDos33Track[isec];
                        List<byte> b = new List<byte>();
                        // sector = newsector;
                        Console.WriteLine("Track: " + track  + " Sector: " + sec);
                        selectedSector = new List<byte>() { 0xff, 0xff, 0xff };
                        selectedSector.AddRange(new List<byte>() { 0xd5, 0xaa, 0x96 }); // Prologe address
                        b = memory.drive.EncodeByte(0x2f).ToList();
                        Console.WriteLine("Volume: " + Print(b) + " 2F");
                        selectedSector.AddRange(b); // Volume
                        b = memory.drive.EncodeByte(track).ToList();
                        Console.WriteLine("Track: " + Print(b) + "" + track);
                        selectedSector.AddRange(b); // Track
                        b = memory.drive.EncodeByte(sec).ToList();
                        Console.WriteLine("Sector: " + Print(b) + "" + sec);
                        selectedSector.AddRange(b); // Sector
                        b = memory.drive.Checksum(0x2f, track, sec).ToList();
                        Console.WriteLine("Checksum: " + Print(b));
                        selectedSector.AddRange(b); // Checksum
                        selectedSector.AddRange(new List<byte>() { 0xde, 0xaa, 0xeb }); // Epilogue address
                        selectedSector.AddRange(new List<byte>() { 0xd5, 0xaa, 0xad }); // Prologe data
                        b = memory.drive.Encode6_2(track, sec).ToList();
                        Console.WriteLine("Encode:");
                        Console.WriteLine(Print(b));
                        Console.WriteLine("Data:");
                        Console.WriteLine(Print(memory.drive.GetSectorData(track, sec).ToList()));
                        selectedSector.AddRange(b); // Data field + checksum
                        selectedSector.AddRange(new List<byte>() { 0xde, 0xaa, 0xeb }); // Epilogue
                        selectedTrack.AddRange(selectedSector);
                        Console.WriteLine("-------------------------------------------------------------");
                    }
                    for (int i = 0;i < trackSize;i++)
                    {
                        rawdisktrack[i] = selectedTrack[i];
                    }

                    pointer = 0;
                }

                 if (pointer > trackSize - 1)
                     pointer = 0;
                
                return rawdisktrack[pointer++];
            }
        }
        if (address == 0xc080 + slotOffset)
            memory.softswitches.DrivePhase0ON_OFF = false;
        if (address == 0xc081 + slotOffset)
            memory.softswitches.DrivePhase0ON_OFF = true;
        if (address == 0xc082 + slotOffset)
            memory.softswitches.DrivePhase1ON_OFF = false;
        if (address == 0xc083 + slotOffset)
            memory.softswitches.DrivePhase1ON_OFF = true;
        if (address == 0xc084 + slotOffset)
            memory.softswitches.DrivePhase2ON_OFF = false;
        if (address == 0xc085 + slotOffset)
            memory.softswitches.DrivePhase2ON_OFF = true;
        if (address == 0xc086 + slotOffset)
            memory.softswitches.DrivePhase3ON_OFF = false;
        if (address == 0xc087 + slotOffset)
            memory.softswitches.DrivePhase3ON_OFF = true;
        if (address == 0xc088 + slotOffset)
            memory.softswitches.DriveMotorON_OFF = false;
        if (address == 0xc089 + slotOffset)
            memory.softswitches.DriveMotorON_OFF = true;
        if (address == 0xc08a + slotOffset)
            memory.softswitches.Drive1_2 = true;
        if (address == 0xc08b + slotOffset)
            memory.softswitches.Drive1_2 = false;
        if (address == 0xc08c + slotOffset)
            memory.softswitches.DriveQ6H_L = false;
        if (address == 0xc08d + slotOffset)
            memory.softswitches.DriveQ6H_L = true;
        if (address == 0xc08e + slotOffset)
        {
            memory.softswitches.DriveQ7H_L = false;
            return 0x9a;
        }
            
        if (address == 0xc08f + slotOffset)
            memory.softswitches.DriveQ7H_L = true;

        return 0;
    }

    string Print(List<byte> bytes)
    {
        string ret = "";
        foreach (var b in bytes)
        {
            ret += b.ToString("X2") + " ";
        }
        return ret;
    }
}
