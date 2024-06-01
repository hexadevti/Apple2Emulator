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

    private CPU cpu { get; set;}
    
    float zoom = 4.0f;
    int scrWidth = 280;
    int scrHeight = 192;

    object lockObj = new object();

    Runtime.State state = new Runtime.State();

    SoundPlayer sound = new SoundPlayer(); 

    int countTicks = 0;
    int ticksFreq = 0;

    SignalGenerator signalGenerator;

    NAudio.Wave.WaveOut playAsio;

    NAudio.Wave.BufferedWaveProvider buffer;

    
    
    [DllImport("kernel32.dll", SetLastError=true)]
    static extern bool Beep(uint dwFreq, uint dwDuration);
    public Form1()
    {
        InitializeComponent();
        this.Width = (int)(scrWidth * zoom + 27);
        this.Height = (int)(scrHeight * zoom + 60);
        pictureBox1.Width = (int)(scrWidth * zoom);
        pictureBox1.Height = (int)(scrHeight * zoom);
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        
        string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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
        
        memory.drive = new DiskDrive(assemblyPath + "roms/teste.dsk", memory);

        // var trk = 0;
        // var sec = 0;
        // Console.WriteLine("Data from " + trk + "," + sec);
        // var dataOrig = memory.drive.GetSectorData(trk, sec);
        // foreach (var sector in dataOrig) 
        // {   
        //     Console.Write(sector.ToString("X2") + " ");
        // }
        // var nibbled = memory.drive.Encode6_2(trk, sec);
        // Console.WriteLine("-----------------------------------------------");
        // Console.WriteLine("reconverted from " + trk + "," + sec);
        // var reconverted = memory.drive.Decode6_2(nibbled);
        // foreach (var sector in reconverted) 
        // {   
        //     Console.Write(sector.ToString("X2") + " ");
        // }
        // // Console.WriteLine("-----------------------------------------------");
        // // Console.WriteLine("Nibbled from " + trk + "," + sec);
        // // foreach (var sector in nibbled) 
        // // {   
        // //     Console.Write(sector.ToString("X2") + " ");
        // // }



        List<Task> threads = new List<Task>();
        cpu = new CPU(state, memory, false);
        Keyboard keyboard= new Keyboard(memory, state, lockObj);
        this.KeyDown += keyboard.OnKeyDown;
        this.KeyPress += keyboard.OnKeyPress;    
        cpu.Reset();
        
        // WaveOut waveOut;
        // waveOut = new WaveOut();
        // waveOut.DeviceNumber = 0;
        // signalGenerator = new SignalGenerator();
        // signalGenerator.Type = SignalGeneratorType.Square;
        // signalGenerator.
        // signalGenerator.Frequency = 400;
        // signalGenerator.FrequencyEnd = 400;
        // signalGenerator.SweepLengthSecs = 1;
        // waveOut.Init(signalGenerator);
        // waveOut.Play();
        
        playAsio = new NAudio.Wave.WaveOut();
        
        NAudio.Wave.WaveFormat formato = new NAudio.Wave.WaveFormat();
        
        buffer = new NAudio.Wave.BufferedWaveProvider(formato);
        playAsio.Init(buffer);
        playAsio.Play();
        
        

        threads.Add(Task.Run(() => {
            while (running)
            {
                lock (lockObj) {
                    cpu.RunCycle();
                }
            }
        }));
        
        threads.Add(Task.Run(() => {
            while (running)
            {
                try 
                {
                    pictureBox1.Image = VideoGenerator.Generate(memory, true);
                    Thread.Sleep(50);
                }
                catch (Exception ex) 
                {
                    
                }
                Thread.Sleep(10);
            }
         }));
         threads.Add(Task.Run(() => {
            while (running)
            {
                if (memory.softswitches.SoundClick)
                {
                    byte[] buf = new byte[] { 0,0,0,0,0,0,0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,15,15,15,14,13,12,11,10,9,8,7,6,5,4,3,2,1,0,0,0,0,0,0 };
                    buffer.AddSamples(buf, 0, buf.Length);
                    Console.WriteLine(playAsio.Volume);
                    memory.softswitches.SoundClick = false;
                }
            }
         }));
        //  threads.Add(Task.Run(() => {
        //     int newTicks = 0;
        //     uint freq = 0;
        //     while (running)
        //     {
        //         int intervalAnalysis = 100;
        //         if (countTicks > newTicks) {
        //             Console.WriteLine("Ticks = " + countTicks);
        //             if (countTicks > 0)
        //             {
        //                 freq = Convert.ToUInt16((float)countTicks/((float)intervalAnalysis/1000f));
        //                 Console.WriteLine("Hz = " + freq);
        //                 //PlayBeep((ushort)freq, 100);

                        
        //                 byte[] buf = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,
        //                 0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,0, 0, 0, 0, 0, 0, 0, 0, 0xff, 0, 0xff, 0xff, 0xff, 0, 0, 0xff, 0, 0xff, 0, 0xff,
        //                  };
                        
        //                     //Aggiungo in coda al buffer
        //                     buffer.AddSamples(buf, 0, buf.Length);
                        
                        
                        
        //                 // signalGenerator = new SignalGenerator();
        //                 // signalGenerator.Type = SignalGeneratorType.Square;
        //                 // signalGenerator.Frequency = freq;
        //                 // signalGenerator.FrequencyEnd = freq;
        //                 // signalGenerator.SweepLengthSecs = 1;

        //                 // waveOut.Init(signalGenerator);
        //                 // waveOut.Play();
        //                 // waveOut.Stop();
                        
        //             }
                    
        //             countTicks = 0;
        //         }
        //         newTicks = countTicks;
                
        //         Thread.Sleep(intervalAnalysis);
        //     }
        //  }));
    }

    
}