using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Runtime;

namespace ConsoleApp 
{
    public  class Program 
    {
        public static bool run = true;

        [RequiresAssemblyFiles()]
        public static void Main(string[] args)
        {
            var roms = new Dictionary<ushort, byte[]>();
            string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyPath != null)
                assemblyPath+= "/";            

            roms.Add(0xf800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF800.rom"));
            roms.Add(0xf000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF000.rom"));
            roms.Add(0xe800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE800.rom"));
            roms.Add(0xe000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE000.rom"));
            roms.Add(0xd800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD800.rom"));
            roms.Add(0xd000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD000.rom"));
            
            run = true;
            CPU cpu = new CPU(new State(), 0xffff, roms, true);
            cpu.Reset();
            cpu.Start();
        }
    }
}