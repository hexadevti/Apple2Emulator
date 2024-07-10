using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Text;
using Runtime.Abstractions;

namespace Runtime
{
    public class CpuSoftswitches
    {
        public void Write(ushort address, byte b, MainBoard mainBoard)
        {
            ProcessSwitch(address, b, mainBoard, null);
        }

        public byte Read(ushort address, MainBoard mainBoard, State state)
        {
            return ProcessSwitch(address, 0x00, mainBoard, state);
        }

        private byte ProcessSwitch(ushort address, byte b, MainBoard mainBoard, State? state)
        {
            if (address == 0xc000)
                return mainBoard.KeyPressed;
            else if (address == 0xc010)
                mainBoard.KeyPressed = mainBoard.KeyPressed < 0x80 ? mainBoard.KeyPressed : (byte)(mainBoard.KeyPressed ^ 0b10000000);
            // else if (address == 0xc00c)
            //     mainBoard.softswitches.Cols40_80 = true; // Apple IIc IIe
            // else if (address == 0xc00d)
            //     mainBoard.softswitches.Cols40_80 = false; // Apple IIc IIe
            else if (address == 0xc030)
            {
                mainBoard.softswitches.SoundClick = !mainBoard.softswitches.SoundClick;
            }
            else if (address == 0xc050)
                mainBoard.softswitches.Graphics_Text = true;
            else if (address == 0xc051)
                mainBoard.softswitches.Graphics_Text = false;
            else if (address == 0xc052)
                mainBoard.softswitches.DisplayFull_Split = true;
            else if (address == 0xc053)
                mainBoard.softswitches.DisplayFull_Split = false;
            else if (address == 0xc054)
            {
                lock (mainBoard.displayLock)
                {
                    mainBoard.softswitches.TextPage1_Page2 = true;
                }
            }
            else if (address == 0xc055)
            {
                lock (mainBoard.displayLock)
                {
                    mainBoard.softswitches.TextPage1_Page2 = false;
                }
            }
            else if (address == 0xc056)
                mainBoard.softswitches.LoRes_HiRes = true;
            else if (address == 0xc057)
                mainBoard.softswitches.LoRes_HiRes = false;
            else if (address == 0xc058)
                mainBoard.softswitches.Cols40_80 = true;
            else if (address == 0xc059)
                mainBoard.softswitches.Cols40_80 = false;
            else if (address == 0xc063)
                return 0x80; // Apple II+ default. For Apple IIe it is defined by shift key pressed
            return 0;

        }
    }
}