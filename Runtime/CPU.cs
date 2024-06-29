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
        memory.cpuCycles++;
        if (memory.adjust1Mhz)
        {
            for (int i = 0; i < this.deleyloops; i++)
                ;
        }
    }

    public void RunCycle()
    {
        byte instruction = memory.ReadByte(state.PC);
        OpCodePart? opCodePart = OpCodes.GetOpCode(instruction);
        ushort? refAddress = null;
        if (opCodePart != null)
        {
            switch (opCodePart.Addressing)
            {
                case Addressing.immediate:
                    IncPC();
                    refAddress = state.PC;
                    IncPC();
                    break;
                case Addressing.absolute:
                    IncPC();
                    refAddress = memory.ReadAddressLLHH(state.PC);
                    if (opCodePart.Register != null)
                    {
                        if (opCodePart.Register == Register.Y)
                            refAddress = (ushort)((refAddress ?? 0) + state.Y);
                        else
                            refAddress = (ushort)((refAddress ?? 0) + state.X);
                    }
                    IncPC();
                    IncPC();
                    break;
                case Addressing.zeropage:
                    IncPC();
                    refAddress = memory.ReadZeroPageAddress(state.PC);

                    if (refAddress != null && opCodePart.Register != null)
                    {
                        if (opCodePart.Register == Register.Y)
                            refAddress = (byte)(refAddress + state.Y);
                        else
                            refAddress = (byte)(refAddress + state.X);
                    }
                    IncPC();
                    break;
                case Addressing.indirect:
                    if (opCodePart.Register != null)
                    {
                        IncPC();
                        refAddress = memory.ReadZeroPageAddress(state.PC);
                        if (refAddress != null)
                        {
                            if (opCodePart.Register == Register.Y)
                            {
                                var pointer = memory.ReadAddressLLHH(refAddress);
                                refAddress = (ushort?)(pointer + state.Y);
                            }
                            else
                                refAddress = memory.ReadAddressLLHH((byte)((byte)refAddress + (byte)state.X));
                        }
                        IncPC();
                    }
                    else
                    {
                        IncPC();
                        var indirectAddress = memory.ReadAddressLLHH(state.PC);
                        if (indirectAddress != null)
                            refAddress = memory.ReadAddressLLHH(indirectAddress);
                        IncPC();
                        IncPC();
                    }
                    break;
                case Addressing.relative:
                    IncPC();
                    var b = memory.ReadByte(state.PC);
                    var offset = unchecked((sbyte)b);
                    refAddress = (ushort)(state.PC + 1 + offset);
                    IncPC();
                    break;
                default:
                    IncPC();
                    break;
            }
            switch (opCodePart.Operation)
            {
                case "CLC":
                    FlagOpCodeProcessors.Process_CLC(state);
                    break;
                case "SEI":
                    FlagOpCodeProcessors.Process_SEI(state);
                    break;
                case "CLV":
                    FlagOpCodeProcessors.Process_CLV(state);
                    break;
                case "CLI":
                    FlagOpCodeProcessors.Process_CLI(state);
                    break;
                case "SEC":
                    FlagOpCodeProcessors.Process_SEC(state);
                    break;
                case "SED":
                    FlagOpCodeProcessors.Process_SED(state);
                    break;
                case "CLD":
                    FlagOpCodeProcessors.Process_CLD(state);
                    break;
                case "LDY":
                    LoadOpCodeProcessors.Process_LDY(state, memory, refAddress ?? 0);
                    break;
                case "LDX":
                    LoadOpCodeProcessors.Process_LDX(state, memory, refAddress ?? 0);
                    break;
                case "LDA":
                    LoadOpCodeProcessors.Process_LDA(state, memory, refAddress ?? 0);
                    break;
                case "STY":
                    StoreOpCodeProcessors.Process_STY(state, memory, refAddress ?? 0);
                    break;
                case "STX":
                    StoreOpCodeProcessors.Process_STX(state, memory, refAddress ?? 0);
                    break;
                case "STA":
                    StoreOpCodeProcessors.Process_STA(state, memory, refAddress ?? 0);
                    break;
                case "CPX":
                    CompareOpCodeProcessors.Process_CPX(state, memory, refAddress ?? 0);
                    break;
                case "CPY":
                    CompareOpCodeProcessors.Process_CPY(state, memory, refAddress ?? 0);
                    break;
                case "CMP":
                    CompareOpCodeProcessors.Process_CMP(state, memory, refAddress ?? 0);
                    break;
                case "BIT":
                    CompareOpCodeProcessors.Process_BIT(state, memory, refAddress ?? 0);
                    break;
                case "BPL":
                    BranchOpCodeProcessors.Process_BPL(state, refAddress ?? 0);
                    break;
                case "BVC":
                    BranchOpCodeProcessors.Process_BVC(state, refAddress ?? 0);
                    break;
                case "BVS":
                    BranchOpCodeProcessors.Process_BVS(state, refAddress ?? 0);
                    break;
                case "BMI":
                    BranchOpCodeProcessors.Process_BMI(state, refAddress ?? 0);
                    break;
                case "BCS":
                    BranchOpCodeProcessors.Process_BCS(state, refAddress ?? 0);
                    break;
                case "BNE":
                    BranchOpCodeProcessors.Process_BNE(state, refAddress ?? 0);
                    break;
                case "BCC":
                    BranchOpCodeProcessors.Process_BCC(state, refAddress ?? 0);
                    break;
                case "BEQ":
                    BranchOpCodeProcessors.Process_BEQ(state, refAddress ?? 0);
                    break;
                case "JMP":
                    BranchOpCodeProcessors.Process_JMP(state, refAddress ?? 0);
                    break;
                case "DEX":
                    IncAndDecOpCodeProcessors.Process_DEX(state);
                    break;
                case "DEY":
                    IncAndDecOpCodeProcessors.Process_DEY(state);
                    break;
                case "INX":
                    IncAndDecOpCodeProcessors.Process_INX(state);
                    break;
                case "INY":
                    IncAndDecOpCodeProcessors.Process_INY(state);
                    break;
                case "DEC":
                    IncAndDecOpCodeProcessors.Process_DEC(state, memory, refAddress ?? 0);
                    break;
                case "INC":
                    IncAndDecOpCodeProcessors.Process_INC(state, memory, refAddress ?? 0);
                    break;
                case "RTS":
                    SubRoutineOpCodeProcessors.Process_RTS(state, memory);
                    break;
                case "JSR":
                    SubRoutineOpCodeProcessors.Process_JSR(state, memory, refAddress ?? 0);
                    break;
                case "RTI":
                    SubRoutineOpCodeProcessors.Process_RTI(state, memory);
                    break;
                case "BRK":
                    SubRoutineOpCodeProcessors.Process_BRK(state, memory);
                    break;
                case "TYA":
                    TransferOpCodeProcessors.Process_TYA(state);
                    break;
                case "TXS":
                    TransferOpCodeProcessors.Process_TXS(state);
                    break;
                case "TXA":
                    TransferOpCodeProcessors.Process_TXA(state);
                    break;
                case "TSX":
                    TransferOpCodeProcessors.Process_TSX(state);
                    break;
                case "TAX":
                    TransferOpCodeProcessors.Process_TAX(state);
                    break;
                case "TAY":
                    TransferOpCodeProcessors.Process_TAY(state);
                    break;
                case "ROL":
                    ShiftAndRollOpCodeProcessors.Process_ROL(state, memory, refAddress);
                    break;
                case "ROR":
                    ShiftAndRollOpCodeProcessors.Process_ROR(state, memory, refAddress);
                    break;
                case "LSR":
                    ShiftAndRollOpCodeProcessors.Process_LSR(state, memory, refAddress);
                    break;
                case "ASL":
                    ShiftAndRollOpCodeProcessors.Process_ASL(state, memory, refAddress);
                    break;
                case "EOR":
                    BitwiseLogicOpCodeProcessors.Process_EOR(state, memory, refAddress ?? 0);
                    break;
                case "ORA":
                    BitwiseLogicOpCodeProcessors.Process_ORA(state, memory, refAddress ?? 0);
                    break;
                case "AND":
                    BitwiseLogicOpCodeProcessors.Process_AND(state, memory, refAddress ?? 0);
                    break;
                case "ADC":
                    MathOpCodeProcessors.Process_ADC(state, memory, refAddress ?? 0);
                    break;
                case "SBC":
                    MathOpCodeProcessors.Process_SBC(state, memory, refAddress ?? 0);
                    break;
                case "PHP":
                    StackOpCodeProcessors.Process_PHP(state, memory);
                    break;
                case "PLP":
                    StackOpCodeProcessors.Process_PLP(state, memory);
                    break;
                case "PLA":
                    StackOpCodeProcessors.Process_PLA(state, memory);
                    break;
                case "PHA":
                    StackOpCodeProcessors.Process_PHA(state, memory);
                    break;
                default:
                    break;
            }
        }
        else
            IncPC();
    }

    public void DelayedRun(bool running)
    {
        int countFreq = 0;
        DateTime lastBlock = DateTime.Now;
        DateTime countTime = DateTime.Now;
        DateTime cpuDelay = DateTime.Now;
        memory.cpuCycles = 0;
        int soundCycles = 0;
        Stopwatch sw2;
        Stopwatch sw3 = Stopwatch.StartNew();
        float cycleTotalTime = 2.8f;
        int countOnCycles = 0;

        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < 100000000; i++)
            ;
        sw.Stop();

        Console.WriteLine("1000000 in " + sw.Elapsed.TotalMicroseconds.ToString("000000.0") + " microseconds ");
        double microsecondLoops = (int)(100000000f / sw.Elapsed.TotalMicroseconds) - 50;
        Console.WriteLine(microsecondLoops + " loops per microsecond");
        //Thread.Sleep(5000);
        while (running)
        {
            if (memory.adjust1Mhz)
            {
                sw2 = Stopwatch.StartNew();
                sw = Stopwatch.StartNew();
                deleyloops = cycleTotalTime * microsecondLoops;
                RunCycle();
                sw.Stop();

                double elepsedCycleTime = (cycleTotalTime - sw.Elapsed.TotalMicroseconds) * microsecondLoops;

                for (int i = 0; i < (elepsedCycleTime > 0 ? elepsedCycleTime : 0); i++)
                    ;
                sw2.Stop();

                if (soundCycles > 6)
                {
                    countFreq++;
                    if (memory.clickEvent.Count() < 20000)
                    {
                        if (memory.softswitches.SoundClick)
                        {
                            memory.clickEvent.Enqueue(0x80);
                            countOnCycles++;
                        }
                        else
                        {
                            countOnCycles = 0;
                            memory.clickEvent.Enqueue(0);
                        }
                    }
                    // Sound routine

                    TimeSpan delta2 = DateTime.Now - countTime;
                    if (delta2.TotalMilliseconds >= 1000)
                    {
                        Console.WriteLine("Sound Cycle = " + countFreq
                         + "Hz, Empty Queue = " + memory.EmptyQueue);

                        countFreq = 0;
                        countTime = DateTime.Now;
                        memory.EmptyQueue = 0;
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


