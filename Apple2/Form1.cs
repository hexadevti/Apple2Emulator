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

        //memory.LoadInterfaceROM(0xc600, File.ReadAllBytes(assemblyPath + "roms/diskinterface.rom"));

        memory.RegisterOverlay(new KeyboardOvl());
        memory.RegisterOverlay(new CpuSoftswitchesOvl());
        memory.RegisterOverlay(new SlotsSoftSwitchesOvl());

        memory.LoadChars(File.ReadAllBytes(assemblyPath + "roms/CharROM.rom"));
        
        //memory.drive = new DiskDrive(assemblyPath + "roms/karateka.dsk", memory);

        List<Task> threads = new List<Task>();
        cpu = new CPU(state, memory, false);
        Keyboard keyboard= new Keyboard(memory, state, lockObj);
        this.KeyDown += keyboard.OnKeyDown;
        this.KeyPress += keyboard.OnKeyPress;    
        cpu.Reset();

        //PlayBeep(1000, 1);
        
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

                    // sound.Stream = new MemoryStream(new byte[] { 0x52, 0x49, 0x46, 0x46, 0x29, 0x00, 0x00, 0x00, 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20, 0x10, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x88, 0x13, 0x00, 0x00, 0x88, 0x13, 0x00, 0x00, 0x01, 0x00, 0x08, 0x00, 0x64, 0x61, 0x74, 0x61, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x00, 0x00, 0xff, 0xff, 0x00, 0x00, 0xff, 0xff, 0x00, 0x00 });
                    // sound.Play();
                    //Beep(10000, 2);
                    byte[] buf = new byte[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff,0xff  };
                    buffer.AddSamples(buf, 0, buf.Length);
                        
                    countTicks++;
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

    public static void PlayBeep(UInt16 frequency, int msDuration, UInt16 volume = 16383)
{
    var mStrm = new MemoryStream();
    BinaryWriter writer = new BinaryWriter(mStrm);

    const double TAU = 2 * Math.PI;
    int formatChunkSize = 16;
    int headerSize = 8;
    short formatType = 1;
    short tracks = 1;
    int samplesPerSecond = 5000;
    short bitsPerSample = 8;
    short frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
    int bytesPerSecond = samplesPerSecond * frameSize;
    int waveSize = 4;
    int samples = (int)((decimal)samplesPerSecond * msDuration / 1000);
    int dataChunkSize = samples * frameSize;
    int fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;
    // var encoding = new System.Text.UTF8Encoding();
    writer.Write(0x46464952); // = encoding.GetBytes("RIFF")
    writer.Write(fileSize);
    writer.Write(0x45564157); // = encoding.GetBytes("WAVE")
    writer.Write(0x20746D66); // = encoding.GetBytes("fmt ")
    writer.Write(formatChunkSize);
    writer.Write(formatType);
    writer.Write(tracks);
    writer.Write(samplesPerSecond);
    writer.Write(bytesPerSecond);
    writer.Write(frameSize);
    writer.Write(bitsPerSample);
    writer.Write(0x61746164); // = encoding.GetBytes("data")
    writer.Write(dataChunkSize);
    {
        
        for (int step = 0; step < samples; step++)
        {
            if (step < samples/2)
                writer.Write(0);
            else
                writer.Write(10000);
        }
    }

    mStrm.Seek(0, SeekOrigin.Begin);
    var ddd = mStrm.ToArray();
    foreach (var dd in ddd) {
        Console.Write("0x" + dd.ToString("X2") + ", ");
    }
    new System.Media.SoundPlayer(mStrm).Play();
    
    writer.Close();
    mStrm.Close();
}
}