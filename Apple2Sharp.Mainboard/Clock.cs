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
            Stopwatch sw3 = Stopwatch.StartNew();
            bool n = false;
            int cpuCycles = 0;
            int cyclesPerMilliseconds = 19500;
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
                        Sound.Sync(_mainBoard);
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
                        _cpu.RunCycle();
                    }
                    else
                    {
                        cyclesPerMillisecondsAdj = cyclesPerMilliseconds + ((_mainBoard.audioJumpInterval) * 3000);
                        cpuCycles++;
                        if (!_mainBoard.cycleWait.TryDequeue(out n))
                        {

                            _cpu.RunCycle();

                            Sound.Sync(_mainBoard);
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
