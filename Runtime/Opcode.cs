using Runtime.OpCodeProcessors;

namespace Runtime
{
    // addressing
    // _z   zeropage
    // _a   absolute
    // _i   indirect
    // X / Y Register

    public enum Addressing
    {
        zeropage,
        immediate,
        absolute,
        indirect,
        relative
    }

    public enum Register
    {
        X,
        Y
    }
    public static class OpCodes
    {
        public static Dictionary<byte, OpCodePart?> opCodeTable = new Dictionary<byte, OpCodePart?>()
        {
            { 0x69, new OpCodePart("ADC", 2, Addressing.immediate)},
            { 0x65, new OpCodePart("ADC", 3, Addressing.zeropage)},
            { 0x75, new OpCodePart("ADC", 4, Addressing.zeropage, Register.X)},
            { 0x6d, new OpCodePart("ADC", 4, Addressing.absolute)},
            { 0x7d, new OpCodePart("ADC", 4, Addressing.absolute, Register.X)}, //*
            { 0x79, new OpCodePart("ADC", 4, Addressing.absolute, Register.Y)}, //*
            { 0x61, new OpCodePart("ADC", 6, Addressing.indirect, Register.X)},
            { 0x71, new OpCodePart("ADC", 5, Addressing.indirect, Register.Y)}, //*
            { 0x29, new OpCodePart("AND", 2, Addressing.immediate)},
            { 0x25, new OpCodePart("AND", 3, Addressing.zeropage)},
            { 0x35, new OpCodePart("AND", 4, Addressing.zeropage, Register.X)},
            { 0x2d, new OpCodePart("AND", 4, Addressing.absolute)},
            { 0x3d, new OpCodePart("AND", 4, Addressing.absolute, Register.X)}, //*
            { 0x39, new OpCodePart("AND", 4, Addressing.absolute, Register.Y)}, //*
            { 0x21, new OpCodePart("AND", 6, Addressing.indirect, Register.X)},
            { 0x31, new OpCodePart("AND", 5, Addressing.indirect, Register.Y)}, //*
            { 0x0a, new OpCodePart("ASL", 2)},
            { 0x06, new OpCodePart("ASL", 5, Addressing.zeropage)},
            { 0x16, new OpCodePart("ASL", 6, Addressing.zeropage, Register.X)},
            { 0x0e, new OpCodePart("ASL", 6, Addressing.absolute)},
            { 0x1e, new OpCodePart("ASL", 7, Addressing.absolute, Register.X)},
            { 0x90, new OpCodePart("BCC", 2, Addressing.relative)},            //**
            { 0xb0, new OpCodePart("BCS", 2, Addressing.relative)}, //**
            { 0xf0, new OpCodePart("BEQ", 2, Addressing.relative)}, //**
            { 0x24, new OpCodePart("BIT", 3, Addressing.zeropage)},
            { 0x2c, new OpCodePart("BIT", 4, Addressing.absolute)},
            { 0x30, new OpCodePart("BMI", 2, Addressing.relative)}, //*
            { 0xd0, new OpCodePart("BNE", 2, Addressing.relative)}, //**
            { 0x10, new OpCodePart("BPL", 2, Addressing.relative)}, //**
            { 0x00, new OpCodePart("BRK", 7)},
            { 0x50, new OpCodePart("BVC", 2, Addressing.relative)}, //*
            { 0x70, new OpCodePart("BVS", 2, Addressing.relative)}, //**
            { 0x18, new OpCodePart("CLC", 2)},
            { 0xd8, new OpCodePart("CLD", 2)},
            { 0x58, new OpCodePart("CLI", 2)},
            { 0xb8, new OpCodePart("CLV", 2)},
            { 0xc9, new OpCodePart("CMP", 2, Addressing.immediate)},
            { 0xc5, new OpCodePart("CMP", 3, Addressing.zeropage)},
            { 0xd5, new OpCodePart("CMP", 4, Addressing.zeropage, Register.X)},
            { 0xcd, new OpCodePart("CMP", 4, Addressing.absolute)},
            { 0xdd, new OpCodePart("CMP", 4, Addressing.absolute, Register.X)}, //*
            { 0xd9, new OpCodePart("CMP", 4, Addressing.absolute, Register.Y)}, //*
            { 0xc1, new OpCodePart("CMP", 6, Addressing.indirect, Register.X)},
            { 0xd1, new OpCodePart("CMP", 5, Addressing.indirect, Register.Y)}, //*
            { 0xe0, new OpCodePart("CPX", 2, Addressing.immediate)},
            { 0xe4, new OpCodePart("CPX", 3, Addressing.zeropage)},
            { 0xec, new OpCodePart("CPX", 4, Addressing.absolute)},
            { 0xc0, new OpCodePart("CPY", 2, Addressing.immediate)},
            { 0xc4, new OpCodePart("CPY", 3, Addressing.zeropage)},
            { 0xcc, new OpCodePart("CPY", 4, Addressing.absolute)},
            { 0xc6, new OpCodePart("DEC", 5, Addressing.zeropage)},
            { 0xd6, new OpCodePart("DEC", 6, Addressing.zeropage, Register.X)},
            { 0xce, new OpCodePart("DEC", 6, Addressing.absolute)},
            { 0xde, new OpCodePart("DEC", 7, Addressing.absolute, Register.X)},
            { 0xca, new OpCodePart("DEX", 2)},
            { 0x88, new OpCodePart("DEY", 2)},
            { 0x49, new OpCodePart("EOR", 2, Addressing.immediate)},
            { 0x45, new OpCodePart("EOR", 3, Addressing.zeropage)},
            { 0x55, new OpCodePart("EOR", 4, Addressing.zeropage, Register.X)},
            { 0x4d, new OpCodePart("EOR", 4, Addressing.absolute)},
            { 0x5d, new OpCodePart("EOR", 4, Addressing.absolute, Register.X)}, //*
            { 0x59, new OpCodePart("EOR", 4, Addressing.absolute, Register.Y)}, //*
            { 0x41, new OpCodePart("EOR", 6, Addressing.indirect, Register.X)},
            { 0x51, new OpCodePart("EOR", 5, Addressing.indirect, Register.Y)}, //*
            { 0xe6, new OpCodePart("INC", 5, Addressing.zeropage)},
            { 0xee, new OpCodePart("INC", 6, Addressing.absolute)},
            { 0xf6, new OpCodePart("INC", 6, Addressing.zeropage, Register.X)},
            { 0xfe, new OpCodePart("INC", 7, Addressing.absolute, Register.X)},
            { 0xe8, new OpCodePart("INX", 2)},
            { 0xc8, new OpCodePart("INY", 2)},
            { 0x4c, new OpCodePart("JMP", 3, Addressing.absolute)},
            { 0x6c, new OpCodePart("JMP", 5, Addressing.indirect)},
            { 0x20, new OpCodePart("JSR", 6, Addressing.absolute)},
            { 0xa9, new OpCodePart("LDA", 2, Addressing.immediate)},
            { 0xa5, new OpCodePart("LDA", 3, Addressing.zeropage)},
            { 0xb5, new OpCodePart("LDA", 4, Addressing.zeropage, Register.X)},
            { 0xad, new OpCodePart("LDA", 4, Addressing.absolute)},
            { 0xbd, new OpCodePart("LDA", 4, Addressing.absolute, Register.X)}, //*
            { 0xb9, new OpCodePart("LDA", 4, Addressing.absolute, Register.Y)}, //*
            { 0xa1, new OpCodePart("LDA", 6, Addressing.indirect, Register.X)},
            { 0xb1, new OpCodePart("LDA", 5, Addressing.indirect, Register.Y)}, //*
            { 0xa2, new OpCodePart("LDX", 2, Addressing.immediate)},
            { 0xa6, new OpCodePart("LDX", 3, Addressing.zeropage)},
            { 0xb6, new OpCodePart("LDX", 4, Addressing.zeropage, Register.Y)},
            { 0xae, new OpCodePart("LDX", 4, Addressing.absolute)},
            { 0xbe, new OpCodePart("LDX", 4, Addressing.absolute, Register.Y)}, //*
            { 0xa0, new OpCodePart("LDY", 2, Addressing.immediate)},
            { 0xa4, new OpCodePart("LDY", 3, Addressing.zeropage)},
            { 0xb4, new OpCodePart("LDY", 4, Addressing.zeropage, Register.X)},
            { 0xac, new OpCodePart("LDY", 4, Addressing.absolute)},
            { 0xbc, new OpCodePart("LDY", 4, Addressing.absolute, Register.X)}, //*
            { 0x4a, new OpCodePart("LSR", 2)},
            { 0x46, new OpCodePart("LSR", 5, Addressing.zeropage)},
            { 0x56, new OpCodePart("LSR", 6, Addressing.zeropage, Register.X)},
            { 0x4e, new OpCodePart("LSR", 6, Addressing.absolute)},
            { 0x5e, new OpCodePart("LSR", 7, Addressing.absolute, Register.X)},
            { 0xea, new OpCodePart("NOP", 2)},
            { 0x09, new OpCodePart("ORA", 2, Addressing.immediate)},
            { 0x05, new OpCodePart("ORA", 3, Addressing.zeropage)},
            { 0x15, new OpCodePart("ORA", 4, Addressing.zeropage, Register.X)},
            { 0x0d, new OpCodePart("ORA", 4, Addressing.absolute)},
            { 0x1d, new OpCodePart("ORA", 4, Addressing.absolute, Register.X)}, //*
            { 0x19, new OpCodePart("ORA", 4, Addressing.absolute, Register.Y)}, //*
            { 0x01, new OpCodePart("ORA", 6, Addressing.indirect, Register.X)},
            { 0x11, new OpCodePart("ORA", 5, Addressing.indirect, Register.Y)}, //*
            { 0x48, new OpCodePart("PHA", 3)},
            { 0x08, new OpCodePart("PHP", 3)},
            { 0x68, new OpCodePart("PLA", 4)},
            { 0x28, new OpCodePart("PLP", 4)},
            { 0x2a, new OpCodePart("ROL", 2)},
            { 0x26, new OpCodePart("ROL", 5, Addressing.zeropage)},
            { 0x36, new OpCodePart("ROL", 6, Addressing.zeropage, Register.X)},
            { 0x2e, new OpCodePart("ROL", 6, Addressing.absolute)},
            { 0x3e, new OpCodePart("ROL", 7, Addressing.absolute, Register.X)},
            { 0x6a, new OpCodePart("ROR", 2)},
            { 0x66, new OpCodePart("ROR", 5, Addressing.zeropage)},
            { 0x76, new OpCodePart("ROR", 6, Addressing.zeropage, Register.X)},
            { 0x6e, new OpCodePart("ROR", 6, Addressing.absolute)},
            { 0x7e, new OpCodePart("ROR", 7, Addressing.absolute, Register.X)},
            { 0x40, new OpCodePart("RTI", 6)},
            { 0x60, new OpCodePart("RTS", 6)},
            { 0xe9, new OpCodePart("SBC", 2, Addressing.immediate)},
            { 0xe5, new OpCodePart("SBC", 3, Addressing.zeropage)},
            { 0xf5, new OpCodePart("SBC", 4, Addressing.zeropage, Register.X)},
            { 0xed, new OpCodePart("SBC", 4, Addressing.absolute)},
            { 0xfd, new OpCodePart("SBC", 4, Addressing.absolute, Register.X)}, //*
            { 0xf9, new OpCodePart("SBC", 4, Addressing.absolute, Register.Y)}, //*
            { 0xe1, new OpCodePart("SBC", 6, Addressing.indirect, Register.X)},
            { 0xf1, new OpCodePart("SBC", 5, Addressing.indirect, Register.Y)}, //*
            { 0x38, new OpCodePart("SEC", 2)},
            { 0xf8, new OpCodePart("SED", 2)},
            { 0x78, new OpCodePart("SEI", 2)},
            { 0x85, new OpCodePart("STA", 3, Addressing.zeropage)},
            { 0x95, new OpCodePart("STA", 4, Addressing.zeropage, Register.X)},
            { 0x8d, new OpCodePart("STA", 4, Addressing.absolute)},
            { 0x9d, new OpCodePart("STA", 5, Addressing.absolute, Register.X)},
            { 0x99, new OpCodePart("STA", 5, Addressing.absolute, Register.Y)},
            { 0x81, new OpCodePart("STA", 6, Addressing.indirect, Register.X)},
            { 0x91, new OpCodePart("STA", 6, Addressing.indirect, Register.Y)},
            { 0x86, new OpCodePart("STX", 3, Addressing.zeropage)},
            { 0x96, new OpCodePart("STX", 4, Addressing.zeropage, Register.Y)},
            { 0x8e, new OpCodePart("STX", 4, Addressing.absolute)},
            { 0x84, new OpCodePart("STY", 3, Addressing.zeropage)},
            { 0x94, new OpCodePart("STY", 4, Addressing.zeropage, Register.X)},
            { 0x8c, new OpCodePart("STY", 4, Addressing.absolute)},
            { 0xaa, new OpCodePart("TAX", 2)},
            { 0xa8, new OpCodePart("TAY", 2)},
            { 0x9a, new OpCodePart("TXS", 2)},
            { 0x8a, new OpCodePart("TXA", 2)},
            { 0xba, new OpCodePart("TSX", 2)},
            { 0x98, new OpCodePart("TYA", 2)},
            { 0x02, new OpCodePart("KIL", 1)},
            { 0x12, new OpCodePart("KIL", 1)},            
            { 0x22, new OpCodePart("KIL", 1)},
            { 0x32, new OpCodePart("KIL", 1)},            
            { 0x42, new OpCodePart("KIL", 1)},            
            { 0x52, new OpCodePart("KIL", 1)},            
            { 0x62, new OpCodePart("KIL", 1)},
            { 0x72, new OpCodePart("KIL", 1)},
            { 0x92, new OpCodePart("KIL", 1)},
            { 0xB2, new OpCodePart("KIL", 1)},
            { 0xD2, new OpCodePart("KIL", 1)},
            { 0xF2, new OpCodePart("KIL", 1)}
        };
        public static OpCodePart? GetOpCode(byte opCode)
        {
            OpCodePart? ret = null;
            try
            {
                ret = opCodeTable[opCode];
            }
            catch (Exception)
            {
                Console.WriteLine("OpCode: " + opCode.ToString("X2") + " not found.");
            }
            return ret;
        }

        public static ushort? ProcessAddressing(OpCodePart? opCodePart, State state, MainBoard mainBoard, CPU cpu)
        {
            ushort? refAddress = null;
            if (opCodePart != null)
            {
                switch (opCodePart.Addressing)
                {
                    case Addressing.immediate:
                        cpu.IncPC();
                        refAddress = state.PC;
                        cpu.IncPC();
                        break;
                    case Addressing.absolute:
                        cpu.IncPC();
                        refAddress = mainBoard.ReadAddressLLHH(state.PC);
                        if (opCodePart.Register != null)
                        {
                            if (opCodePart.Register == Register.Y)
                                refAddress = (ushort)((refAddress ?? 0) + state.Y);
                            else
                                refAddress = (ushort)((refAddress ?? 0) + state.X);
                        }
                        cpu.IncPC();
                        cpu.IncPC();
                        break;
                    case Addressing.zeropage:
                        cpu.IncPC();
                        refAddress = mainBoard.ReadZeroPageAddress(state.PC);

                        if (refAddress != null && opCodePart.Register != null)
                        {
                            if (opCodePart.Register == Register.Y)
                                refAddress = (byte)(refAddress + state.Y);
                            else
                                refAddress = (byte)(refAddress + state.X);
                        }
                        cpu.IncPC();
                        break;
                    case Addressing.indirect:
                        if (opCodePart.Register != null)
                        {
                            cpu.IncPC();
                            refAddress = mainBoard.ReadZeroPageAddress(state.PC);
                            if (refAddress != null)
                            {
                                if (opCodePart.Register == Register.Y)
                                {
                                    var pointer = mainBoard.ReadAddressLLHH(refAddress);
                                    refAddress = (ushort?)(pointer + state.Y);
                                }
                                else
                                    refAddress = mainBoard.ReadAddressLLHH((byte)((byte)refAddress + (byte)state.X));
                            }
                            cpu.IncPC();
                        }
                        else
                        {
                            cpu.IncPC();
                            var indirectAddress = mainBoard.ReadAddressLLHH(state.PC);
                            if (indirectAddress != null)
                                refAddress = mainBoard.ReadAddressLLHH(indirectAddress);
                            cpu.IncPC();
                            cpu.IncPC();
                        }
                        break;
                    case Addressing.relative:
                        cpu.IncPC();
                        var b = mainBoard.ReadByte(state.PC);
                        var offset = unchecked((sbyte)b);
                        refAddress = (ushort)(state.PC + 1 + offset);
                        cpu.IncPC();
                        break;
                    default:
                        cpu.IncPC();
                        break;
                }
            }
            else
                cpu.IncPC();
            return refAddress;
        }

        public static void Process(OpCodePart? operation, State state, MainBoard mainBoard, ushort? refAddress)
        {
            if (operation != null)
            {
                switch (operation.Operation)
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
                        LoadOpCodeProcessors.Process_LDY(state, mainBoard, refAddress ?? 0);
                        break;
                    case "LDX":
                        LoadOpCodeProcessors.Process_LDX(state, mainBoard, refAddress ?? 0);
                        break;
                    case "LDA":
                        LoadOpCodeProcessors.Process_LDA(state, mainBoard, refAddress ?? 0);
                        break;
                    case "STY":
                        StoreOpCodeProcessors.Process_STY(state, mainBoard, refAddress ?? 0);
                        break;
                    case "STX":
                        StoreOpCodeProcessors.Process_STX(state, mainBoard, refAddress ?? 0);
                        break;
                    case "STA":
                        StoreOpCodeProcessors.Process_STA(state, mainBoard, refAddress ?? 0);
                        break;
                    case "CPX":
                        CompareOpCodeProcessors.Process_CPX(state, mainBoard, refAddress ?? 0);
                        break;
                    case "CPY":
                        CompareOpCodeProcessors.Process_CPY(state, mainBoard, refAddress ?? 0);
                        break;
                    case "CMP":
                        CompareOpCodeProcessors.Process_CMP(state, mainBoard, refAddress ?? 0);
                        break;
                    case "BIT":
                        CompareOpCodeProcessors.Process_BIT(state, mainBoard, refAddress ?? 0);
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
                        IncAndDecOpCodeProcessors.Process_DEC(state, mainBoard, refAddress ?? 0);
                        break;
                    case "INC":
                        IncAndDecOpCodeProcessors.Process_INC(state, mainBoard, refAddress ?? 0);
                        break;
                    case "RTS":
                        SubRoutineOpCodeProcessors.Process_RTS(state, mainBoard);
                        break;
                    case "JSR":
                        SubRoutineOpCodeProcessors.Process_JSR(state, mainBoard, refAddress ?? 0);
                        break;
                    case "RTI":
                        SubRoutineOpCodeProcessors.Process_RTI(state, mainBoard);
                        break;
                    case "BRK":
                        SubRoutineOpCodeProcessors.Process_BRK(state, mainBoard);
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
                        ShiftAndRollOpCodeProcessors.Process_ROL(state, mainBoard, refAddress);
                        break;
                    case "ROR":
                        ShiftAndRollOpCodeProcessors.Process_ROR(state, mainBoard, refAddress);
                        break;
                    case "LSR":
                        ShiftAndRollOpCodeProcessors.Process_LSR(state, mainBoard, refAddress);
                        break;
                    case "ASL":
                        ShiftAndRollOpCodeProcessors.Process_ASL(state, mainBoard, refAddress);
                        break;
                    case "EOR":
                        BitwiseLogicOpCodeProcessors.Process_EOR(state, mainBoard, refAddress ?? 0);
                        break;
                    case "ORA":
                        BitwiseLogicOpCodeProcessors.Process_ORA(state, mainBoard, refAddress ?? 0);
                        break;
                    case "AND":
                        BitwiseLogicOpCodeProcessors.Process_AND(state, mainBoard, refAddress ?? 0);
                        break;
                    case "ADC":
                        MathOpCodeProcessors.Process_ADC(state, mainBoard, refAddress ?? 0);
                        break;
                    case "SBC":
                        MathOpCodeProcessors.Process_SBC(state, mainBoard, refAddress ?? 0);
                        break;
                    case "PHP":
                        StackOpCodeProcessors.Process_PHP(state, mainBoard);
                        break;
                    case "PLP":
                        StackOpCodeProcessors.Process_PLP(state, mainBoard);
                        break;
                    case "PLA":
                        StackOpCodeProcessors.Process_PLA(state, mainBoard);
                        break;
                    case "PHA":
                        StackOpCodeProcessors.Process_PHA(state, mainBoard);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class OpCodePart
    {
        public int Cycles { get; set; }
        public string Operation { get; set; }
        public Addressing? Addressing { get; set; }
        public Register? Register { get; set; }
        public OpCodePart(string operation, int cycles, Addressing? addressing = null, Register? register = null)
        {
            Cycles = cycles;
            Operation = operation;
            if (addressing != null)
                Addressing = addressing;
            if (register != null)
                Register = register;
        }
    }


    public enum OpCode
    {

        KIL = 0x02,
        BRK = 0x00, // IRQ vector @ $FFFE,$FFFF
        RTI = 0x40,
        NOP = 0xEA,


        //LDA
        LDA_immediate = 0xA9,
        LDA_zeropage = 0xA5,
        LDA_zeropage_X = 0xB5,
        LDA_absolute = 0xAD,
        LDA_absolute_X = 0xBD,
        LDA_absolute_Y = 0xB9,
        LDA_indirect_X = 0xA1,
        LDA_indirect_Y = 0xB1,

        //LDX
        LDX_immediate = 0xA2,
        LDX_zeropage = 0xA6,
        LDX_zeropage_Y = 0xB6,
        LDX_absolute = 0xAE,
        LDX_absolute_Y = 0xBE,

        //LDY
        LDY_immediate = 0xA0,
        LDY_zeropage = 0xA4,
        LDY_zeropage_X = 0xB4,
        LDY_absolute = 0xAC,
        LDY_absolute_X = 0xBC,

        //STA
        STA_absolute = 0x8D,
        STA_zeropage = 0x85,
        STA_zeropage_X = 0x95,
        STA_absolute_X = 0x9D,
        STA_absolute_Y = 0x99,
        STA_indirect_X = 0x81,
        STA_indirect_Y = 0x91,

        //STX
        STX_absolute = 0x8E,
        STX_zeropage = 0x86,
        STX_zeropage_Y = 0x96,

        //STY
        STY_absolute = 0x8C,
        STY_zeropage = 0x84,
        STY_zeropage_X = 0x94,


        // ADC Add Memory to Accumulator with Carry
        ADC_absolute = 0x6D,
        ADC_zeropage = 0x65,
        ADC_zeropage_X = 0x75,
        ADC_absolute_X = 0x7D,
        ADC_absolute_Y = 0x79,
        ADC_indirect_X = 0x61,
        ADC_indirect_Y = 0x71,
        ADC_immediate = 0x69,
        // AND
        AND_immediate = 0x29,
        AND_zeropage = 0x25,
        AND_zeropage_X = 0x35,
        AND_absolute = 0x2D,
        AND_absolute_X = 0x3D,
        AND_absolute_Y = 0x39,
        AND_indirect_X = 0x21,
        AND_indirect_Y = 0x31,
        // ASL
        ASL = 0x0A,
        ASL_zeropage = 0x06,
        ASL_zeropage_X = 0x16,
        ASL_absolute = 0x0E,
        ASL_absolute_X = 0x1E,
        // BCC
        BCC_relative = 0x90,
        // BCS
        BCS_relative = 0xB0,
        // BEQ
        BEQ_relative = 0xF0,
        // BNE
        BNE_relative = 0xD0,
        // BMI
        BMI_relative = 0x30,
        // BPL
        BPL_relative = 0x10,
        //BVC
        BVC_relative = 0x50,
        //BVS
        BVS_relative = 0x70,


        // Stack
        PHA = 0x48,
        PLA = 0x68,
        PHP = 0x08,
        PLP = 0x28,
        //Transfer
        TAX = 0xAA,
        TAY = 0xA8,
        TSX = 0xBA,
        TXA = 0x8A,
        TXS = 0x9A,
        TYA = 0x98,

        //BIT
        BIT_zeropage = 0x24,
        BIT_absolute = 0x2C,

        //FLAGS
        SEC = 0x38,
        SED = 0xF8,
        SEI = 0x78,
        CLC = 0x18,
        CLD = 0xD8,
        CLI = 0x58,
        CLV = 0xB8,

        //JUMP
        JMP_absolute = 0x4C,
        JMP_indirect = 0x6C,
        JSR_absolute = 0x20,
        RTS = 0x60,

        // Subtract with Borrow
        SBC_immediate = 0xE9,
        SBC_zeropage = 0xE5,
        SBC_zeropage_X = 0xF5,
        SBC_absolute = 0xED,
        SBC_absolute_X = 0xFD,
        SBC_absolute_Y = 0xF9,
        SBC_indirect_X = 0xE1,
        SBC_indirect_Y = 0xF1,

        //ROL
        ROL = 0x2A,
        ROL_zeropage = 0x26,
        ROL_zeropage_X = 0x36,
        ROL_absolute = 0x2E,
        ROL_absolute_X = 0X3E,

        //ROR
        ROR = 0x6A,
        ROR_zeropage = 0x66,
        ROR_zeropage_X = 0x76,
        ROR_absolute = 0x6E,
        ROR_ABSOLUTE_X = 0X7E,

        //LSR
        LSR = 0x4A,
        LSR_zeropage = 0x46,
        LSR_zeropage_X = 0x56,
        LSR_absolute = 0x4E,
        LSR_absolute_X = 0X5E,

        // CMP
        CMP_immediate = 0xC9,
        CMP_zeropage = 0xC5,
        CMP_zeropage_X = 0xD5,
        CMP_absolute = 0xCD,
        CMP_absolute_X = 0xDD,
        CMP_absolute_Y = 0xD9,
        CMP_indirect_X = 0xC1,
        CMP_indirect_Y = 0xD1,

        // CPX
        CPX_immediate = 0xE0,
        CPX_zeropage = 0xE4,
        CPX_absolute = 0xEC,

        // CPY
        CPY_immediate = 0xC0,
        CPY_zeropage = 0xC4,
        CPY_absolute = 0xCC,

        // DEC
        DEC_zeropage = 0xC6,
        DEC_zeropage_X = 0xD6,
        DEC_absolute = 0xCE,
        DEC_absolute_X = 0xDE,

        // Registers Dec/Inc
        DEX = 0xCA,
        DEY = 0x88,
        INX = 0xE8,
        INY = 0xC8,

        // INC
        INC_zeropage = 0xE6,
        INC_zeropage_X = 0xF6,
        INC_absolute = 0xEE,
        INC_absolute_X = 0xFE,

        // EOR
        EOR_immediate = 0x49,
        EOR_zeropage = 0x45,
        EOR_zeropage_X = 0x55,
        EOR_absolute = 0x4D,
        EOR_absolute_X = 0x5D,
        EOR_absolute_Y = 0x59,
        EOR_indirect_X = 0x41,
        EOR_indirect_Y = 0x51,

        // ORA
        ORA_immediate = 0x09,
        ORA_zeropage = 0x05,
        ORA_zeropage_X = 0x15,
        ORA_absolute = 0x0D,
        ORA_absolute_X = 0x1D,
        ORA_absolute_Y = 0x19,
        ORA_indirect_X = 0x01,
        ORA_indirect_Y = 0x11
    }
}
