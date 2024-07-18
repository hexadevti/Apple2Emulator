using Apple2Sharp.Mainboard.Enums;
using Apple2Sharp.Mainboard.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apple2Sharp.Mainboard
{
    public class Sound
    {
        static int soundCycles = 0;
        static bool switchJumpInterval = false;
        static byte[] bytes = new byte[4800];
        static DateTime countTime = DateTime.Now;

        static int k = 0;

        static double adjcycle = 50;

        static int cyclesPerMilliseconds = 19500;
        static int adjCoef = 100;
        public static void Sync(Apple2Board _mainBoard)
        {
            if (soundCycles > (switchJumpInterval ? 1 : 1))
            {
                switchJumpInterval = !switchJumpInterval;

                if (k < bytes.Length)
                {
                    bytes[k] = (byte)(_mainBoard.softswitches.SoundClick ? 0x80 : 0x00);
                    k++;
                }
                else
                {
                    _mainBoard.clickBuffer.Enqueue(bytes);
                    k = 0;
                    bytes = new byte[bytes.Length];
                }


                TimeSpan delta2 = DateTime.Now - countTime;
                if (delta2.TotalMilliseconds >= adjcycle)
                {
                    // mainBoard.screenLog.Enqueue(" Queue buffer: " + mainBoard.clickBuffer.Count()
                    //  + " cyclesPerMilliseconds = " + cyclesPerMilliseconds);

                    if (_mainBoard.clickBuffer.Count() > 2)
                    {
                        cyclesPerMilliseconds -= (_mainBoard.clickBuffer.Count() - 2) * adjCoef;
                    }
                    else if (_mainBoard.clickBuffer.Count() < 2)
                    {
                        cyclesPerMilliseconds += (2 - _mainBoard.clickBuffer.Count()) * adjCoef;
                    }

                    if (adjCoef > 10)
                        adjCoef--;

                    countTime = DateTime.Now;
                }
                soundCycles = 0;
            }
            else
            {
                soundCycles++;
            }
        }


    }
}
