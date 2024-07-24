
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Apple2Sharp.Mainboard.Interfaces;

namespace Apple2Sharp.Mainboard.Cards
{
    public class DiskIICard : ICard
    {
        public bool Empty { get { return false; } }
        private int pointer = 0;
        private int _slotNumber = 6;
        private byte[] _c000ROM;
        public DiskDrive drive1 { get; set; }
        public DiskDrive drive2 { get; set; }

        public bool Drive1_2 { get; set; }

        public bool DrivePhase0ON_OFF { get; set; }
        public bool DrivePhase1ON_OFF { get; set; }
        public bool DrivePhase2ON_OFF { get; set; } 
        public bool DrivePhase3ON_OFF { get; set; }

        public bool DriveQ6H_L { get; set; }
        public bool DriveQ7H_L { get; set; }
        public bool DriveMotorON_OFF { get; set; }
        
        

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

        Dictionary<string, List<byte>> output = new Dictionary<string, List<byte>>();
        public DiskIICard(int slotNumber, byte[] c000ROM, string disk1, string disk2)
        {
            _slotNumber = slotNumber;
            _c000ROM = c000ROM;
            this.drive1 = new DiskDrive(disk1, this);
            this.drive2 = new DiskDrive(disk2, this);
        }

        public void Write(ushort address, byte b, Apple2Board mainBoard)
        {
         
            var sec = Drive1_2 ? (drive1.FlagDos_Prodos ? mainBoard.ReadByte(0x2d) : mainBoard.ReadByte(0xd357)) : (drive2.FlagDos_Prodos ? mainBoard.ReadByte(0x2d) : mainBoard.ReadByte(0xd357));
            var trk = Drive1_2 ? drive1.track : drive2.track;
            string key = trk + "_" + sec;
            if (Drive1_2)
            {
                drive1.sector = sec;
            }
            else
            {
                drive2.sector = sec;
            }

            if (address == 0xc08d + SlotNumber * 0x10)
            {
                if (DriveQ6H_L == false && DriveQ7H_L == true)
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

                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.ff") + " Save Track: " + trk + " Sector DOS: " + mainBoard.ReadByte(0x2d) + " Sector PRODOS: " + mainBoard.ReadByte(0xd357));

                    List<string> keysToClear = new List<string>();
                    foreach (var data in output)
                    {
                        if (data.Value.Count == 354)
                        {
                            byte[] cleanData = data.Value.Skip(7).Take(343).ToArray();

                            if (Drive1_2)
                            {
                                byte[] decsecData = drive1.Decode6_2(cleanData);
                                Console.WriteLine(drive1.Print(decsecData.ToList()));

                                if (drive1.FlagDos_Prodos)
                                    drive1.SetSectorData(trk, drive1.translateDos33Track[sec], decsecData); // DOS
                                else
                                    drive1.SetBlockData(trk, sec, decsecData); // PRODOS
                                drive1.SaveImage();
                                drive1.TrackRawData(trk, true);
                            }
                            else
                            {
                                byte[] decsecData = drive2.Decode6_2(cleanData);
                                if (drive2.FlagDos_Prodos)
                                    drive2.SetSectorData(trk, drive2.translateDos33Track[sec], decsecData); //  DOS
                                else
                                    drive2.SetBlockData(trk, sec, decsecData); //  PRODOS
                                drive2.SaveImage();
                                drive2.TrackRawData(trk, true);
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



            ProcessSwitchc080(address, b, mainBoard);

        }


        public byte Read(ushort address, Apple2Board mainBoard)
        {
            int trackSize = 5856;
            var sec = mainBoard.baseRAM[0x2d];
            var trk = mainBoard.baseRAM[0x2e];
            string key = trk + "_" + sec;
            if (Drive1_2)
            {
                drive1.sector = sec;
            }
            else
            {
                drive2.sector = sec;
            }
            if (address == 0xc08c + _slotNumber * 0x10)
            {
                if (DriveQ6H_L == false && DriveQ7H_L == false)
                {
                    if (pointer > trackSize - 1)
                        pointer = 0;
                    if (Drive1_2)
                    {
                        drive1.TrackRawData(drive1.track);
                        if (drive1.diskRawData[drive2.track] != null)
                            return drive1.diskRawData[drive1.track][pointer++];
                        else
                            return 0;
                    }
                    else
                    {
                        drive2.TrackRawData(drive2.track);
                        if (drive2.diskRawData[drive2.track] != null)
                            return drive2.diskRawData[drive2.track][pointer++];
                        else return 0;
                    }
                }
            }

            return ProcessSwitchc080(address, 0, mainBoard);
        }

        private byte ProcessSwitchc080(ushort address, byte b, Apple2Board mainBoard)
        {

            if (address == 0xc080 + _slotNumber * 0x10)
            {
                DrivePhase0ON_OFF = false;
                if (Drive1_2)
                    drive1.AddPhase(0x00);
                else
                    drive2.AddPhase(0x00);
            }
            else if (address == 0xc081 + _slotNumber * 0x10)
            {
                DrivePhase0ON_OFF = true;
                if (Drive1_2)
                    drive1.AddPhase(0x01);
                else
                    drive2.AddPhase(0x01);
            }
            else if (address == 0xc082 + _slotNumber * 0x10)
            {
                DrivePhase1ON_OFF = false;
                if (Drive1_2)
                    drive1.AddPhase(0x10);
                else
                    drive2.AddPhase(0x10);
            }
            else if (address == 0xc083 + _slotNumber * 0x10)
            {
                DrivePhase1ON_OFF = true;
                if (Drive1_2)
                    drive1.AddPhase(0x11);
                else
                    drive2.AddPhase(0x11);
            }
            else if (address == 0xc084 + _slotNumber * 0x10)
            {
                DrivePhase2ON_OFF = false;
                if (Drive1_2)
                    drive1.AddPhase(0x20);
                else
                    drive2.AddPhase(0x20);
            }
            else if (address == 0xc085 + _slotNumber * 0x10)
            {
                DrivePhase2ON_OFF = true;
                if (Drive1_2)
                    drive1.AddPhase(0x21);
                else
                    drive2.AddPhase(0x21);
            }
            else if (address == 0xc086 + _slotNumber * 0x10)
            {
                DrivePhase3ON_OFF = false;
                if (Drive1_2)
                    drive1.AddPhase(0x30);
                else
                    drive2.AddPhase(0x30);
            }
            else if (address == 0xc087 + _slotNumber * 0x10)
            {
                DrivePhase3ON_OFF = true;
                if (Drive1_2)
                    drive1.AddPhase(0x31);
                else
                    drive2.AddPhase(0x31);
            }
            else if (address == 0xc088 + _slotNumber * 0x10)
            {
                DriveMotorON_OFF = false;
                if (Drive1_2)
                    drive1.on = false;
                else
                    drive2.on = false;
            }
            else if (address == 0xc089 + _slotNumber * 0x10)
            {
                DriveMotorON_OFF = true;
                if (Drive1_2)
                {
                    drive1.on = true;
                    drive2.on = false;
                }
                else
                {
                    drive2.on = true;
                    drive1.on = false;
                }
            }
            else if (address == 0xc08a + _slotNumber * 0x10)
                Drive1_2 = true;
            else if (address == 0xc08b + _slotNumber * 0x10)
                Drive1_2 = false;
            else if (address == 0xc08c + _slotNumber * 0x10)
                DriveQ6H_L = false;
            else if (address == 0xc08d + _slotNumber * 0x10)
                DriveQ6H_L = true;
            else if (address == 0xc08e + _slotNumber * 0x10)
            {
                DriveQ7H_L = false;
                return 0; // Not Write Protected, 9f Write protected
            }
            else if (address == 0xc08f + _slotNumber * 0x10)
            {
                DriveQ7H_L = true;
            }
            return 0;
        }
    }
}