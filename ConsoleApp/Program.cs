using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Text;
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
            memory.LoadSlotsROM(0xc600, File.ReadAllBytes(assemblyPath + "roms/diskinterface.rom"));
            memory.drive1 = new DiskDrive(assemblyPath + "roms/ProDOS 1.2.dsk", memory);
            memory.drive2 = new DiskDrive(assemblyPath + "roms/EMPTY DOS33.dsk", memory);


            List<Task> threads = new List<Task>();
            cpu = new CPU(state, memory);
            cpu.Reset();




            threads.Add(Task.Run(() =>
            {
                while (running)
                {
                    cpu.RunCycle();
                }
            }));
            threads.Add(Task.Run(() =>
            {
                while (running)
                {
                    Console.SetCursorPosition(0, 0);

                    var cursorH = memory.baseRAM[0x24];
                    var cursorV = memory.baseRAM[0x25];

                    StringBuilder output = new StringBuilder();

                    int posH = 0;
                    int posV = 0;

                    for (int b = 0; b < 3; b++)
                    {
                        posV = b * 8;
                        for (int l = 0; l < 8; l++)
                        {

                            for (ushort c = 0; c < 0x28; c++)
                            {
                                posH = c;

                                var chr = memory.baseRAM[(ushort)(0x400 + (b * 0x28) + (l * 0x80) + c)];
                                chr = (byte)(chr & 0b01111111);
                                if (posV == cursorV && posH == cursorH)
                                    chr = DateTime.Now.Millisecond > 500 ? chr : (byte)95;

                                if (chr == 96)
                                    chr = DateTime.Now.Millisecond > 500 ? (byte)32 : (byte)95;
                                output.Append(Encoding.ASCII.GetString(new[] { chr }));

                            }
                            posV = posV + 1;
                            output.Append("\n");
                        }
                    }

                    Console.Write(output.ToString());
                    Thread.Sleep(10);
                }
            }));
            threads.Add(Task.Run(() =>
            {
                while (running)
                {
                    if (Console.KeyAvailable)
                    {
                        var consoleKeyInfo = Console.ReadKey(true);

                        switch (consoleKeyInfo.Modifiers)
                        {
                            case ConsoleModifiers.Alt:
                                switch (consoleKeyInfo.Key)
                                {
                                    case ConsoleKey.C:
                                        memory.KeyPressed = 0x83;
                                        break;
                                    case ConsoleKey.F12:
                                        state.PC = 0;
                                        Console.Beep();
                                        break;
                                }
                                break;

                            default:
                                switch (consoleKeyInfo.Key)
                                {
                                    case ConsoleKey.LeftArrow:
                                        memory.KeyPressed = 0x88;
                                        break;
                                    case ConsoleKey.RightArrow:
                                        memory.KeyPressed = 0x95;
                                        break;
                                    case ConsoleKey.UpArrow:
                                        memory.KeyPressed = 0x8b;
                                        break;
                                    case ConsoleKey.DownArrow:
                                        memory.KeyPressed = 0x8a;
                                        break;
                                    case ConsoleKey.Enter:
                                        memory.KeyPressed = 0x8D;
                                        break;
                                    case ConsoleKey.Escape:
                                        memory.KeyPressed = 0x9b;
                                        break;
                                    default:
                                        switch (consoleKeyInfo.KeyChar)
                                        {
                                            case (char)231:
                                                memory.KeyPressed = 0x83;
                                                break;
                                            default:
                                                memory.KeyPressed = (byte)(Encoding.ASCII.GetBytes(new[] { consoleKeyInfo.KeyChar.ToString().ToUpper()[0] })[0] | 0b10000000);
                                                break;
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                    Thread.Sleep(10);
                }
            }));

            Task.WaitAll(threads.ToArray());
        }
    }
}