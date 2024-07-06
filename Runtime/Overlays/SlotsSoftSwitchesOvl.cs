
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Runtime.Abstractions;

namespace Runtime.Overlays;

public class SlotsSoftSwitchesOvl
{
    int slot = 6;
    int pointer = 0;
    int trackSize = 5856;
    Dictionary<string, List<byte>> output = new Dictionary<string, List<byte>>();
    public void Write(ushort address, byte b, Memory memory)
    {
        int slotOffset = slot * 0x10;
        var sec = memory.softswitches.Drive1_2 ? (memory.drive1.FlagDos_Prodos ? memory.ReadByte(0x2d) : memory.ReadByte(0xd357)) : (memory.drive2.FlagDos_Prodos ? memory.ReadByte(0x2d) : memory.ReadByte(0xd357));
        var trk = memory.softswitches.Drive1_2 ? memory.drive1.track : memory.drive2.track;
        string key = trk + "_" + sec;
        if (memory.softswitches.Drive1_2)
        {
            memory.drive1.sector = sec;
        }
        else
        {
            memory.drive2.sector = sec;
        }

        if (address == 0xc08d + slotOffset)
        {
            if (memory.softswitches.DriveQ6H_L == false && memory.softswitches.DriveQ7H_L == true)
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

                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.ff") + " Save Track: " + trk + " Sector DOS: " + memory.ReadByte(0x2d) + " Sector PRODOS: " + memory.ReadByte(0xd357));

                List<string> keysToClear = new List<string>();
                foreach (var data in output)
                {
                    if (data.Value.Count == 354)
                    {
                        byte[] cleanData = data.Value.Skip(7).Take(343).ToArray();

                        if (memory.softswitches.Drive1_2)
                        {
                            byte[] decsecData = memory.drive1.Decode6_2(cleanData);
                            Console.WriteLine(memory.drive1.Print(decsecData.ToList()));

                            if (memory.drive1.FlagDos_Prodos)
                                memory.drive1.SetSectorData(trk, memory.drive1.translateDos33Track[sec], decsecData); // DOS
                            else
                                memory.drive1.SetBlockData(trk, sec, decsecData); // PRODOS
                            memory.drive1.SaveImage();
                            memory.drive1.TrackRawData(trk, true);
                        }
                        else
                        {
                            byte[] decsecData = memory.drive2.Decode6_2(cleanData);
                            if (memory.drive2.FlagDos_Prodos)
                                memory.drive2.SetSectorData(trk, memory.drive2.translateDos33Track[sec], decsecData); //  DOS
                            else
                                memory.drive2.SetBlockData(trk, sec, decsecData); //  PRODOS
                            memory.drive2.SaveImage();
                            memory.drive2.TrackRawData(trk, true);
                        }
                        keysToClear.Add(key);
                    }
                }

                foreach (var keyd in keysToClear)
                {
                    output.Remove(keyd);
                }
            }
        }

        

        ProcessSwitchc080(address, b, memory, null, slotOffset);
        
    }


    public byte Read(ushort address, Memory memory, State state)
    {
        int slotOffset = slot * 0x10;
        var sec = memory.baseRAM[0x2d];
        var trk = memory.baseRAM[0x2e];
        string key = trk + "_" + sec;
        if (memory.softswitches.Drive1_2)
        {
            memory.drive1.sector = sec;
        }
        else
        {
            memory.drive2.sector = sec;
        }
        if (address == 0xc08c + slotOffset)
        {
            if (memory.softswitches.DriveQ6H_L == false && memory.softswitches.DriveQ7H_L == false)
            {
                if (pointer > trackSize - 1)
                    pointer = 0;
                if (memory.softswitches.Drive1_2)
                {
                    memory.drive1.TrackRawData(memory.drive1.track);
                    return memory.drive1.diskRawData[memory.drive1.track][pointer++];
                }
                else
                {
                    memory.drive2.TrackRawData(memory.drive2.track);
                    return memory.drive2.diskRawData[memory.drive2.track][pointer++];
                }
            }
        }
        
        return ProcessSwitchc080(address, 0, memory, state, slotOffset);
    }

    private byte ProcessSwitchc080(ushort address, byte b, Memory memory, State? state, int slotOffset)
    {
        if (address == 0xc0b0)
        {
            memory.softswitches.cols80PageSelect = 0;
        }
        else if (address == 0xc0b4)
        {
            memory.softswitches.cols80PageSelect = 1;
        }
        else if (address == 0xc0b8)
        {
            memory.softswitches.cols80PageSelect = 2;
        }
        else if (address == 0xc0bc)
        {
            memory.softswitches.cols80PageSelect = 3;
        }

        if (address == 0xc080 + slotOffset)
        {
            memory.softswitches.DrivePhase0ON_OFF = false;
            if (memory.softswitches.Drive1_2)
                memory.drive1.AddPhase(0x00);
            else
                memory.drive2.AddPhase(0x00);
        }
        else if (address == 0xc081 + slotOffset)
        {
            memory.softswitches.DrivePhase0ON_OFF = true;
            if (memory.softswitches.Drive1_2)
                memory.drive1.AddPhase(0x01);
            else
                memory.drive2.AddPhase(0x01);
        }
        else if (address == 0xc082 + slotOffset)
        {
            memory.softswitches.DrivePhase1ON_OFF = false;
            if (memory.softswitches.Drive1_2)
                memory.drive1.AddPhase(0x10);
            else
                memory.drive2.AddPhase(0x10);
        }
        else if (address == 0xc083 + slotOffset)
        {
            memory.softswitches.DrivePhase1ON_OFF = true;
            if (memory.softswitches.Drive1_2)
                memory.drive1.AddPhase(0x11);
            else
                memory.drive2.AddPhase(0x11);
        }
        else if (address == 0xc084 + slotOffset)
        {
            memory.softswitches.DrivePhase2ON_OFF = false;
            if (memory.softswitches.Drive1_2)
                memory.drive1.AddPhase(0x20);
            else
                memory.drive2.AddPhase(0x20);
        }
        else if (address == 0xc085 + slotOffset)
        {
            memory.softswitches.DrivePhase2ON_OFF = true;
            if (memory.softswitches.Drive1_2)
                memory.drive1.AddPhase(0x21);
            else
                memory.drive2.AddPhase(0x21);
        }
        else if (address == 0xc086 + slotOffset)
        {
            memory.softswitches.DrivePhase3ON_OFF = false;
            if (memory.softswitches.Drive1_2)
                memory.drive1.AddPhase(0x30);
            else
                memory.drive2.AddPhase(0x30);
        }
        else if (address == 0xc087 + slotOffset)
        {
            memory.softswitches.DrivePhase3ON_OFF = true;
            if (memory.softswitches.Drive1_2)
                memory.drive1.AddPhase(0x31);
            else
                memory.drive2.AddPhase(0x31);
        }
        else if (address == 0xc088 + slotOffset)
        {
            memory.softswitches.DriveMotorON_OFF = false;
            if (memory.softswitches.Drive1_2)
                memory.drive1.on = false;
            else
                memory.drive2.on = false;
        }
        else if (address == 0xc089 + slotOffset)
        {
            memory.softswitches.DriveMotorON_OFF = true;
            if (memory.softswitches.Drive1_2)
            {
                memory.drive1.on = true;
                memory.drive2.on = false;
            }
            else
            {
                memory.drive2.on = true;
                memory.drive1.on = false;
            }
        }
        else if (address == 0xc08a + slotOffset)
            memory.softswitches.Drive1_2 = true;
        else if (address == 0xc08b + slotOffset)
            memory.softswitches.Drive1_2 = false;
        else if (address == 0xc08c + slotOffset)
            memory.softswitches.DriveQ6H_L = false;
        else if (address == 0xc08d + slotOffset)
            memory.softswitches.DriveQ6H_L = true;
        else if (address == 0xc08e + slotOffset)
        {
            memory.softswitches.DriveQ7H_L = false;
            return 0; // Not Write Protected, 9f Write protected
        }
        else if (address == 0xc08f + slotOffset)
        {
            memory.softswitches.DriveQ7H_L = true;
        }
        return 0;
    }
}
