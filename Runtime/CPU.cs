using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Runtime.OpCodeProcessors;

namespace Runtime;

public class CPU
{
    
    public State state { get; set; }
    public Memory memory { get; set; }

    public ushort m53266 { get; set; }
    
    public CPU(State state, Memory memory)
    {
        this.state = state;
        this.memory = memory;
        memory.RegisterOverlay(new AppleScreen());
        memory.RegisterOverlay(new Keyboard());
    }

    public void Reset()
    {
        state.PC = 0;
    }

    public void IncrementPC()
    {
        state.PC++;
    }

    public void RunCycle()
    {
        if (state.PC == 0)  
        {
            state.PC = memory.ReadAddressLLHH(0xfffc) ?? 0;
        }   

        byte instruction = memory.ReadByte(state.PC);
        OpCodePart? opCodePart = OpCodes.GetOpCode(instruction);
        ushort? refAddress = null;
        
        string pc = state.PC.ToString("X");
        
        if (pc == "FFBC")
                Thread.Sleep(1);


        if (opCodePart != null)
        {
            switch (opCodePart.Addressing)
            {
                case "immediate":
                    state.PC++;
                    refAddress = state.PC;
                    state.PC++;
                    break;
                case "absolute":
                    state.PC++;
                    refAddress = memory.ReadAddressLLHH(state.PC);
                    if (opCodePart.Register != null)
                    {
                        if (opCodePart.Register=="Y")
                        {
                            refAddress = (ushort)(refAddress + state.Y);
                        }
                        else
                        {
                            refAddress = (ushort)(refAddress + state.X);
                        }
                    }
                    state.PC++;
                    state.PC++;
                    break;
                case "zeropage":
                    state.PC++;
                    refAddress = memory.ReadZeroPageAddress(state.PC);
                    if (refAddress != null && opCodePart.Register != null)
                    {
                        if (opCodePart.Register=="Y")
                        {
                            refAddress = (ushort)(refAddress + state.Y);
                        }
                        else
                        {
                            refAddress = (ushort)(refAddress + state.X);
                        }
                    }
                    state.PC++;
                    break;
                case "indirect":
                    if (opCodePart.Register != null)
                    {
                        state.PC++;
                        refAddress = memory.ReadZeroPageAddress(state.PC);
                        if (refAddress != null)
                        {
                            if (opCodePart.Register=="Y")
                            {
                                var pointer = memory.ReadZeroPageAddress(refAddress);
                                refAddress = (ushort?)(pointer + state.Y);
                            }
                            else
                            {
                                refAddress = memory.ReadZeroPageAddress((ushort)(refAddress + state.X));
                            }
                        }
                        state.PC++;
                    }
                    else
                    {
                        state.PC++;
                        var indirectAddress = memory.ReadAddressLLHH(state.PC);
                        if (indirectAddress != null)
                            refAddress = memory.ReadAddressHHLL(indirectAddress);
                        state.PC++;
                        state.PC++;
                    }
                    break;
                case "relative":
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


            string op = opCodePart?.Operation + "_" + opCodePart?.Addressing  + "_" + opCodePart?.Register;
            string ac = state.A + " " + state.X + " " + state.Y;
            string fl = (state.N ? "1" : "0") + "" + (state.V ? "1" : "0") + (state.B ? "1": "0") + (state.D ? "1": "0") + (state.I ? "1": "0") + (state.Z ? "1": "0") + (state.C ? "1": "0");
            string ad = refAddress?.ToString("X");

            


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

         
}   


