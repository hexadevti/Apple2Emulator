//using Apple2Sharp.Mainboard.Enums;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Apple2Sharp.Mainboard
//{
//    public class Clock
//    {
//        private Mos6502 _cpu;
//        public Apple2Board mainBoard { get; set; }
//        public CpuState cpuState { get; set; }
//        public void Run()
//        {
//            DateTime countTime = DateTime.Now;
//            int soundCycles = 0;
//            Stopwatch sw3 = Stopwatch.StartNew();
//            int k = 0;
//            byte[] bytes = new byte[mainBoard.audioBuffer];
//            double adjcycle = 50;
//            bool switchJumpInterval = false;
//            bool n = false;
//            int cpuCycles = 0;
//            int cyclesPerMilliseconds = 19500;
//            int adjCoef = 100;
//            int cyclesPerMillisecondsAdj = cyclesPerMilliseconds;
//            int joystickCycles0 = 0;
//            int joystickCycles1 = 0;
//            int joystickCycles2 = 0;
//            int joystickCycles3 = 0;

//            while (cpuState != CpuState.Stopped)
//            {
//                if (cpuState == CpuState.Paused)
//                {
//                    Thread.Sleep(50);
//                    continue;
//                }

//                if (mainBoard.adjust1Mhz)
//                {

//                    cpuCycles++;
//                    if (!mainBoard.cycleWait.TryDequeue(out n))
//                    {

//                        RunCycle();

//                        if (soundCycles > (switchJumpInterval ? 1 : 1))
//                        {
//                            switchJumpInterval = !switchJumpInterval;

//                            if (k < mainBoard.audioBuffer)
//                            {
//                                bytes[k] = (byte)(mainBoard.softswitches.SoundClick ? 0x80 : 0x00);
//                                k++;
//                            }
//                            else
//                            {
//                                mainBoard.clickBuffer.Enqueue(bytes);
//                                k = 0;
//                                bytes = new byte[mainBoard.audioBuffer];
//                            }


//                            TimeSpan delta2 = DateTime.Now - countTime;
//                            if (delta2.TotalMilliseconds >= adjcycle)
//                            {
//                                // mainBoard.screenLog.Enqueue(" Queue buffer: " + mainBoard.clickBuffer.Count()
//                                //  + " cyclesPerMilliseconds = " + cyclesPerMilliseconds);

//                                if (mainBoard.clickBuffer.Count() > 2)
//                                {
//                                    cyclesPerMilliseconds -= (mainBoard.clickBuffer.Count() - 2) * adjCoef;
//                                }
//                                else if (mainBoard.clickBuffer.Count() < 2)
//                                {
//                                    cyclesPerMilliseconds += (2 - mainBoard.clickBuffer.Count()) * adjCoef;
//                                }

//                                if (adjCoef > 10)
//                                    adjCoef--;

//                                countTime = DateTime.Now;
//                            }
//                            soundCycles = 0;
//                        }
//                        else
//                        {
//                            soundCycles++;
//                        }
//                    }

//                    if (mainBoard.softswitches.CgReset0)
//                    {
//                        joystickCycles0++;
//                    }
//                    if (mainBoard.softswitches.CgReset1)
//                    {
//                        joystickCycles1++;
//                    }
//                    if (mainBoard.softswitches.CgReset2)
//                    {
//                        joystickCycles2++;
//                    }
//                    if (mainBoard.softswitches.CgReset3)
//                    {
//                        joystickCycles3++;
//                    }

//                    if (joystickCycles0 >= 1790) // for 1Mhz 0 -> 10, 127 (center) -> 1790, 255 -> 3580
//                    {
//                        joystickCycles0 = 0;
//                        mainBoard.softswitches.Cg0 = false;
//                        mainBoard.softswitches.CgReset0 = false;
//                    }

//                    if (joystickCycles1 >= 3500) // for 1Mhz 0 -> 10, 127 (center) -> 1790, 255 -> 3580
//                    {
//                        joystickCycles1 = 0;
//                        mainBoard.softswitches.Cg1 = false;
//                        mainBoard.softswitches.CgReset1 = false;
//                    }

//                    if (joystickCycles2 >= 500) // for 1Mhz 0 -> 10, 127 (center) -> 1790, 255 -> 3580
//                    {
//                        joystickCycles2 = 0;
//                        mainBoard.softswitches.Cg2 = false;
//                        mainBoard.softswitches.CgReset2 = false;
//                    }

//                    if (joystickCycles3 >= 1790) // for 1Mhz 0 -> 10, 127 (center) -> 1790, 255 -> 3580
//                    {
//                        joystickCycles3 = 0;
//                        mainBoard.softswitches.Cg3 = false;
//                        mainBoard.softswitches.CgReset3 = false;
//                    }



//                    if (cpuCycles >= cyclesPerMilliseconds)
//                    {
//                        Thread.Sleep(1);
//                        cpuCycles = 0;
//                    }
//                }
//                else
//                {
//                    if (mainBoard.audioJumpInterval == 10)
//                    {
//                        RunCycle();
//                    }
//                    else
//                    {
//                        cyclesPerMillisecondsAdj = cyclesPerMilliseconds + ((mainBoard.audioJumpInterval) * 3000);
//                        cpuCycles++;
//                        if (!mainBoard.cycleWait.TryDequeue(out n))
//                        {

//                            RunCycle();

//                            if (soundCycles > (switchJumpInterval ? mainBoard.audioJumpInterval / 2 + 1 : mainBoard.audioJumpInterval / 2 + 1))
//                            {
//                                switchJumpInterval = !switchJumpInterval;

//                                if (k < mainBoard.audioBuffer)
//                                {
//                                    bytes[k] = (byte)(mainBoard.softswitches.SoundClick ? 0x80 : 0x00);
//                                }
//                                else
//                                {
//                                    mainBoard.clickBuffer.Enqueue(bytes);
//                                    k = 0;
//                                    bytes = new byte[mainBoard.audioBuffer];
//                                }

//                                k++;

//                                TimeSpan delta2 = DateTime.Now - countTime;
//                                if (delta2.TotalMilliseconds >= adjcycle)
//                                {
//                                    mainBoard.screenLog.Enqueue(" Queue buffer: " + mainBoard.clickBuffer.Count()
//                                     + " cyclesPerMillisecondsAdj = " + cyclesPerMillisecondsAdj);

//                                    if (mainBoard.clickBuffer.Count() > 2)
//                                    {
//                                        cyclesPerMillisecondsAdj -= (mainBoard.clickBuffer.Count() - 2) * 10;
//                                    }
//                                    else if (mainBoard.clickBuffer.Count() < 2)
//                                    {
//                                        cyclesPerMillisecondsAdj += (2 - mainBoard.clickBuffer.Count()) * 10;
//                                    }

//                                    countTime = DateTime.Now;
//                                }
//                                soundCycles = 0;
//                            }
//                            else
//                            {
//                                soundCycles++;
//                            }
//                        }

//                        if (cpuCycles >= cyclesPerMillisecondsAdj)
//                        {
//                            Thread.Sleep(1);
//                            cpuCycles = 0;
//                        }

//                    }

//                }
//                if (mainBoard.cpuCycles >= 1000000)
//                {
//                    sw3.Stop();
//                    mainBoard.clockSpeed = sw3.Elapsed.TotalMilliseconds;
//                    mainBoard.cpuCycles = 0;
//                    sw3 = Stopwatch.StartNew();
//                }


//            }
//        }
//    }
//}
//}
