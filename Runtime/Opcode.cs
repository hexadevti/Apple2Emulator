namespace Runtime
{
    // addressing
    // _z   zeropage
    // _a   absolute
    // _i   indirect
    // X / Y Register
    public static class OpCodes 
    {
        public static Dictionary<byte, OpCodePart> opCodeTable = new Dictionary<byte, OpCodePart>()
        {
            { 0x00, new OpCodePart("BRK")},
            { 0x01, new OpCodePart("ORA", "indirect", "X")},
            { 0x02, new OpCodePart("KIL")},
            { 0x05, new OpCodePart("ORA", "zeropage")},
            { 0x06, new OpCodePart("ASL", "zeropage")},
            { 0x08, new OpCodePart("PHP")},
            { 0x09, new OpCodePart("ORA", "immediate")},
            { 0x0a, new OpCodePart("ASL")},
            { 0x0d, new OpCodePart("ORA", "absolute")},
            { 0x0e, new OpCodePart("ASL", "absolute")},
            { 0x10, new OpCodePart("BPL", "relative")},
            { 0x11, new OpCodePart("ORA", "indirect", "Y")},
            { 0x15, new OpCodePart("ORA", "zeropage", "X")},
            { 0x16, new OpCodePart("ASL", "zeropage", "X")},
            { 0x18, new OpCodePart("CLC")},
            { 0x19, new OpCodePart("ORA", "absolute", "Y")},
            { 0x1d, new OpCodePart("ORA", "absolute", "X")},
            { 0x1e, new OpCodePart("ASL", "absolute", "X")},
            { 0x20, new OpCodePart("JSR", "absolute")},
            { 0x21, new OpCodePart("AND", "indirect", "X")},
            { 0x24, new OpCodePart("BIT", "zeropage")},
            { 0x25, new OpCodePart("AND", "zeropage")},
            { 0x26, new OpCodePart("ROL", "zeropage")},
            { 0x28, new OpCodePart("PLP")},
            { 0x29, new OpCodePart("AND", "immediate")},
            { 0x2a, new OpCodePart("ROL")},
            { 0x2c, new OpCodePart("BIT", "absolute")},
            { 0x2d, new OpCodePart("AND", "absolute")},
            { 0x2e, new OpCodePart("ROL", "absolute")},
            { 0x30, new OpCodePart("BMI", "relative")},
            { 0x31, new OpCodePart("AND", "indirect", "Y")},
            { 0x35, new OpCodePart("AND", "zeropage", "X")},
            { 0x36, new OpCodePart("ROL", "zeropage", "X")},
            { 0x38, new OpCodePart("SEC")},
            { 0x39, new OpCodePart("AND", "absolute", "Y")},
            { 0x3d, new OpCodePart("AND", "absolute", "X")},
            { 0x3e, new OpCodePart("ROL", "absolute", "X")},
            { 0x40, new OpCodePart("RTI")},
            { 0x41, new OpCodePart("EOR", "indirect", "X")},
            { 0x45, new OpCodePart("EOR", "zeropage")},
            { 0x46, new OpCodePart("LSR", "zeropage")},
            { 0x48, new OpCodePart("PHA")},
            { 0x49, new OpCodePart("EOR", "immediate")},
            { 0x4a, new OpCodePart("LSR")},
            { 0x4c, new OpCodePart("JMP", "absolute")},
            { 0x4d, new OpCodePart("EOR", "absolute")},
            { 0x4e, new OpCodePart("LSR", "absolute")},
            { 0x50, new OpCodePart("BVC", "relative")},
            { 0x51, new OpCodePart("EOR", "indirect", "Y")},
            { 0x55, new OpCodePart("EOR", "zeropage", "X")},
            { 0x56, new OpCodePart("LSR", "zeropage", "X")},
            { 0x58, new OpCodePart("CLI")},
            { 0x59, new OpCodePart("EOR", "absolute", "Y")},
            { 0x5d, new OpCodePart("EOR", "absolute", "X")},
            { 0x5e, new OpCodePart("LSR", "absolute", "X")},
            { 0x60, new OpCodePart("RTS")},
            { 0x61, new OpCodePart("ADC", "indirect", "X")},
            { 0x65, new OpCodePart("ADC", "zeropage")},
            { 0x66, new OpCodePart("ROR", "zeropage")},
            { 0x68, new OpCodePart("PLA")},
            { 0x69, new OpCodePart("ADC", "immediate")},
            { 0x6a, new OpCodePart("ROR")},
            { 0x6c, new OpCodePart("JMP", "indirect")},
            { 0x6d, new OpCodePart("ADC", "absolute")},
            { 0x6e, new OpCodePart("ROR", "absolute")},
            { 0x70, new OpCodePart("BVS", "relative")},
            { 0x71, new OpCodePart("ADC", "indirect", "Y")},
            { 0x75, new OpCodePart("ADC", "zeropage", "X")},
            { 0x76, new OpCodePart("ROR", "zeropage", "X")},
            { 0x78, new OpCodePart("SEI")},
            { 0x79, new OpCodePart("ADC", "absolute", "Y")},
            { 0x7d, new OpCodePart("ADC", "absolute", "X")},
            { 0x7e, new OpCodePart("ROR", "ABSOLUTE", "X")},
            { 0x81, new OpCodePart("STA", "indirect", "X")},
            { 0x84, new OpCodePart("STY", "zeropage")},
            { 0x85, new OpCodePart("STA", "zeropage")},
            { 0x86, new OpCodePart("STX", "zeropage")},
            { 0x88, new OpCodePart("DEY")},
            { 0x8a, new OpCodePart("TXA")},
            { 0x8c, new OpCodePart("STY", "absolute")},
            { 0x8d, new OpCodePart("STA", "absolute")},
            { 0x8e, new OpCodePart("STX", "absolute")},
            { 0x90, new OpCodePart("BCC", "relative")},
            { 0x91, new OpCodePart("STA", "indirect", "Y")},
            { 0x94, new OpCodePart("STY", "zeropage", "X")},
            { 0x95, new OpCodePart("STA", "zeropage", "X")},
            { 0x96, new OpCodePart("STX", "zeropage", "Y")},
            { 0x98, new OpCodePart("TYA")},
            { 0x99, new OpCodePart("STA", "absolute", "Y")},
            { 0x9a, new OpCodePart("TXS")},
            { 0x9d, new OpCodePart("STA", "absolute", "X")},
            { 0xa0, new OpCodePart("LDY", "immediate")},
            { 0xa1, new OpCodePart("LDA", "indirect", "X")},
            { 0xa2, new OpCodePart("LDX", "immediate")},
            { 0xa4, new OpCodePart("LDY", "zeropage")},
            { 0xa5, new OpCodePart("LDA", "zeropage")},
            { 0xa6, new OpCodePart("LDX", "zeropage")},
            { 0xa8, new OpCodePart("TAY")},
            { 0xa9, new OpCodePart("LDA", "immediate")},
            { 0xaa, new OpCodePart("TAX")},
            { 0xac, new OpCodePart("LDY", "absolute")},
            { 0xad, new OpCodePart("LDA", "absolute")},
            { 0xae, new OpCodePart("LDX", "absolute")},
            { 0xb0, new OpCodePart("BCS", "relative")},
            { 0xb1, new OpCodePart("LDA", "indirect", "Y")},
            { 0xb4, new OpCodePart("LDY", "zeropage", "X")},
            { 0xb5, new OpCodePart("LDA", "zeropage", "X")},
            { 0xb6, new OpCodePart("LDX", "zeropage", "Y")},
            { 0xb8, new OpCodePart("CLV")},
            { 0xb9, new OpCodePart("LDA", "absolute", "Y")},
            { 0xba, new OpCodePart("TSX")},
            { 0xbc, new OpCodePart("LDY", "absolute", "X")},
            { 0xbd, new OpCodePart("LDA", "absolute", "X")},
            { 0xbe, new OpCodePart("LDX", "absolute", "Y")},
            { 0xc0, new OpCodePart("CPY", "immediate")},
            { 0xc1, new OpCodePart("CMP", "indirect", "X")},
            { 0xc4, new OpCodePart("CPY", "zeropage")},
            { 0xc5, new OpCodePart("CMP", "zeropage")},
            { 0xc6, new OpCodePart("DEC", "zeropage")},
            { 0xc8, new OpCodePart("INY")},
            { 0xc9, new OpCodePart("CMP", "immediate")},
            { 0xca, new OpCodePart("DEX")},
            { 0xcc, new OpCodePart("CPY", "absolute")},
            { 0xcd, new OpCodePart("CMP", "absolute")},
            { 0xce, new OpCodePart("DEC", "absolute")},
            { 0xd0, new OpCodePart("BNE", "relative")},
            { 0xd1, new OpCodePart("CMP", "indirect", "Y")},
            { 0xd5, new OpCodePart("CMP", "zeropage", "X")},
            { 0xd6, new OpCodePart("DEC", "zeropage", "X")},
            { 0xd8, new OpCodePart("CLD")},
            { 0xd9, new OpCodePart("CMP", "absolute", "Y")},
            { 0xdd, new OpCodePart("CMP", "absolute", "X")},
            { 0xde, new OpCodePart("DEC", "absolute", "X")},
            { 0xe0, new OpCodePart("CPX", "immediate")},
            { 0xe1, new OpCodePart("SBC", "indirect", "X")},
            { 0xe4, new OpCodePart("CPX", "zeropage")},
            { 0xe5, new OpCodePart("SBC", "zeropage")},
            { 0xe6, new OpCodePart("INC", "zeropage")},
            { 0xe8, new OpCodePart("INX")},
            { 0xe9, new OpCodePart("SBC", "immediate")},
            { 0xea, new OpCodePart("NOP")},
            { 0xec, new OpCodePart("CPX", "absolute")},
            { 0xed, new OpCodePart("SBC", "absolute")},
            { 0xee, new OpCodePart("INC", "absolute")},
            { 0xf0, new OpCodePart("BEQ", "relative")},
            { 0xf1, new OpCodePart("SBC", "indirect", "Y")},
            { 0xf5, new OpCodePart("SBC", "zeropage", "X")},
            { 0xf6, new OpCodePart("INC", "zeropage", "X")},
            { 0xf8, new OpCodePart("SED")},
            { 0xf9, new OpCodePart("SBC", "absolute", "Y")},
            { 0xfd, new OpCodePart("SBC", "absolute", "X")},
            { 0xfe, new OpCodePart("INC", "absolute", "X")}
        };
    
        public static OpCodePart? GetOpCode(byte opCode)
        {
            if (opCodeTable.TryGetValue(opCode, out var op))
                return op;
            else 
                return null;
        }
    }

    public class OpCodePart
        {
            public string Operation { get; set; }
            public string? Addressing { get; set; }
            public string? Register { get; set; }
            public OpCodePart(string operation, string? addressing = null, string? register = null)
            {
                Operation = operation;
                if (addressing!=null)
                    Addressing = addressing;
                if (register!= null)
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
