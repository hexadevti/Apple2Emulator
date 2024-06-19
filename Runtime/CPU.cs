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




    public bool debug = false;

    public bool cursorInverted = false;


    public string pc { get; set; }
    public string op { get; set; }
    public string axy { get; set; }
    public string fl { get; set; }
    public const string fh = "NVBDIZC";
    public string ad { get; set; }
    public string inst { get; set; }

    public ushort lastPC = 0;
    public int PCCount = 0;
    public DateTime last1mhz = DateTime.MinValue;
    public DateTime actual1mhz = DateTime.MinValue;

    public int delayCycle = 0;

    public bool adjust1Mhz = false;


    public CPU(State state, Memory memory, bool debug = false)
    {
        this.memory = memory;
        this.debug = debug;
        this.state = state;

        pc = ""; op = ""; axy = ""; fl = ""; ad = ""; inst = "";

        this.memory.softswitches = new Softswitches()
        {
            Graphics_Text = false
        };
        last1mhz = DateTime.Now;
    }

    public void Reset()
    {
        lastPC = 0;
        state.PC = 0;
        state.PC = memory.ReadAddressLLHH(0xfffc) ?? 0;
    }

    public void InitConsole()
    {
        Console.CursorVisible = false;
        Console.WindowHeight = 700;
    }

    public void IncPC()
    {
        lastPC = state.PC;
        state.PC++;
        if (PCCount == 1000000)
        {
            PCCount = 0;
            TimeSpan cycle = DateTime.Now - last1mhz;
            if (adjust1Mhz)
            {
                if (cycle.TotalMilliseconds < 1000)
                    delayCycle += Convert.ToInt16((1000 - cycle.TotalMilliseconds) / 2);
                else
                {
                    delayCycle -= Convert.ToInt16((cycle.TotalMilliseconds - 1000) / 2);
                    if (delayCycle < 0)
                        delayCycle = 0;
                }
            }
            else
                delayCycle = 0;
            last1mhz = actual1mhz = DateTime.Now;
            Console.WriteLine(cycle.TotalMilliseconds);
        }
        PCCount++;
        for (int i = 0; i < delayCycle; i++)
        {
            var a = i;
        }

    }

    public void RunCycle(bool adjust1mhz)
    {
        adjust1Mhz = adjust1mhz;
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

    public void RefreshScreen()
    {
        Console.SetCursorPosition(0, 0);

        var cursorH = memory.baseRAM[0x24];
        var cursorV = memory.baseRAM[0x25];

        StringBuilder output = new StringBuilder();

        int posH = 0;
        int posV = 0;

        for (int b = 0; b < 3; b++)
        {
            posV = b * 8;
            for (int l = 0; l < 8; l++)
            {

                for (ushort c = 0; c < 0x28; c++)
                {
                    posH = c;

                    var chr = memory.baseRAM[(ushort)(0x400 + (b * 0x28) + (l * 0x80) + c)];
                    chr = (byte)(chr & 0b01111111);
                    if (posV == cursorV && posH == cursorH)
                        chr = DateTime.Now.Millisecond > 500 ? chr : (byte)95;

                    if (chr == 96)
                        chr = DateTime.Now.Millisecond > 500 ? (byte)32 : (byte)95;
                    output.Append(Encoding.ASCII.GetString(new[] { chr }));

                }
                posV = posV + 1;
                output.Append("\n");
            }
        }

        Console.Write(output.ToString());
    }

    public void Keyboard()
    {
        if (Console.KeyAvailable)
        {
            var consoleKeyInfo = Console.ReadKey(true);

            switch (consoleKeyInfo.Modifiers)
            {
                case ConsoleModifiers.Alt:
                    switch (consoleKeyInfo.Key)
                    {
                        case ConsoleKey.C:
                            memory.KeyPressed = 0x83;
                            break;
                        case ConsoleKey.F12:
                            state.PC = 0;
                            Console.Beep();
                            break;
                    }
                    break;

                default:
                    switch (consoleKeyInfo.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            memory.KeyPressed = 0x88;
                            break;
                        case ConsoleKey.RightArrow:
                            memory.KeyPressed = 0x95;
                            break;
                        case ConsoleKey.UpArrow:
                            memory.KeyPressed = 0x8b;
                            break;
                        case ConsoleKey.DownArrow:
                            memory.KeyPressed = 0x8a;
                            break;
                        case ConsoleKey.Enter:
                            memory.KeyPressed = 0x8D;
                            break;
                        case ConsoleKey.Escape:
                            memory.KeyPressed = 0x9b;
                            break;
                        default:
                            switch (consoleKeyInfo.KeyChar)
                            {
                                case (char)231:
                                    memory.KeyPressed = 0x83;
                                    break;
                                default:
                                    memory.KeyPressed = (byte)(Encoding.ASCII.GetBytes(new[] { consoleKeyInfo.KeyChar.ToString().ToUpper()[0] })[0] | 0b10000000);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}


