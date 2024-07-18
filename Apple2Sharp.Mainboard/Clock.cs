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
        private Sound _sound { get; set; }

        public Clock(IProcessor cpu, Apple2Board mainBoard)   
        {
            _cpu = cpu;
            _mainBoard = mainBoard;
            _sound = new Sound();
        }
        public void Run()
        {
            Stopwatch sw3 = Stopwatch.StartNew();
            bool n = false;
            int cpuCycles = 0;
            int cyclesPerMilliseconds = 23000;
            int cyclesPerMillisecondsAdj = cyclesPerMilliseconds;

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
                        cyclesPerMilliseconds = _sound.Sync(_mainBoard, cyclesPerMilliseconds);
                    }

                    Joystick.Process(_mainBoard);

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
                        if (!_mainBoard.cycleWait.TryDequeue(out n))
                        {
                            _cpu.RunCycle();
                        }
                        Joystick.Process(_mainBoard, 0.285f);
                    }
                    else
                    {
                        cyclesPerMillisecondsAdj = cyclesPerMilliseconds + (_mainBoard.audioJumpInterval * 15000);
                        cpuCycles++;
                        if (!_mainBoard.cycleWait.TryDequeue(out n))
                        {
                            _cpu.RunCycle();
                            cyclesPerMillisecondsAdj = _sound.Sync(_mainBoard, cyclesPerMillisecondsAdj, (int)(_mainBoard.audioJumpInterval ));
                        }

                        Joystick.Process(_mainBoard);

                        if (cpuCycles >= cyclesPerMillisecondsAdj)
                        {
                            Thread.Sleep(1);
                            cpuCycles = 0;
                        }
                    }
                }
                if (_mainBoard.cpuCycles >= 1000000)
                {
                    if (_mainBoard.adjust1Mhz || _mainBoard.audioJumpInterval < 10)
                        _mainBoard.screenLog.Enqueue("Soundbuf: " + _mainBoard.clickBuffer.Count()
                     + " cPerMilli = " + cyclesPerMilliseconds
                     + " cw = " + _mainBoard.cycleWait.Count());
                
                    sw3.Stop();
                    _mainBoard.clockSpeed = sw3.Elapsed.TotalMilliseconds;
                    _mainBoard.cpuCycles = 0;
                    sw3 = Stopwatch.StartNew();
                }


            }
        }


    }
}
