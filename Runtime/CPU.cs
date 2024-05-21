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
    public string ad { get; set;}
    public string inst { get; set; }

    public CPU(State state, Memory memory, bool debug = false)
    {
        this.memory = memory;
        this.debug = debug;
        this.state = state;
        
        pc = ""; op = ""; axy = ""; fl=""; ad = ""; inst = "";
        
        this.memory.softswitches = new Softswitches() {
            Graphics_Text = false
        };
        
    }

    public void Reset()
    {
        state.PC = 0;
    }

    public void InitConsole()
    {
        Console.CursorVisible = false;
    }
    
    public void RunCycle()
    {
        if (state.PC == 0)  
        {
            state.PC = memory.ReadAddressLLHH(0xfffc) ?? 0;
        }   

        byte instruction = memory.ReadByte(state.PC);
        OpCodePart? opCodePart = OpCodes.GetOpCode(instruction);
        if (debug)
        {
            op = opCodePart?.Operation + (opCodePart?.Addressing != null ? "_" + opCodePart?.Addressing : "") + (opCodePart?.Register != null ? "_" + opCodePart?.Register : "");
            pc = state.PC.ToString("x");
            axy = state.A.ToString("X") + " " + state.X.ToString("X") + " " + state.Y.ToString("X");
            fl = (state.N ? "1" : "0") + "" + (state.V ? "1" : "0") + (state.B ? "1": "0") 
                + (state.D ? "1": "0") + (state.I ? "1": "0") + (state.Z ? "1": "0") 
                + (state.C ? "1": "0");
            inst = pc + ": " + instruction.ToString("x");
            ushort nextbytes = 0;
            if (opCodePart?.Addressing == Addressing.immediate)
                nextbytes = 1;
            if (opCodePart?.Addressing == Addressing.absolute)
                nextbytes = 2;
            if (opCodePart?.Addressing == Addressing.zeropage)
                nextbytes = 1;
            if (opCodePart?.Addressing == Addressing.indirect && opCodePart.Register != null)
                nextbytes = 1;
            if (opCodePart?.Addressing == Addressing.indirect && opCodePart.Register == null)
                nextbytes = 2;
            if (opCodePart?.Addressing == Addressing.relative)
                nextbytes = 1;
            for (int i = 1;i<=nextbytes;i++)
            {
                inst = inst + " " + memory.ReadByte((ushort)(state.PC+i)).ToString("x");
            }
            if (pc == "d820" || pc == "d838")
            {
                state.I = true;
                state.B = true;
                Thread.Sleep(1);
            }

        var a1l = memory.ReadByte(0xd012);

        } 
           
        ushort? refAddress = null;
        
        
        if (opCodePart != null)
        {
            switch (opCodePart.Addressing)
            {
                case Addressing.immediate:
                    state.PC++;
                    refAddress = state.PC;
                    state.PC++;
                    break;
                case Addressing.absolute:
                    state.PC++;
                    refAddress = memory.ReadAddressLLHH(state.PC);
                    if (opCodePart.Register != null)
                    {
                        if (opCodePart.Register== Register.Y)
                        {
                            refAddress = (ushort)((refAddress ?? 0) + state.Y);
                        }
                        else
                        {
                            refAddress = (ushort)((refAddress ?? 0) + state.X);
                        }
                    }
                    state.PC++;
                    state.PC++;
                    break;
                case Addressing.zeropage:
                    state.PC++;
                    refAddress = memory.ReadZeroPageAddress(state.PC);
                    
                    if (refAddress != null && opCodePart.Register != null)
                    {
                        if (opCodePart.Register== Register.Y)
                        {
                            refAddress = (byte)(refAddress + state.Y);
                        }
                        else
                        {
                            refAddress = (byte)(refAddress + state.X);
                        }
                    }
                    state.PC++;
                    break;
                case Addressing.indirect:
                    if (opCodePart.Register != null)
                    {
                        state.PC++;
                        refAddress = memory.ReadZeroPageAddress(state.PC);
                        if (refAddress != null)
                        {
                            if (opCodePart.Register==Register.Y)
                            {
                                var pointer = memory.ReadAddressLLHH(refAddress);
                                refAddress = (ushort?)(pointer + state.Y);
                            }
                            else
                            {
                                refAddress = memory.ReadAddressLLHH((ushort)(refAddress + state.X));
                            }
                        }
                        state.PC++;
                    }
                    else
                    {
                        state.PC++;
                        var indirectAddress = memory.ReadAddressLLHH(state.PC);
                        if (indirectAddress != null)
                            refAddress = memory.ReadAddressLLHH(indirectAddress);
                        state.PC++;
                        state.PC++;
                    }
                    break;
                case Addressing.relative:
                    state.PC++;
                    var b = memory.ReadByte(state.PC);
                    var offset = unchecked((sbyte)b);
                    refAddress = (ushort)(state.PC + 1 + offset);
                    state.PC++;
                    break;
                default:
                    state.PC++;    
                    break;
            }

           

            if (debug)
                ad = (refAddress.HasValue ? refAddress.Value.ToString("x") : "null");

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
        {
            state.PC++;
        }
    }

    public void RefreshScreen()
    {
        Console.SetCursorPosition(0,0);

        var cursorH = memory.memory[0x24];
        var cursorV = memory.memory[0x25];
        
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
                    
                    var chr = memory.memory[(ushort)(0x400 + (b * 0x28) + (l * 0x80) + c)];
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
                            memory.KeyPressed = (byte)(Encoding.ASCII.GetBytes( new[] { consoleKeyInfo.KeyChar.ToString().ToUpper()[0]})[0] | 0b10000000);
                            break;
                    }
                    break;
            }
        }
    }
}   


