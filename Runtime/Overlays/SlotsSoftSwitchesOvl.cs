
using System.Reflection;
using System.Security.Cryptography;
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

    int trackSize = 5856;
    
    Dictionary<string, List<byte>> output = new Dictionary<string, List<byte>>();


    public void Write(ushort address, byte b, Memory memory)
    {
        int slotOffset = slot * 0x10;
        var sec = memory.baseRAM[0x2d];
        var trk = memory.baseRAM[0x2e];
        string key = trk + "_" + sec;
        if (address == 0xc08d + slotOffset)
        {
            if (memory.drive != null && (memory.softswitches.DriveQ6H_L == false && memory.softswitches.DriveQ7H_L == true))
            {
                if (output.ContainsKey(key))
                {
                    var data = output[key];
                    data.Add(b);
                    output[key] = data;
                }
                else
                {
                    output.Add(key, new List<byte>() { b });
                }
            }
        }
        else if (address == 0xc08e + slotOffset)
        {
            memory.softswitches.DriveQ7H_L = false;
            
        }
        else if (address == 0xc08f + slotOffset)
        {
            memory.softswitches.DriveQ7H_L = true;
        }
        
    }

    public byte Read(ushort address, Memory memory, State state)
    {
        int slotOffset = slot * 0x10;
        var sec = memory.baseRAM[0x2d];
        var trk = memory.baseRAM[0x2e];
        string key = trk + "_" + sec;
        if (address == 0xc08c + slotOffset)
        {
            if (memory.drive != null && (memory.softswitches.DriveQ6H_L == false && memory.softswitches.DriveQ7H_L == false))
            {
                byte track = 0;
                if (state.PC > 0xc000 + slot * 0x100 && state.PC < (0xc000 + slot * 0x100) + 0x100)
                {
                    track = 0;
                }
                else
                {
                    track = memory.ReadByte(0x478);
                }

                if ((int)track > 34)
                {
                    track = 0;
                }

                memory.drive.TrackRawData(track);

                

                if (pointer > trackSize - 1)
                    pointer = 0;

                return memory.drive.diskRawData[track][pointer++];
            }
            else if (memory.drive != null && (memory.softswitches.DriveQ6H_L == false && memory.softswitches.DriveQ7H_L == true))
            {
                List<string> keysToClear = new List<string>();
                foreach (var data in output)
                {
                    if (data.Value.Count == 354)
                    {
                        int trkd = int.Parse(data.Key.Split('_')[0]);
                        int secd = int.Parse(data.Key.Split('_')[1]);
                        Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.ff") + " Save Track: " + trkd + " Sector: " + secd);
                        var cleanData = data.Value.Skip(7).Take(343).ToArray();
                        var decsecData = memory.drive.Decode6_2(cleanData);
                        if (trkd == 17 && secd == 15)
                            Console.WriteLine(Print(decsecData.ToList()));
                        memory.drive.SetSectorData(trkd, memory.drive.translateDos33Track[secd], decsecData); 
                        memory.drive.SaveImage();
                        memory.drive.TrackRawData(trkd, true);
                        keysToClear.Add(key);
                    }
                }

                foreach (var keyd in keysToClear)
                {
                    output.Remove(keyd);
                }

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
            return 0; // Not Write Protected, 9f Write protected
        }
        if (address == 0xc08f + slotOffset)
        {
            memory.softswitches.DriveQ7H_L = true;
        }

        return 0;
    }

    string Print(List<byte> bytes)
    {
        string ret = "";
        for (int i = 0; i < bytes.Count; i = i + 16)
        {

            foreach (byte b in bytes.Skip(i).Take(16))
            {
                ret += b.ToString("X2") + " ";
            }
            foreach (byte b in bytes.Skip(i).Take(16))
            {
                ret += Convert.ToChar((byte)(b - 0x80));
            }
            ret += "\r\n";
        }
        return ret;
    }
}
