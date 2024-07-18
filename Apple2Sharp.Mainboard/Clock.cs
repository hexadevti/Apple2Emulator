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
    public class Clock
    {
        private IProcessor _cpu;
        private Apple2Board _mainBoard { get; set; }

        public Clock(IProcessor cpu, Apple2Board mainBoard)   
        {
            _cpu = cpu;
            _mainBoard = mainBoard;
        }
        public void Run()
        {
            DateTime countTime = DateTime.Now;
            int soundCycles = 0;
            Stopwatch sw3 = Stopwatch.StartNew();
            int k = 0;
            byte[] bytes = new byte[_mainBoard.audioBuffer];
            double adjcycle = 50;
            bool switchJumpInterval = false;
            bool n = false;
            int cpuCycles = 0;
            int cyclesPerMilliseconds = 19500;
            int adjCoef = 100;
            int cyclesPerMillisecondsAdj = cyclesPerMilliseconds;
            int joystickCycles0 = 0;
            int joystickCycles1 = 0;
            int joystickCycles2 = 0;
            int joystickCycles3 = 0;

            while (_cpu.cpuState != CpuState.Stopped)
            {
                if (_cpu.cpuState == CpuState.Paused)
                {
                    Thread.Sleep(50);
                    continue;
                }

                if (_mainBoard.adjust1Mhz)
                {

                    cpuCycles++;
                    if (!_mainBoard.cycleWait.TryDequeue(out n))
                    {

                        _cpu.RunCycle();

                        if (soundCycles > (switchJumpInterval ? 1 : 1))
                        {
                            switchJumpInterval = !switchJumpInterval;

                            if (k < _mainBoard.audioBuffer)
                            {
                                bytes[k] = (byte)(_mainBoard.softswitches.SoundClick ? 0x80 : 0x00);
                                k++;
                            }
                            else
                            {
                                _mainBoard.clickBuffer.Enqueue(bytes);
                                k = 0;
                                bytes = new byte[_mainBoard.audioBuffer];
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

                    if (_mainBoard.softswitches.CgReset0)
                    {
                        joystickCycles0++;
                    }
                    if (_mainBoard.softswitches.CgReset1)
                    {
                        joystickCycles1++;
                    }
                    if (_mainBoard.softswitches.CgReset2)
                    {
                        joystickCycles2++;
                    }
                    if (_mainBoard.softswitches.CgReset3)
                    {
                        joystickCycles3++;
                    }

                    if (joystickCycles0 >= _mainBoard.timerpdl0) // 1Mhz 0 -> 10, 127 (center) -> 1790, 255 -> 3580
                    {
                        joystickCycles0 = 0;
                        _mainBoard.softswitches.Cg0 = false;
                        _mainBoard.softswitches.CgReset0 = false;
                    }

                    if (joystickCycles1 >= _mainBoard.timerpdl1) // for 1Mhz 0 -> 10, 127 (center) -> 1790, 255 -> 3580
                    {
                        joystickCycles1 = 0;
                        _mainBoard.softswitches.Cg1 = false;
                        _mainBoard.softswitches.CgReset1 = false;
                    }

                    if (joystickCycles2 >= _mainBoard.timerpdl2) // for 1Mhz 0 -> 10, 127 (center) -> 1790, 255 -> 3580
                    {
                        joystickCycles2 = 0;
                        _mainBoard.softswitches.Cg2 = false;
                        _mainBoard.softswitches.CgReset2 = false;
                    }

                    if (joystickCycles3 >= _mainBoard.timerpdl3) // for 1Mhz 0 -> 10, 127 (center) -> 1790, 255 -> 3580
                    {
                        joystickCycles3 = 0;
                        _mainBoard.softswitches.Cg3 = false;
                        _mainBoard.softswitches.CgReset3 = false;
                    }

                    if (cpuCycles >= cyclesPerMilliseconds)
                    {
                        Thread.Sleep(1);
                        cpuCycles = 0;
                    }
                }
                else
                {
                    if (_mainBoard.audioJumpInterval == 10)
                    {
                        _cpu.RunCycle();
                    }
                    else
                    {
                        cyclesPerMillisecondsAdj = cyclesPerMilliseconds + ((_mainBoard.audioJumpInterval) * 3000);
                        cpuCycles++;
                        if (!_mainBoard.cycleWait.TryDequeue(out n))
                        {

                            _cpu.RunCycle();

                            if (soundCycles > (switchJumpInterval ? _mainBoard.audioJumpInterval / 2 + 1 : _mainBoard.audioJumpInterval / 2 + 1))
                            {
                                switchJumpInterval = !switchJumpInterval;

                                if (k < _mainBoard.audioBuffer)
                                {
                                    bytes[k] = (byte)(_mainBoard.softswitches.SoundClick ? 0x80 : 0x00);
                                }
                                else
                                {
                                    _mainBoard.clickBuffer.Enqueue(bytes);
                                    k = 0;
                                    bytes = new byte[_mainBoard.audioBuffer];
                                }

                                k++;

                                TimeSpan delta2 = DateTime.Now - countTime;
                                if (delta2.TotalMilliseconds >= adjcycle)
                                {
                                    _mainBoard.screenLog.Enqueue(" Queue buffer: " + _mainBoard.clickBuffer.Count()
                                     + " cyclesPerMillisecondsAdj = " + cyclesPerMillisecondsAdj);

                                    if (_mainBoard.clickBuffer.Count() > 2)
                                    {
                                        cyclesPerMillisecondsAdj -= (_mainBoard.clickBuffer.Count() - 2) * 10;
                                    }
                                    else if (_mainBoard.clickBuffer.Count() < 2)
                                    {
                                        cyclesPerMillisecondsAdj += (2 - _mainBoard.clickBuffer.Count()) * 10;
                                    }

                                    countTime = DateTime.Now;
                                }
                                soundCycles = 0;
                            }
                            else
                            {
                                soundCycles++;
                            }
                        }

                        if (cpuCycles >= cyclesPerMillisecondsAdj)
                        {
                            Thread.Sleep(1);
                            cpuCycles = 0;
                        }

                    }

                }
                if (_mainBoard.cpuCycles >= 1000000)
                {
                    sw3.Stop();
                    _mainBoard.clockSpeed = sw3.Elapsed.TotalMilliseconds;
                    _mainBoard.cpuCycles = 0;
                    sw3 = Stopwatch.StartNew();
                }


            }
        }


    }
}
