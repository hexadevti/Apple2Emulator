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
using System.Media;
using System.Net;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;

namespace Apple2;

public partial class Form1 : Form
{
    private Memory memory { get; set; }

    private CPU cpu { get; set; }

    float zoom = 4.0f;
    int scrWidth = 280;
    int scrHeight = 192;

    object lockObj = new object();

    Runtime.State state = new Runtime.State();

    SoundPlayer sound = new SoundPlayer();

    int countTicks = 0;
    int ticksFreq = 0;

    SignalGenerator signalGenerator;

    NAudio.Wave.WaveOut waveOut;

    NAudio.Wave.BufferedWaveProvider buffer;

    DateTime lastClick = DateTime.MinValue;

    DateTime actualClick = DateTime.MinValue;


    public Form1()
    {
        InitializeComponent();
        this.Width = (int)(scrWidth * zoom + 27);
        this.Height = (int)(scrHeight * zoom + 60);
        pictureBox1.Width =  this.Width - 27;
        pictureBox1.Height = this.Height - 60;
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        this.Resize += Form1_Resize;

        string ? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (assemblyPath != null)
            assemblyPath += "/";

        bool running = true;


        memory = new Memory(state);


        memory.LoadROM(0xf800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF800.rom"));
        memory.LoadROM(0xf000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF000.rom"));
        memory.LoadROM(0xe800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE800.rom"));
        memory.LoadROM(0xe000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE000.rom"));
        memory.LoadROM(0xd800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD800.rom"));
        memory.LoadROM(0xd000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD000.rom"));

        memory.LoadInterfaceROM(0xc600, File.ReadAllBytes(assemblyPath + "roms/diskinterface.rom"));

        memory.RegisterOverlay(new KeyboardOvl());
        memory.RegisterOverlay(new CpuSoftswitchesOvl());
        memory.RegisterOverlay(new SlotsSoftSwitchesOvl());

        memory.LoadChars(File.ReadAllBytes(assemblyPath + "roms/CharROM.rom"));

        memory.drive = new DiskDrive(assemblyPath + "roms/ProDOS_2_4_2.dsk", memory);

        List<Task> threads = new List<Task>();
        cpu = new CPU(state, memory, false);
        Keyboard keyboard = new Keyboard(memory, state, lockObj);
        this.KeyDown += keyboard.OnKeyDown;
        this.KeyPress += keyboard.OnKeyPress;
        cpu.Reset();


        // waveOut = new NAudio.Wave.WaveOut();
        // NAudio.Wave.WaveFormat format = new NAudio.Wave.WaveFormat();
        // buffer = new NAudio.Wave.BufferedWaveProvider(format);
        // signalGenerator = new SignalGenerator()
        // {
        //     Frequency = 0,
        //     Gain = 0.2,
        //     Type = SignalGeneratorType.Square
        // };
        // var sampleRate = 16000;
        // var frequency = 500;
        // var amplitude = 0.2;
        // var seconds = 5;

        // var raw = new byte[sampleRate * seconds * 2];

        // var multiple = 2.0*frequency/sampleRate;
        // for (int n = 0; n < sampleRate * seconds; n++)
        // {
        //     var sampleSaw = ((n*multiple)%2) - 1;
        //     var sampleValue = sampleSaw > 0 ? amplitude : -amplitude;
        //     var sample = (short)(sampleValue * Int16.MaxValue);
        //     var bytes = BitConverter.GetBytes(sample);
        //     raw[n*2] = bytes[0];
        //     raw[n*2 + 1] = bytes[1];
        // }

        // var ms = new MemoryStream(raw);
        // var rs = new RawSourceWaveStream(ms, new WaveFormat(sampleRate, 16, 1));

        // var wo = new WaveOutEvent();
        // wo.Init(rs);
        // wo.Play();
        // while (wo.PlaybackState == PlaybackState.Playing)
        // {
        //     Thread.Sleep(500);
        // }
        // wo.Dispose();

        // waveOut.Init(rs);
        // waveOut.Play();


        threads.Add(Task.Run(() =>
        {
            while (running)
            {
                lock (lockObj)
                {
                    cpu.RunCycle();
                }
            }
        }));

        threads.Add(Task.Run(() =>
        {
            while (running)
            {
                pictureBox1.Image = VideoGenerator.Generate(memory, true);
                Thread.Sleep(50);
            }
        }));
        // threads.Add(Task.Run(() =>
        // {
        //     while (running)
        //     {
        //         if (memory.softswitches.SoundClick)
        //         {
        //             actualClick = DateTime.Now;
        //             TimeSpan interval = actualClick - lastClick;
        //             if (interval.Milliseconds < 1000)
        //             {
        //                 Console.WriteLine("Ticks interval (ms) = " + interval.TotalMilliseconds);
        //                 var freq = 1000f / interval.TotalMilliseconds;
        //                 Console.WriteLine("freq (hz) = " + freq);
        //             }
        //             else
        //             {
        //             }


        //             memory.softswitches.SoundClick = false;
        //             lastClick = actualClick;

        //         }
        //     }
        // }));

    }

    private void Form1_Resize(object sender, System.EventArgs e)
    {
        Control control = (Control)sender;
        pictureBox1.Width =  this.Width;
        pictureBox1.Height = this.Height;
    }


}