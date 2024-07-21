using Apple2Sharp.Mainboard.Enums;

namespace Apple2Sharp.CPU65C02
{

    public class OpCodePart
    {
        public int Cycles { get; set; }
        public OpCodeType Operation { get; set; }
        public Addressing? Addressing { get; set; }
        public Register? Register { get; set; }

        public OpCodePart(OpCodeType operation, int cycles, Addressing? addressing = null, Register? register = null)
        {
            Cycles = cycles;
            Operation = operation;
            if (addressing != null)
                Addressing = addressing;
            if (register != null)
                Register = register;
        }
    }


}
