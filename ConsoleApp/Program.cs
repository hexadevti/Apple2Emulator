using Runtime;

namespace ConsoleApp 
{
    public  class Program 
    {
        public static bool run = true;
        

        public static void Main(string[] args)
        {
            //var appleRom = File.ReadAllBytes(@"C:\Users\luciano\repos\6502\ConsoleApp\apple1.rom");
            var appleRom = File.ReadAllBytes(@"/Users/lucianofaria/Desktop/Projects/6502/ConsoleApp/apple1.rom");
            //var appleRom = File.ReadAllBytes(@"/Users/lucianofaria/Desktop/Projects/6502/ConsoleApp/basic.rom");
            
            run = true;
            CPU cpu = new(new State(), new Memory(0xffff, appleRom, 0xff00));
            cpu.Reset();
            Loop(cpu);
            //var values = Enum.GetNames(typeof(OpCode));
            // foreach(var val in values)
            // {
            //     int op = Enum.Parse(typeof(OpCode), val).GetHashCode();
            //     var parts = val.Split('_');
            //     Console.WriteLine(@"{ 0x" + op.ToString("x").PadLeft(2, '0') + ", new OpCodePart(\"" + parts[0] + "\"" + (parts.Length >= 2 ? ", \"" + parts[1] + "\"" : "") + (parts.Length == 3 ? ", \"" + parts[2] + "\"" : "") + ")},");
            // }
            //  Console.ReadKey();
        }

        public static void Loop(CPU cpu)
        {
            
            while (run)
            {
                cpu.RunCycle();
                
            }

            Console.ReadKey();
        }

    }
}