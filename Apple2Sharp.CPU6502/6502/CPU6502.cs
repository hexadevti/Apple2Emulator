using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using Apple2Sharp.Mainboard;
using Apple2Sharp.Mainboard.Interfaces;
using Apple2Sharp.Mainboard.Enums;
using System.Threading.Tasks.Dataflow;

namespace Apple2Sharp.CPU6502
{
    public class CPU6502 : IProcessor
    {
        public State state { get; set; }
        public Apple2Board mainBoard { get; set; }
        public CpuState cpuState { get; set; }
        public ushort lastPC = 0;
        public DateTime last1mhz = DateTime.MinValue;

        public CPU6502(State state, Apple2Board mainBoard)
        {
            this.mainBoard = mainBoard;
            this.state = state;
            last1mhz = DateTime.Now;
            cpuState = CpuState.Stopped;
        }

        public void WarmStart()
        {
            cpuState = CpuState.Paused;
            mainBoard.ClearBaseRAM();
            mainBoard.softswitches.SlotC3RomOn_Off = true;
            mainBoard.softswitches.IntCXRomOn_Off = false;
            mainBoard.softswitches.AltCharSetOn_Off = false;
            mainBoard.softswitches.Store80On_Off = false;
            mainBoard.softswitches.IntC8RomOn_Off = false;
            mainBoard.softswitches.IIEMemoryBankReadRAM_ROM = false;
            mainBoard.softswitches.RAMReadOn_Off = false;
            mainBoard.softswitches.AltZPOn_Off = false;
            mainBoard.softswitches.IOUDisOn_Off = true;
            mainBoard.softswitches.DHiResOn_Off = false;
            Reset();
            cpuState = CpuState.Running;
        }
        public void Reset()
        {
            state = new State();
            lastPC = 0;
            cpuState = CpuState.Paused;
            state.PC = mainBoard.ReadAddressLLHH(0xfffc) ?? 0;
            cpuState = CpuState.Running;

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
            if (lastPC == 0x008f)
            {
               Thread.Sleep(1);
            }
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
