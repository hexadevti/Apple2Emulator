using Runtime;

namespace ConsoleApp 
{
    public  class Program 
    {
        public static bool run = true;
        

        public static void Main(string[] args)
        {
            //var appleRom = File.ReadAllBytes(@"C:\Users\luciano\repos\6502\ConsoleApp\apple1.rom");\
            var roms = new Dictionary<ushort, byte[]>();
            
            //roms.Add(0xf800, File.ReadAllBytes(@"/Users/lucianofaria/Desktop/Projects/6502/ConsoleApp/Apple II ROM Pages F8-FF - 341-0004 - Original Monitor.bin"));
            roms.Add(0xf800, File.ReadAllBytes(@"C:\Users\luciano\repos\6502\ConsoleApp\Apple II+ - 341-0020 - Applesoft BASIC Autostart Monitor F800 - 2716.bin"));
            roms.Add(0xf000, File.ReadAllBytes(@"C:\Users\luciano\repos\6502\ConsoleApp\Apple II+ - 341-0015 - Applesoft BASIC F000 - 2716.bin"));
            roms.Add(0xe800, File.ReadAllBytes(@"C:\Users\luciano\repos\6502\ConsoleApp\Apple II+ - 341-0014 - Applesoft BASIC E800 - 2716.bin"));
            roms.Add(0xe000, File.ReadAllBytes(@"C:\Users\luciano\repos\6502\ConsoleApp\Apple II+ - 341-0013 - Applesoft BASIC E000 - 2716.bin"));
            roms.Add(0xd800, File.ReadAllBytes(@"C:\Users\luciano\repos\6502\ConsoleApp\Apple II+ - 341-0012 - Applesoft BASIC D800 - 2716.bin"));
            roms.Add(0xd000, File.ReadAllBytes(@"C:\Users\luciano\repos\6502\ConsoleApp\Apple II+ - 341-0011 - Applesoft BASIC D000 - 2716.bin"));

            //roms.Add(0xe000, File.ReadAllBytes(@"/Users/lucianofaria/Desktop/Projects/6502/ConsoleApp/basic.rom"));
            
            run = true;
            CPU cpu = new CPU(new State(), 0xffff, roms, true);
            cpu.Reset();
            cpu.Start();
        }
    }
}