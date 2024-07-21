using System;
using System.Collections.Generic;
using Apple2Sharp.Mainboard;
using Apple2Sharp.Mainboard.Enums;

namespace Apple2Sharp.CPU65C02
{
    public static class OpCodes
    {
        public static Dictionary<byte, OpCodePart?> opCodeTable = new Dictionary<byte, OpCodePart?>()
        {
            { 0x69, new OpCodePart(OpCodeType.ADC, 2, Addressing.immediate)},
            { 0x65, new OpCodePart(OpCodeType.ADC, 3, Addressing.zeropage)},
            { 0x75, new OpCodePart(OpCodeType.ADC, 4, Addressing.zeropage, Register.X)},
            { 0x6d, new OpCodePart(OpCodeType.ADC, 4, Addressing.absolute)},
            { 0x7d, new OpCodePart(OpCodeType.ADC, 4, Addressing.absolute, Register.X)}, //*
            { 0x79, new OpCodePart(OpCodeType.ADC, 4, Addressing.absolute, Register.Y)}, //*
            { 0x61, new OpCodePart(OpCodeType.ADC, 6, Addressing.indirect, Register.X)},
            { 0x71, new OpCodePart(OpCodeType.ADC, 5, Addressing.indirect, Register.Y)}, //*
            { 0x72, new OpCodePart(OpCodeType.ADC, 5, Addressing.izeropage)},
            { 0x29, new OpCodePart(OpCodeType.AND, 2, Addressing.immediate)},
            { 0x25, new OpCodePart(OpCodeType.AND, 3, Addressing.zeropage)},
            { 0x35, new OpCodePart(OpCodeType.AND, 4, Addressing.zeropage, Register.X)},
            { 0x2d, new OpCodePart(OpCodeType.AND, 4, Addressing.absolute)},
            { 0x3d, new OpCodePart(OpCodeType.AND, 4, Addressing.absolute, Register.X)}, //*
            { 0x39, new OpCodePart(OpCodeType.AND, 4, Addressing.absolute, Register.Y)}, //*
            { 0x21, new OpCodePart(OpCodeType.AND, 6, Addressing.indirect, Register.X)},
            { 0x31, new OpCodePart(OpCodeType.AND, 5, Addressing.indirect, Register.Y)}, //*
            { 0x32, new OpCodePart(OpCodeType.AND, 5, Addressing.izeropage)}, //*
            { 0x0a, new OpCodePart(OpCodeType.ASL, 2)},
            { 0x06, new OpCodePart(OpCodeType.ASL, 5, Addressing.zeropage)},
            { 0x16, new OpCodePart(OpCodeType.ASL, 6, Addressing.zeropage, Register.X)},
            { 0x0e, new OpCodePart(OpCodeType.ASL, 6, Addressing.absolute)},
            { 0x1e, new OpCodePart(OpCodeType.ASL, 7, Addressing.absolute, Register.X)},
            { 0x90, new OpCodePart(OpCodeType.BCC, 2, Addressing.relative)},            //**
            { 0xb0, new OpCodePart(OpCodeType.BCS, 2, Addressing.relative)}, //**
            { 0xf0, new OpCodePart(OpCodeType.BEQ, 2, Addressing.relative)}, //**
            { 0x24, new OpCodePart(OpCodeType.BIT, 3, Addressing.zeropage)},
            { 0x2c, new OpCodePart(OpCodeType.BIT, 4, Addressing.absolute)},
            { 0x89, new OpCodePart(OpCodeType.BITI, 2, Addressing.immediate)},
            { 0x34, new OpCodePart(OpCodeType.BIT, 4, Addressing.zeropage, Register.X)},
            { 0x3c, new OpCodePart(OpCodeType.BIT, 4, Addressing.absolute, Register.X)},
            { 0x30, new OpCodePart(OpCodeType.BMI, 2, Addressing.relative)}, //*
            { 0xd0, new OpCodePart(OpCodeType.BNE, 2, Addressing.relative)}, //**
            { 0x10, new OpCodePart(OpCodeType.BPL, 2, Addressing.relative)}, //**
            { 0x00, new OpCodePart(OpCodeType.BRK, 7)},
            { 0x80, new OpCodePart(OpCodeType.BRA, 3, Addressing.relative)},
            { 0x50, new OpCodePart(OpCodeType.BVC, 2, Addressing.relative)}, //*
            { 0x70, new OpCodePart(OpCodeType.BVS, 2, Addressing.relative)}, //**
            { 0x18, new OpCodePart(OpCodeType.CLC, 2)},
            { 0xd8, new OpCodePart(OpCodeType.CLD, 2)},
            { 0x58, new OpCodePart(OpCodeType.CLI, 2)},
            { 0xb8, new OpCodePart(OpCodeType.CLV, 2)},
            { 0xc9, new OpCodePart(OpCodeType.CMP, 2, Addressing.immediate)},
            { 0xc5, new OpCodePart(OpCodeType.CMP, 3, Addressing.zeropage)},
            { 0xd2, new OpCodePart(OpCodeType.CMP, 5, Addressing.izeropage)},
            { 0xd5, new OpCodePart(OpCodeType.CMP, 4, Addressing.zeropage, Register.X)},
            { 0xcd, new OpCodePart(OpCodeType.CMP, 4, Addressing.absolute)},
            { 0xdd, new OpCodePart(OpCodeType.CMP, 4, Addressing.absolute, Register.X)}, //*
            { 0xd9, new OpCodePart(OpCodeType.CMP, 4, Addressing.absolute, Register.Y)}, //*
            { 0xc1, new OpCodePart(OpCodeType.CMP, 6, Addressing.indirect, Register.X)},
            { 0xd1, new OpCodePart(OpCodeType.CMP, 5, Addressing.indirect, Register.Y)}, //*
            { 0xe0, new OpCodePart(OpCodeType.CPX, 2, Addressing.immediate)},
            { 0xe4, new OpCodePart(OpCodeType.CPX, 3, Addressing.zeropage)},
            { 0xec, new OpCodePart(OpCodeType.CPX, 4, Addressing.absolute)},
            { 0xc0, new OpCodePart(OpCodeType.CPY, 2, Addressing.immediate)},
            { 0xc4, new OpCodePart(OpCodeType.CPY, 3, Addressing.zeropage)},
            { 0xcc, new OpCodePart(OpCodeType.CPY, 4, Addressing.absolute)},
            { 0x3a, new OpCodePart(OpCodeType.DEA, 2)},
            { 0xc6, new OpCodePart(OpCodeType.DEC, 5, Addressing.zeropage)},
            { 0xd6, new OpCodePart(OpCodeType.DEC, 6, Addressing.zeropage, Register.X)},
            { 0xce, new OpCodePart(OpCodeType.DEC, 6, Addressing.absolute)},
            { 0xde, new OpCodePart(OpCodeType.DEC, 7, Addressing.absolute, Register.X)},
            { 0xca, new OpCodePart(OpCodeType.DEX, 2)},
            { 0x88, new OpCodePart(OpCodeType.DEY, 2)},
            { 0x49, new OpCodePart(OpCodeType.EOR, 2, Addressing.immediate)},
            { 0x45, new OpCodePart(OpCodeType.EOR, 3, Addressing.zeropage)},
            { 0x55, new OpCodePart(OpCodeType.EOR, 4, Addressing.zeropage, Register.X)},
            { 0x4d, new OpCodePart(OpCodeType.EOR, 4, Addressing.absolute)},
            { 0x5d, new OpCodePart(OpCodeType.EOR, 4, Addressing.absolute, Register.X)}, //*
            { 0x59, new OpCodePart(OpCodeType.EOR, 4, Addressing.absolute, Register.Y)}, //*
            { 0x41, new OpCodePart(OpCodeType.EOR, 6, Addressing.indirect, Register.X)},
            { 0x51, new OpCodePart(OpCodeType.EOR, 5, Addressing.indirect, Register.Y)}, //*
            { 0x52, new OpCodePart(OpCodeType.EOR, 5, Addressing.izeropage)},
            { 0x1a, new OpCodePart(OpCodeType.INA, 2)},
            { 0xe6, new OpCodePart(OpCodeType.INC, 5, Addressing.zeropage)},
            { 0xee, new OpCodePart(OpCodeType.INC, 6, Addressing.absolute)},
            { 0xf6, new OpCodePart(OpCodeType.INC, 6, Addressing.zeropage, Register.X)},
            { 0xfe, new OpCodePart(OpCodeType.INC, 7, Addressing.absolute, Register.X)},
            { 0xe8, new OpCodePart(OpCodeType.INX, 2)},
            { 0xc8, new OpCodePart(OpCodeType.INY, 2)},
            { 0x4c, new OpCodePart(OpCodeType.JMP, 3, Addressing.absolute)},
            { 0x6c, new OpCodePart(OpCodeType.JMP, 5, Addressing.indirect)},
            { 0x7c, new OpCodePart(OpCodeType.JMP, 6, Addressing.iabsolute, Register.X)},
            { 0x20, new OpCodePart(OpCodeType.JSR, 6, Addressing.absolute)},
            { 0xa9, new OpCodePart(OpCodeType.LDA, 2, Addressing.immediate)},
            { 0xa5, new OpCodePart(OpCodeType.LDA, 3, Addressing.zeropage)},
            { 0xb5, new OpCodePart(OpCodeType.LDA, 4, Addressing.zeropage, Register.X)},
            { 0xad, new OpCodePart(OpCodeType.LDA, 4, Addressing.absolute)},
            { 0xbd, new OpCodePart(OpCodeType.LDA, 4, Addressing.absolute, Register.X)}, //*
            { 0xb9, new OpCodePart(OpCodeType.LDA, 4, Addressing.absolute, Register.Y)}, //*
            { 0xa1, new OpCodePart(OpCodeType.LDA, 6, Addressing.indirect, Register.X)},
            { 0xb1, new OpCodePart(OpCodeType.LDA, 5, Addressing.indirect, Register.Y)}, //*
            { 0xb2, new OpCodePart(OpCodeType.LDA, 5, Addressing.izeropage)},
            { 0xa2, new OpCodePart(OpCodeType.LDX, 2, Addressing.immediate)},
            { 0xa6, new OpCodePart(OpCodeType.LDX, 3, Addressing.zeropage)},
            { 0xb6, new OpCodePart(OpCodeType.LDX, 4, Addressing.zeropage, Register.Y)},
            { 0xae, new OpCodePart(OpCodeType.LDX, 4, Addressing.absolute)},
            { 0xbe, new OpCodePart(OpCodeType.LDX, 4, Addressing.absolute, Register.Y)}, //*
            { 0xa0, new OpCodePart(OpCodeType.LDY, 2, Addressing.immediate)},
            { 0xa4, new OpCodePart(OpCodeType.LDY, 3, Addressing.zeropage)},
            { 0xb4, new OpCodePart(OpCodeType.LDY, 4, Addressing.zeropage, Register.X)},
            { 0xac, new OpCodePart(OpCodeType.LDY, 4, Addressing.absolute)},
            { 0xbc, new OpCodePart(OpCodeType.LDY, 4, Addressing.absolute, Register.X)}, //*
            { 0x4a, new OpCodePart(OpCodeType.LSR, 2)},
            { 0x46, new OpCodePart(OpCodeType.LSR, 5, Addressing.zeropage)},
            { 0x56, new OpCodePart(OpCodeType.LSR, 6, Addressing.zeropage, Register.X)},
            { 0x4e, new OpCodePart(OpCodeType.LSR, 6, Addressing.absolute)},
            { 0x5e, new OpCodePart(OpCodeType.LSR, 7, Addressing.absolute, Register.X)},
            { 0x09, new OpCodePart(OpCodeType.ORA, 2, Addressing.immediate)},
            { 0x05, new OpCodePart(OpCodeType.ORA, 3, Addressing.zeropage)},
            { 0x15, new OpCodePart(OpCodeType.ORA, 4, Addressing.zeropage, Register.X)},
            { 0x0d, new OpCodePart(OpCodeType.ORA, 4, Addressing.absolute)},
            { 0x1d, new OpCodePart(OpCodeType.ORA, 4, Addressing.absolute, Register.X)}, //*
            { 0x19, new OpCodePart(OpCodeType.ORA, 4, Addressing.absolute, Register.Y)}, //*
            { 0x01, new OpCodePart(OpCodeType.ORA, 6, Addressing.indirect, Register.X)},
            { 0x11, new OpCodePart(OpCodeType.ORA, 5, Addressing.indirect, Register.Y)}, //*
            { 0x12, new OpCodePart(OpCodeType.ORA, 5, Addressing.izeropage)},
            { 0x48, new OpCodePart(OpCodeType.PHA, 3)},
            { 0xda, new OpCodePart(OpCodeType.PHX, 3)},
            { 0x5a, new OpCodePart(OpCodeType.PHY, 3)},
            { 0x08, new OpCodePart(OpCodeType.PHP, 3)},
            { 0x68, new OpCodePart(OpCodeType.PLA, 4)},
            { 0xfa, new OpCodePart(OpCodeType.PLX, 4)},
            { 0x7a, new OpCodePart(OpCodeType.PLY, 4)},
            { 0x28, new OpCodePart(OpCodeType.PLP, 4)},
            { 0x2a, new OpCodePart(OpCodeType.ROL, 2)},
            { 0x26, new OpCodePart(OpCodeType.ROL, 5, Addressing.zeropage)},
            { 0x36, new OpCodePart(OpCodeType.ROL, 6, Addressing.zeropage, Register.X)},
            { 0x2e, new OpCodePart(OpCodeType.ROL, 6, Addressing.absolute)},
            { 0x3e, new OpCodePart(OpCodeType.ROL, 7, Addressing.absolute, Register.X)},
            { 0x6a, new OpCodePart(OpCodeType.ROR, 2)},
            { 0x66, new OpCodePart(OpCodeType.ROR, 5, Addressing.zeropage)},
            { 0x76, new OpCodePart(OpCodeType.ROR, 6, Addressing.zeropage, Register.X)},
            { 0x6e, new OpCodePart(OpCodeType.ROR, 6, Addressing.absolute)},
            { 0x7e, new OpCodePart(OpCodeType.ROR, 7, Addressing.absolute, Register.X)},
            { 0x40, new OpCodePart(OpCodeType.RTI, 6)},
            { 0x60, new OpCodePart(OpCodeType.RTS, 6)},
            { 0xe9, new OpCodePart(OpCodeType.SBC, 2, Addressing.immediate)},
            { 0xe5, new OpCodePart(OpCodeType.SBC, 3, Addressing.zeropage)},
            { 0xf5, new OpCodePart(OpCodeType.SBC, 4, Addressing.zeropage, Register.X)},
            { 0xed, new OpCodePart(OpCodeType.SBC, 4, Addressing.absolute)},
            { 0xfd, new OpCodePart(OpCodeType.SBC, 4, Addressing.absolute, Register.X)}, //*
            { 0xf9, new OpCodePart(OpCodeType.SBC, 4, Addressing.absolute, Register.Y)}, //*
            { 0xe1, new OpCodePart(OpCodeType.SBC, 6, Addressing.indirect, Register.X)},
            { 0xf1, new OpCodePart(OpCodeType.SBC, 5, Addressing.indirect, Register.Y)}, //*
            { 0xf2, new OpCodePart(OpCodeType.SBC, 5, Addressing.izeropage)},
            { 0x38, new OpCodePart(OpCodeType.SEC, 2)},
            { 0xf8, new OpCodePart(OpCodeType.SED, 2)},
            { 0x78, new OpCodePart(OpCodeType.SEI, 2)},
            { 0x85, new OpCodePart(OpCodeType.STA, 3, Addressing.zeropage)},
            { 0x95, new OpCodePart(OpCodeType.STA, 4, Addressing.zeropage, Register.X)},
            { 0x8d, new OpCodePart(OpCodeType.STA, 4, Addressing.absolute)},
            { 0x9d, new OpCodePart(OpCodeType.STA, 5, Addressing.absolute, Register.X)},
            { 0x99, new OpCodePart(OpCodeType.STA, 5, Addressing.absolute, Register.Y)},
            { 0x81, new OpCodePart(OpCodeType.STA, 6, Addressing.indirect, Register.X)},
            { 0x91, new OpCodePart(OpCodeType.STA, 6, Addressing.indirect, Register.Y)},
            { 0x92, new OpCodePart(OpCodeType.STA, 5, Addressing.izeropage)},
            { 0x86, new OpCodePart(OpCodeType.STX, 3, Addressing.zeropage)},
            { 0x96, new OpCodePart(OpCodeType.STX, 4, Addressing.zeropage, Register.Y)},
            { 0x8e, new OpCodePart(OpCodeType.STX, 4, Addressing.absolute)},
            { 0x84, new OpCodePart(OpCodeType.STY, 3, Addressing.zeropage)},
            { 0x94, new OpCodePart(OpCodeType.STY, 4, Addressing.zeropage, Register.X)},
            { 0x8c, new OpCodePart(OpCodeType.STY, 4, Addressing.absolute)},
            { 0x64, new OpCodePart(OpCodeType.STZ, 3, Addressing.zeropage)},
            { 0x74, new OpCodePart(OpCodeType.STZ, 4, Addressing.zeropage, Register.X)},
            { 0x9c, new OpCodePart(OpCodeType.STZ, 4, Addressing.absolute)},
            { 0x9e, new OpCodePart(OpCodeType.STZ, 5, Addressing.absolute, Register.X)},
            { 0xaa, new OpCodePart(OpCodeType.TAX, 2)},
            { 0xa8, new OpCodePart(OpCodeType.TAY, 2)},
            { 0x9a, new OpCodePart(OpCodeType.TXS, 2)},
            { 0x8a, new OpCodePart(OpCodeType.TXA, 2)},
            { 0xba, new OpCodePart(OpCodeType.TSX, 2)},
            { 0x98, new OpCodePart(OpCodeType.TYA, 2)},
            { 0x14, new OpCodePart(OpCodeType.TRB, 5, Addressing.zeropage)},
            { 0x1c, new OpCodePart(OpCodeType.TRB, 6, Addressing.absolute)},
            { 0x04, new OpCodePart(OpCodeType.TSB, 5, Addressing.zeropage)},
            { 0x0c, new OpCodePart(OpCodeType.TSB, 6, Addressing.absolute)},
            
            { 0x02, new OpCodePart(OpCodeType.NOP, 2, Addressing.zeropage)},
            { 0x22, new OpCodePart(OpCodeType.NOP, 2, Addressing.zeropage)},
            { 0x42, new OpCodePart(OpCodeType.NOP, 2, Addressing.zeropage)},
            { 0x62, new OpCodePart(OpCodeType.NOP, 2, Addressing.zeropage)},
            { 0x82, new OpCodePart(OpCodeType.NOP, 2, Addressing.zeropage)},
            { 0xc2, new OpCodePart(OpCodeType.NOP, 2, Addressing.zeropage)},
            { 0xe2, new OpCodePart(OpCodeType.NOP, 2, Addressing.zeropage)},
            { 0xea, new OpCodePart(OpCodeType.NOP, 2)},
            
            { 0x03, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x13, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x23, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x33, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x43, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x53, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x63, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x73, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x83, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x93, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xa3, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xb3, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xc3, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xd3, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xe3, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xf3, new OpCodePart(OpCodeType.NOP, 1)},

            { 0x07, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x17, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x27, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x37, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x47, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x57, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x67, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x77, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x87, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x97, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xa7, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xb7, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xc7, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xd7, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xe7, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xf7, new OpCodePart(OpCodeType.NOP, 1)},

            { 0x0b, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x1b, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x2b, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x3b, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x4b, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x5b, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x6b, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x7b, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x8b, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x9b, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xab, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xbb, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xcb, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xdb, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xeb, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xfb, new OpCodePart(OpCodeType.NOP, 1)},

            { 0x0f, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x1f, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x2f, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x3f, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x4f, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x5f, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x6f, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x7f, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x8f, new OpCodePart(OpCodeType.NOP, 1)},
            { 0x9f, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xaf, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xbf, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xcf, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xdf, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xef, new OpCodePart(OpCodeType.NOP, 1)},
            { 0xff, new OpCodePart(OpCodeType.NOP, 1)},

            { 0x44, new OpCodePart(OpCodeType.NOP, 3, Addressing.zeropage)},
            { 0x54, new OpCodePart(OpCodeType.NOP, 4, Addressing.zeropage)},
            { 0xd4, new OpCodePart(OpCodeType.NOP, 4, Addressing.zeropage)},
            { 0xf4, new OpCodePart(OpCodeType.NOP, 4, Addressing.zeropage, Register.X)},
            { 0x5c, new OpCodePart(OpCodeType.NOP, 8, Addressing.absolute, Register.X)},
            { 0xdc, new OpCodePart(OpCodeType.NOP, 4, Addressing.absolute, Register.X)},
            { 0xfc, new OpCodePart(OpCodeType.NOP, 4, Addressing.absolute, Register.X)}
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

        public static ushort? ProcessAddressing(OpCodePart? opCodePart, State state, Apple2Board mainBoard, CPU65C02 cpu)
        {
            ushort? refAddress = null;
            switch (opCodePart?.Addressing)
            {
                case Addressing.immediate:
                    cpu.IncrementPC();
                    refAddress = state.PC;
                    cpu.IncrementPC();
                    break;
                case Addressing.absolute:
                    cpu.IncrementPC();
                    refAddress = mainBoard.ReadAddressLLHH(state.PC);
                    if (opCodePart.Register != null)
                    {
                        if (opCodePart.Register == Register.Y)
                            refAddress = (ushort)((refAddress ?? 0) + state.Y);
                        else
                            refAddress = (ushort)((refAddress ?? 0) + state.X);
                    }
                    cpu.IncrementPC();
                    cpu.IncrementPC();
                    break;
                case Addressing.iabsolute:
                    cpu.IncrementPC();
                    refAddress = mainBoard.ReadAddressLLHH(state.PC);
                    if (opCodePart.Register != null)
                    {
                        if (opCodePart.Register == Register.Y)
                            refAddress = (ushort)((refAddress ?? 0) + state.Y);
                        else
                            refAddress = (ushort)((refAddress ?? 0) + state.X);
                    }
                    refAddress = mainBoard.ReadAddressLLHH(refAddress);
                    cpu.IncrementPC();
                    cpu.IncrementPC();
                    break;
                case Addressing.zeropage:
                    cpu.IncrementPC();
                    refAddress = mainBoard.ReadZeroPageAddress(state.PC);
                    if (refAddress != null)
                    {
                        if (opCodePart.Register != null)
                        {
                            if (opCodePart.Register == Register.Y)
                                refAddress = (byte)(refAddress + state.Y);
                            else
                                refAddress = (byte)(refAddress + state.X);
                        }
                    }
                    cpu.IncrementPC();
                    break;
                case Addressing.izeropage:
                    cpu.IncrementPC();
                    refAddress = mainBoard.ReadZeroPageAddress(state.PC);
                    if (refAddress != null)
                    {
                        if (opCodePart.Register != null)
                        {
                            if (opCodePart.Register == Register.Y)
                                refAddress = (byte)(refAddress + state.Y);
                            else
                                refAddress = (byte)(refAddress + state.X);
                        }
                    }
                    refAddress = mainBoard.ReadAddressLLHH(refAddress);
                    cpu.IncrementPC();
                    break;
                
                case Addressing.indirect:
                    if (opCodePart.Register != null)
                    {
                        cpu.IncrementPC();
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
                        cpu.IncrementPC();
                    }
                    else
                    {
                        cpu.IncrementPC();
                        var indirectAddress = mainBoard.ReadAddressLLHH(state.PC);
                        if (indirectAddress != null)
                            refAddress = mainBoard.ReadAddressLLHH(indirectAddress);
                        cpu.IncrementPC();
                        cpu.IncrementPC();
                    }
                    break;
                case Addressing.relative:
                    cpu.IncrementPC();
                    var b = mainBoard.ReadByte(state.PC);
                    var offset = unchecked((sbyte)b);
                    refAddress = (ushort)(state.PC + 1 + offset);
                    cpu.IncrementPC();
                    break;
                default:
                    cpu.IncrementPC();
                    break;
            }
            return refAddress;
        }

        public static void Process(OpCodePart? operation, State state, Apple2Board mainBoard, ushort? refAddress)
        {
            switch (operation?.Operation)
            {
                case OpCodeType.CLC:
                    FlagOpCodeProcessors.Process_CLC(state);
                    break;
                case OpCodeType.SEI:
                    FlagOpCodeProcessors.Process_SEI(state);
                    break;
                case OpCodeType.CLV:
                    FlagOpCodeProcessors.Process_CLV(state);
                    break;
                case OpCodeType.CLI:
                    FlagOpCodeProcessors.Process_CLI(state);
                    break;
                case OpCodeType.SEC:
                    FlagOpCodeProcessors.Process_SEC(state);
                    break;
                case OpCodeType.SED:
                    FlagOpCodeProcessors.Process_SED(state);
                    break;
                case OpCodeType.CLD:
                    FlagOpCodeProcessors.Process_CLD(state);
                    break;
                case OpCodeType.LDY:
                    LoadOpCodeProcessors.Process_LDY(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.LDX:
                    LoadOpCodeProcessors.Process_LDX(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.LDA:
                    LoadOpCodeProcessors.Process_LDA(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.STY:
                    StoreOpCodeProcessors.Process_STY(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.STX:
                    StoreOpCodeProcessors.Process_STX(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.STA:
                    StoreOpCodeProcessors.Process_STA(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.STZ:
                    StoreOpCodeProcessors.Process_STZ(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.CPX:
                    CompareOpCodeProcessors.Process_CPX(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.CPY:
                    CompareOpCodeProcessors.Process_CPY(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.CMP:
                    CompareOpCodeProcessors.Process_CMP(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.BIT:
                    CompareOpCodeProcessors.Process_BIT(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.BITI:
                    CompareOpCodeProcessors.Process_BITI(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.BPL:
                    BranchOpCodeProcessors.Process_BPL(state, refAddress ?? 0);
                    break;
                case OpCodeType.BRA:
                    BranchOpCodeProcessors.Process_BRA(state, refAddress ?? 0);
                    break;
                case OpCodeType.BVC:
                    BranchOpCodeProcessors.Process_BVC(state, refAddress ?? 0);
                    break;
                case OpCodeType.BVS:
                    BranchOpCodeProcessors.Process_BVS(state, refAddress ?? 0);
                    break;
                case OpCodeType.BMI:
                    BranchOpCodeProcessors.Process_BMI(state, refAddress ?? 0);
                    break;
                case OpCodeType.BCS:
                    BranchOpCodeProcessors.Process_BCS(state, refAddress ?? 0);
                    break;
                case OpCodeType.BNE:
                    BranchOpCodeProcessors.Process_BNE(state, refAddress ?? 0);
                    break;
                case OpCodeType.BCC:
                    BranchOpCodeProcessors.Process_BCC(state, refAddress ?? 0);
                    break;
                case OpCodeType.BEQ:
                    BranchOpCodeProcessors.Process_BEQ(state, refAddress ?? 0);
                    break;
                case OpCodeType.JMP:
                    BranchOpCodeProcessors.Process_JMP(state, refAddress ?? 0);
                    break;
                case OpCodeType.DEX:
                    IncAndDecOpCodeProcessors.Process_DEX(state);
                    break;
                case OpCodeType.DEY:
                    IncAndDecOpCodeProcessors.Process_DEY(state);
                    break;
                case OpCodeType.INX:
                    IncAndDecOpCodeProcessors.Process_INX(state);
                    break;
                case OpCodeType.INY:
                    IncAndDecOpCodeProcessors.Process_INY(state);
                    break;
                case OpCodeType.INA:
                    IncAndDecOpCodeProcessors.Process_INA(state);
                    break;
                case OpCodeType.DEC:
                    IncAndDecOpCodeProcessors.Process_DEC(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.DEA:
                    IncAndDecOpCodeProcessors.Process_DEA(state);
                    break;
                case OpCodeType.INC:
                    IncAndDecOpCodeProcessors.Process_INC(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.RTS:
                    SubRoutineOpCodeProcessors.Process_RTS(state, mainBoard);
                    break;
                case OpCodeType.JSR:
                    SubRoutineOpCodeProcessors.Process_JSR(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.RTI:
                    SubRoutineOpCodeProcessors.Process_RTI(state, mainBoard);
                    break;
                case OpCodeType.BRK:
                    SubRoutineOpCodeProcessors.Process_BRK(state, mainBoard);
                    break;
                case OpCodeType.TYA:
                    TransferOpCodeProcessors.Process_TYA(state);
                    break;
                case OpCodeType.TXS:
                    TransferOpCodeProcessors.Process_TXS(state);
                    break;
                case OpCodeType.TXA:
                    TransferOpCodeProcessors.Process_TXA(state);
                    break;
                case OpCodeType.TSX:
                    TransferOpCodeProcessors.Process_TSX(state);
                    break;
                case OpCodeType.TAX:
                    TransferOpCodeProcessors.Process_TAX(state);
                    break;
                case OpCodeType.TAY:
                    TransferOpCodeProcessors.Process_TAY(state);
                    break;
                case OpCodeType.TRB:
                    TestOpCodeProcessors.Process_TRB(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.TSB:
                    TestOpCodeProcessors.Process_TSB(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.ROL:
                    ShiftAndRollOpCodeProcessors.Process_ROL(state, mainBoard, refAddress);
                    break;
                case OpCodeType.ROR:
                    ShiftAndRollOpCodeProcessors.Process_ROR(state, mainBoard, refAddress);
                    break;
                case OpCodeType.LSR:
                    ShiftAndRollOpCodeProcessors.Process_LSR(state, mainBoard, refAddress);
                    break;
                case OpCodeType.ASL:
                    ShiftAndRollOpCodeProcessors.Process_ASL(state, mainBoard, refAddress);
                    break;
                case OpCodeType.EOR:
                    BitwiseLogicOpCodeProcessors.Process_EOR(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.ORA:
                    BitwiseLogicOpCodeProcessors.Process_ORA(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.AND:
                    BitwiseLogicOpCodeProcessors.Process_AND(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.ADC:
                    MathOpCodeProcessors.Process_ADC(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.SBC:
                    MathOpCodeProcessors.Process_SBC(state, mainBoard, refAddress ?? 0);
                    break;
                case OpCodeType.PHP:
                    StackOpCodeProcessors.Process_PHP(state, mainBoard);
                    break;
                case OpCodeType.PLP:
                    StackOpCodeProcessors.Process_PLP(state, mainBoard);
                    break;
                case OpCodeType.PLA:
                    StackOpCodeProcessors.Process_PLA(state, mainBoard);
                    break;
                case OpCodeType.PLX:
                    StackOpCodeProcessors.Process_PLX(state, mainBoard);
                    break;
                case OpCodeType.PLY:
                    StackOpCodeProcessors.Process_PLY(state, mainBoard);
                    break;
                case OpCodeType.PHA:
                    StackOpCodeProcessors.Process_PHA(state, mainBoard);
                    break;
                case OpCodeType.PHX:
                    StackOpCodeProcessors.Process_PHX(state, mainBoard);
                    break;
                case OpCodeType.PHY:
                    StackOpCodeProcessors.Process_PHY(state, mainBoard);
                    break;
                default:
                    break;
            }
        }
    }
}