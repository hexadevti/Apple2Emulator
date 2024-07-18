namespace Apple2Sharp.CPU
{
    public class State
    {
        public ushort PC { get; set; }
        public byte Y { get; set; }
        public byte A { get; set; }
        public byte X { get; set; }
        public bool C { get; set; }
        public bool Z { get; set; }
        public bool I { get; set; }
        public bool D { get; set; }
        public bool B { get; set; }
        public bool V { get; set; }
        public bool N { get; set; }
        public byte S { get; set; }

        public State()
        {
            PC = 0xfffc;
            Y = 0;
            A = 0;
            X = 0;
            C = false;
            Z = false;
            I = false;
            D = false;
            B = false;
            V = false;
            N = false;
            S = 0;
        }
    }
}