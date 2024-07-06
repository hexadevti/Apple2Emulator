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
        Thread.Sleep(100);
    }
    public void Reset()
    {
        lastPC = 0;
        state.PC = memory.ReadAddressLLHH(0xfffc) ?? 0;
    }

    public void IncPC()
    {
        state.PC++;
    }

    public void RunCycle()
    {
        byte instruction = memory.ReadByte(state.PC);
        lastPC = state.PC;
        OpCodePart? opCodePart = OpCodes.GetOpCode(instruction);
        if (lastPC == 0xc87d)
        {
            Thread.Sleep(1);
        }
        ushort? refAddress = OpCodes.ProcessAddressing(opCodePart, state, memory, this);

        
        OpCodes.Process(opCodePart, state, memory, refAddress);
        EnqueueCycles(opCodePart);
    }

    public void EnqueueCycles(OpCodePart? opCodePart)
    {
        if (memory.adjust1Mhz)
        {
            int cycles = opCodePart != null ? opCodePart.Cycles : 1;
            for (int i = 0; i < cycles; i++)
            {
                memory.cycleWait.Enqueue(true);
                memory.cpuCycles++;
            }
        }
        else
        {
            memory.cpuCycles++;
        }
    }

    public void DelayedRun(bool running)
    {
        DateTime countTime = DateTime.Now;
        int soundCycles = 0;
        Stopwatch sw3 = Stopwatch.StartNew();
        int bufferSize = 6000;
        int k = 0;
        byte[] bytes = new byte[bufferSize];

        Thread.Sleep(100);

        double elapsedCycleTime = 600;
        double adjcycle = 100;
        Stopwatch sw;
        sw = Stopwatch.StartNew();
        bool switchJumpInterval = false;
        int baseAudioJumpInterval = 20;
        bool n = false;
        int audioFineTuning = 0;

        while (running)
        {

            if (memory.adjust1Mhz)
            {
                
                if (sw.Elapsed.TotalNanoseconds >= elapsedCycleTime)
                {
                    if (!memory.cycleWait.TryDequeue(out n))
                    {
                        RunCycle();
                        if (soundCycles > (switchJumpInterval ? 1 : 0))
                        {
                            switchJumpInterval = !switchJumpInterval;

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
                                memory.screenLog.Enqueue(" Queue buffer: " + memory.clickBuffer.Count()
                                 + " elepsedCycleTime = " + elapsedCycleTime);

                                if (memory.clickBuffer.Count() > 2)
                                {
                                    elapsedCycleTime += (memory.clickBuffer.Count() - 2) * 2;
                                }
                                else if (memory.clickBuffer.Count() < 2)
                                {
                                    elapsedCycleTime -= (2 - memory.clickBuffer.Count()) * 2;
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
                    sw = Stopwatch.StartNew();
                }
            }
            else
            {
                if (memory.audioJumpInterval == 10 || sw.Elapsed.TotalNanoseconds >= elapsedCycleTime - 60 * memory.audioJumpInterval + audioFineTuning)
                {
                    if (memory.audioJumpInterval == 10 || !memory.cycleWait.TryDequeue(out n))
                    {
                        RunCycle();
                        
                        if (memory.audioJumpInterval != 10 && soundCycles > memory.audioJumpInterval + baseAudioJumpInterval)
                        {

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
                                memory.screenLog.Enqueue(" Queue buffer: " + memory.clickBuffer.Count()
                                 + " audioJumpInterval = " + memory.audioJumpInterval + baseAudioJumpInterval
                                 + " elepsedCycleTime = " + (elapsedCycleTime - 60 * memory.audioJumpInterval + audioFineTuning));

                                if (memory.clickBuffer.Count() > 1)
                                {
                                    baseAudioJumpInterval += (memory.clickBuffer.Count() - 1);
                                }
                                else if (memory.clickBuffer.Count() < 1)
                                {
                                    baseAudioJumpInterval -= (1 - memory.clickBuffer.Count());
                                }
                                if (memory.clickBuffer.Count() > 1)
                                {
                                    audioFineTuning += (memory.clickBuffer.Count() - 1) * 2;
                                }
                                else if (memory.clickBuffer.Count() < 1)
                                {
                                    audioFineTuning -= (1 - memory.clickBuffer.Count()) * 2;
                                }

                                countTime = DateTime.Now;
                            }
                            soundCycles = 0;
                        }
                        else if (memory.audioJumpInterval != 10)
                        {
                            soundCycles++;
                        }
                    }
                    if (memory.audioJumpInterval != 10)
                        sw = Stopwatch.StartNew();
                }
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


