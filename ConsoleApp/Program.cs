using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using Runtime;
using Runtime.Overlays;

namespace ConsoleApp 
{
    public  class Program 
    {
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

            // roms.Add(0xf800, File.ReadAllBytes(assemblyPath + "roms/OriginalF800.rom"));
            //roms.Add(0xf800, File.ReadAllBytes(assemblyPath + "roms/FreezesF800v1.rom"));

            roms.Add(0xc600, File.ReadAllBytes(assemblyPath + "roms/diskinterface.rom"));
        
            Runtime.Memory memory = new Runtime.Memory(0xffff);
            //memory.RegisterOverlay(new AppleScreenOvl()); // Apple I
            memory.RegisterOverlay(new KeyboardOvl());
            memory.RegisterOverlay(new CpuSoftswitchesOvl());
            memory.RegisterOverlay(new SlotsSoftSwitchesOvl());
            memory.drive = new DiskDrive(assemblyPath + "roms/DOS 3.3 System Master - 680-0051-00.dsk", memory);

            foreach (var item in roms)
            {
                memory.WriteAt(item.Key, item.Value);
            }

            List<Task> threads = new List<Task>();

            bool running = true;


            CPU cpu = new CPU(new State(), memory, true);
            cpu.Reset();
            cpu.InitConsole();

            threads.Add(Task.Run(() => {
                while (running)
                {
                    cpu.RunCycle();
                }
            }));
            threads.Add(Task.Run(() => {
                while (running)
                {

                    cpu.RefreshScreen();
                    Thread.Sleep(10);
                }
            }));
            threads.Add(Task.Run(() => {
                while (running)
                {
                    cpu.Keyboard();
                    Thread.Sleep(10);
                }
            }));

            Task.WaitAll(threads.ToArray());
        }
    }
}