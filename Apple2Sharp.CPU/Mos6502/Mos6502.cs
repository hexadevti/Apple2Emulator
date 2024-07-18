using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using Apple2Sharp.Mainboard;
using Apple2Sharp.Mainboard.Interfaces;
using Apple2Sharp.Mainboard.Enums;
using Apple2Sharp.CPU.Mos6502;
using System.Threading.Tasks.Dataflow;

namespace Apple2Sharp.CPU.Mos6502
{
    public class Mos6502 : IProcessor
    {
        public State state { get; set; }
        public Apple2Board mainBoard { get; set; }
        public CpuState cpuState { get; set; }
        public ushort lastPC = 0;
        public DateTime last1mhz = DateTime.MinValue;

        public Mos6502(State state, Apple2Board mainBoard)
        {
            this.mainBoard = mainBoard;
            this.state = state;
            last1mhz = DateTime.Now;
            cpuState = CpuState.Running;
        }

        public void WarmStart()
        {
            cpuState = CpuState.Paused;
            mainBoard.ClearBaseRAM();
            Reset();
            cpuState = CpuState.Running;
        }
        public void Reset()
        {
            state = new State();
            lastPC = 0;
            state.PC = mainBoard.ReadAddressLLHH(0xfffc) ?? 0;
        }
        public void IncrementPC()
        {
            state.PC++;
        }
        public void RunCycle()
        {
            byte instruction = mainBoard.ReadByte(state.PC);
            lastPC = state.PC;
            OpCodePart? opCodePart = OpCodes.GetOpCode(instruction);
            // Break point with lastPC
            //if (lastPC == 0xe000)
            //{
            //    Thread.Sleep(1);
            //}
            ushort? refAddress = OpCodes.ProcessAddressing(opCodePart, state, mainBoard, this);
            OpCodes.Process(opCodePart, state, mainBoard, refAddress);
            EnqueueCycles(opCodePart);
        }
        public void EnqueueCycles(OpCodePart? opCodePart)
        {
            int cycles = opCodePart != null ? opCodePart.Cycles : 1;
            for (int i = 0; i < cycles; i++)
            {
                if (mainBoard.adjust1Mhz || (!mainBoard.adjust1Mhz && mainBoard.audioJumpInterval != 10))
                    mainBoard.cycleWait.Enqueue(true);
                mainBoard.cpuCycles++;
            }
        
        }
        public void RunFast()
        {
            Stopwatch sw3 = Stopwatch.StartNew();
            while (cpuState != CpuState.Stopped)
            {
                RunCycle();
                if (mainBoard.cpuCycles >= 1000000)
                {
                    sw3.Stop();
                    mainBoard.clockSpeed = sw3.Elapsed.TotalMilliseconds;
                    mainBoard.cpuCycles = 0;
                    sw3 = Stopwatch.StartNew();
                }
            }
        }
    }

    
}
