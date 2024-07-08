using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Text;
using Runtime.OpCodeProcessors;
using Runtime.Cards;




namespace Runtime;

public class CPU
{

    public State state { get; set; }
    public MainBoard mainBoard { get; set; }
    public ushort lastPC = 0;
    public DateTime last1mhz = DateTime.MinValue;

    public CPU(State state, MainBoard mainBoard)
    {
        this.mainBoard = mainBoard;
        this.state = state;
        last1mhz = DateTime.Now;
    }

    public void WarmStart()
    {
        mainBoard.ClearBaseRAM();
        Thread.Sleep(100);
        Reset();
        Thread.Sleep(100);
    }
    public void Reset()
    {
        lastPC = 0;
        state.PC = mainBoard.ReadAddressLLHH(0xfffc) ?? 0;
    }

    public void IncPC()
    {
        state.PC++;
    }

    public void RunCycle()
    {
        byte instruction = mainBoard.ReadByte(state.PC);
        lastPC = state.PC;
        OpCodePart? opCodePart = OpCodes.GetOpCode(instruction);
        if (lastPC == 0xc876)
        {
            Thread.Sleep(1);
        }
        ushort? refAddress = OpCodes.ProcessAddressing(opCodePart, state, mainBoard, this);

        
        OpCodes.Process(opCodePart, state, mainBoard, refAddress);
        EnqueueCycles(opCodePart);
    }

    public void EnqueueCycles(OpCodePart? opCodePart)
    {
        if (mainBoard.adjust1Mhz)
        {
            int cycles = opCodePart != null ? opCodePart.Cycles : 1;
            for (int i = 0; i < cycles; i++)
            {
                mainBoard.cycleWait.Enqueue(true);
                mainBoard.cpuCycles++;
            }
        }
        else
        {
            mainBoard.cpuCycles++;
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

            if (mainBoard.adjust1Mhz)
            {
                
                if (sw.Elapsed.TotalNanoseconds >= elapsedCycleTime)
                {
                    if (!mainBoard.cycleWait.TryDequeue(out n))
                    {
                        RunCycle();
                        if (soundCycles > (switchJumpInterval ? 1 : 0))
                        {
                            switchJumpInterval = !switchJumpInterval;

                            if (k < bufferSize)
                            {
                                bytes[k] = (byte)(mainBoard.softswitches.SoundClick ? 0x80 : 0x00);
                            }
                            else
                            {
                                mainBoard.clickBuffer.Enqueue(bytes);
                                k = 0;
                                bytes = new byte[bufferSize];
                            }

                            k++;

                            TimeSpan delta2 = DateTime.Now - countTime;
                            if (delta2.TotalMilliseconds >= adjcycle)
                            {
                                mainBoard.screenLog.Enqueue(" Queue buffer: " + mainBoard.clickBuffer.Count()
                                 + " elepsedCycleTime = " + elapsedCycleTime);

                                if (mainBoard.clickBuffer.Count() > 2)
                                {
                                    elapsedCycleTime += (mainBoard.clickBuffer.Count() - 2) * 2;
                                }
                                else if (mainBoard.clickBuffer.Count() < 2)
                                {
                                    elapsedCycleTime -= (2 - mainBoard.clickBuffer.Count()) * 2;
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
                if (mainBoard.audioJumpInterval == 10 || sw.Elapsed.TotalNanoseconds >= elapsedCycleTime - 60 * mainBoard.audioJumpInterval + audioFineTuning)
                {
                    if (mainBoard.audioJumpInterval == 10 || !mainBoard.cycleWait.TryDequeue(out n))
                    {
                        RunCycle();
                        
                        if (mainBoard.audioJumpInterval != 10 && soundCycles > mainBoard.audioJumpInterval + baseAudioJumpInterval)
                        {

                            if (k < bufferSize)
                            {
                                bytes[k] = (byte)(mainBoard.softswitches.SoundClick ? 0x80 : 0x00);
                            }
                            else
                            {
                                mainBoard.clickBuffer.Enqueue(bytes);
                                k = 0;
                                bytes = new byte[bufferSize];
                            }

                            k++;

                            TimeSpan delta2 = DateTime.Now - countTime;
                            if (delta2.TotalMilliseconds >= adjcycle)
                            {
                                mainBoard.screenLog.Enqueue(" Queue buffer: " + mainBoard.clickBuffer.Count()
                                 + " audioJumpInterval = " + mainBoard.audioJumpInterval + baseAudioJumpInterval
                                 + " elepsedCycleTime = " + (elapsedCycleTime - 60 * mainBoard.audioJumpInterval + audioFineTuning));

                                if (mainBoard.clickBuffer.Count() > 1)
                                {
                                    baseAudioJumpInterval += (mainBoard.clickBuffer.Count() - 1);
                                }
                                else if (mainBoard.clickBuffer.Count() < 1)
                                {
                                    baseAudioJumpInterval -= (1 - mainBoard.clickBuffer.Count());
                                }
                                if (mainBoard.clickBuffer.Count() > 1)
                                {
                                    audioFineTuning += (mainBoard.clickBuffer.Count() - 1) * 2;
                                }
                                else if (mainBoard.clickBuffer.Count() < 1)
                                {
                                    audioFineTuning -= (1 - mainBoard.clickBuffer.Count()) * 2;
                                }

                                countTime = DateTime.Now;
                            }
                            soundCycles = 0;
                        }
                        else if (mainBoard.audioJumpInterval != 10)
                        {
                            soundCycles++;
                        }
                    }
                    if (mainBoard.audioJumpInterval != 10)
                        sw = Stopwatch.StartNew();
                }
            }
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


