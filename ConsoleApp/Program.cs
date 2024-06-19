using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using Runtime;
using Runtime.Overlays;


namespace ConsoleApp
{
    public class Program
    {
        [RequiresAssemblyFiles()]
        public static void Main(string[] args)
        {
            
            Memory memory;
            CPU cpu;
            object lockObj = new object();
            Runtime.State state = new Runtime.State();
            bool running = true;

            string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyPath != null)
                assemblyPath += "/";

            memory = new Runtime.Memory(state);
            memory.LoadROM(0xf800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF800.rom"));
            memory.LoadROM(0xf000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF000.rom"));
            memory.LoadROM(0xe800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE800.rom"));
            memory.LoadROM(0xe000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE000.rom"));
            memory.LoadROM(0xd800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD800.rom"));
            memory.LoadROM(0xd000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD000.rom"));

            //memory.LoadInterfaceROM(0xc600, File.ReadAllBytes(assemblyPath + "roms/diskinterface.rom"));

            memory.RegisterOverlay(new KeyboardOvl());
            memory.RegisterOverlay(new CpuSoftswitchesOvl());
            memory.RegisterOverlay(new SlotsSoftSwitchesOvl());
            memory.RegisterOverlay(new EmptySlot1Ovl());
            memory.RegisterOverlay(new EmptySlot2Ovl());
            memory.RegisterOverlay(new EmptySlot3Ovl());
            memory.RegisterOverlay(new EmptySlot4Ovl());
            memory.RegisterOverlay(new EmptySlot5Ovl());
            memory.RegisterOverlay(new DiskIISlot6Ovl());
            memory.RegisterOverlay(new EmptySlot7Ovl());

            memory.drive1 = new DiskDrive(assemblyPath + "roms/ProDOS 1.2.dsk", memory);
            memory.drive2 = new DiskDrive(assemblyPath + "roms/EMPTY DOS33.dsk", memory);


            List<Task> threads = new List<Task>();
            cpu = new CPU(state, memory, false);
            cpu.Reset();
            
            


            threads.Add(Task.Run(() =>
            {
                while (running)
                {
                    cpu.RunCycle(false);
                }
            }));
            threads.Add(Task.Run(() =>
            {
                while (running)
                {
                    cpu.RefreshScreen();
                    Thread.Sleep(10);
                }
            }));
            threads.Add(Task.Run(() =>
            {
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