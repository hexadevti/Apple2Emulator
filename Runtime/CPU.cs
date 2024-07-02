using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Text;
using Runtime.OpCodeProcessors;
using Runtime.Overlays;




namespace Runtime;

public class CPU
{

    public State state { get; set; }
    public Memory memory { get; set; }
    public ushort lastPC = 0;
    public DateTime last1mhz = DateTime.MinValue;
    public double deleyloops = 0;
    public CPU(State state, Memory memory)
    {
        this.memory = memory;
        this.state = state;
        last1mhz = DateTime.Now;
    }

    public void WarmStart()
    {
        memory.Clear();
        Thread.Sleep(100);
        Reset();
    }
    public void Reset()
    {
        lastPC = 0;
        state.PC = memory.ReadAddressLLHH(0xfffc) ?? 0;
    }

    public void IncPC()
    {
        lastPC = state.PC;
        state.PC++;
    }

    public void RunCycle()
    {
        byte instruction = memory.ReadByte(state.PC);
        OpCodePart? opCodePart = OpCodes.GetOpCode(instruction);
        ushort? refAddress = OpCodes.ProcessAddressing(opCodePart, state, memory, this);
        OpCodes.Process(opCodePart, state, memory, refAddress);
        EnqueueCycles(opCodePart);
    }

    public void EnqueueCycles(OpCodePart? opCodePart)
    {
        int cycles = opCodePart != null ? opCodePart.Cycles : 1;
        for (int i = 0;i<cycles;i++)
        {
            memory.cycleWait.Enqueue(true);
            memory.cpuCycles++;
        }
    }

    public void DelayedRun(bool running)
    {
        int countFreq = 0;
        DateTime countTime = DateTime.Now;
        memory.cpuCycles = 0;
        int soundCycles = 0;
        Stopwatch sw3 = Stopwatch.StartNew();
        int countOnCycles = 0;
        int bufferSize = 6400;
        int k = 0;
        byte[] bytes = new byte[bufferSize];

        Thread.Sleep(100);

        double elapsedCycleTime = 600; // 1100000 / delay; // 3500; // 1200;
        double adjcycle = 100;
        Stopwatch sw;
        sw = Stopwatch.StartNew();
        while (running)
        {

            if (memory.adjust1Mhz)
            {
                bool n = false;
                if (sw.Elapsed.TotalNanoseconds >= elapsedCycleTime)
                {
                    if (!memory.cycleWait.TryDequeue(out n))
                    {
                        RunCycle();
                        if (soundCycles > 0)
                        {
                            countFreq++;

                            if (k < bufferSize)
                            {
                                bytes[k] = (byte)(memory.softswitches.SoundClick ? 0x80 : 0x00);
                            }
                            else
                            {
                                memory.clickBuffer.Enqueue(bytes);
                                k = 0;
                                bytes = new byte[bufferSize];
                            }

                            k++;

                            TimeSpan delta2 = DateTime.Now - countTime;
                            if (delta2.TotalMilliseconds >= adjcycle)
                            {
                                //adjcycle = 100;
                                memory.newText.Enqueue("Sound Cycle = " + countFreq
                                 + " Queue buffer: " + memory.clickBuffer.Count()
                                 + " elepsedCycleTime = " + elapsedCycleTime);

                                if (memory.clickBuffer.Count() > 2)
                                {
                                    elapsedCycleTime += (memory.clickBuffer.Count() - 2) * 2;
                                }
                                else if (memory.clickBuffer.Count() < 2)
                                {
                                    elapsedCycleTime -= (2 - memory.clickBuffer.Count()) * 2;
                                }

                                countFreq = 0;
                                countTime = DateTime.Now;
                            }
                            soundCycles = 0;

                            if (countOnCycles >= 100)
                            {
                                memory.softswitches.SoundClick = false;
                            }

                        }
                        else
                        {
                            soundCycles++;
                        }
                    }
                    sw = Stopwatch.StartNew();
                }
            }
            else
            {
                deleyloops = 0;
                RunCycle();
            }
            if (memory.cpuCycles >= 1000000)
            {
                sw3.Stop();
                memory.clockSpeed = sw3.Elapsed.TotalMilliseconds;
                memory.cpuCycles = 0;
                sw3 = Stopwatch.StartNew();
            }

        }
    }
}


