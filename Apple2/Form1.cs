using Runtime;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.AxHost;
using System.Windows.Input;
using Microsoft.VisualBasic.Devices;
using Runtime.Overlays;
using System.Security.Cryptography.X509Certificates;

namespace Apple2;

public partial class Form1 : Form
{
    private Memory memory { get; set; }

    private CPU cpu { get; set;}
    
    float zoom = 4.0f;
    int scrWidth = 280;
    int scrHeight = 192;

    object lockObj = new object();

    Runtime.State state = new Runtime.State();

    public Form1()
    {
        InitializeComponent();
        this.Width = (int)(scrWidth * zoom + 27);
        this.Height = (int)(scrHeight * zoom + 60);
        pictureBox1.Width = (int)(scrWidth * zoom);
        pictureBox1.Height = (int)(scrHeight * zoom);
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

        
        var roms = new Dictionary<ushort, byte[]>();
        
        string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (assemblyPath != null)
            assemblyPath += "/";

        bool running = true;

        //roms.Add(0xf800, File.ReadAllBytes(assemblyPath + "roms/OriginalF800.rom"));
        roms.Add(0xf800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF800.rom"));
        roms.Add(0xf000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF000.rom"));
        roms.Add(0xe800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE800.rom"));
        roms.Add(0xe000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE000.rom"));
        roms.Add(0xd800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD800.rom"));
        roms.Add(0xd000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD000.rom"));

        memory = new Memory(0xffff, state);

        //memory.ImportImage(File.ReadAllText(assemblyPath + "roms/karateka.bin"), 0x2000);
        memory.RegisterOverlay(new KeyboardOvl());
        memory.RegisterOverlay(new CpuSoftswitchesOvl());
        memory.LoadChars(File.ReadAllBytes(assemblyPath + "roms/CharROM.rom"));
        
        roms.Add(0xc600, File.ReadAllBytes(assemblyPath + "roms/diskinterface.rom"));
        memory.drive = new DiskDrive(assemblyPath + "roms/karateka.dsk", memory);
        memory.RegisterOverlay(new SlotsSoftSwitchesOvl());
        
        foreach (var item in roms)
        {
            memory.WriteAt(item.Key, item.Value);
        }
        List<Task> threads = new List<Task>();
        cpu = new CPU(state, memory, false);
        Keyboard keyboard= new Keyboard(memory, state, lockObj);
        this.KeyDown += keyboard.OnKeyDown;
        this.KeyPress += keyboard.OnKeyPress;    
        cpu.Reset();

        threads.Add(Task.Run(() => {
            while (running)
            {
                lock (lockObj) {
                    cpu.RunCycle();
                }
            }
        }));
        Thread.Sleep(1000);
        threads.Add(Task.Run(() => {
            while (running)
            {
                try 
                {
                    lock (lockObj)
                    {
                        pictureBox1.Image = VideoGenerator.Generate(memory, true);
                    }
                }
                catch (Exception ex) 
                {
                    
                }
                Thread.Sleep(10);
            }
         }));
    }
}