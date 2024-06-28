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
using System.Diagnostics;
using System.Security;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System;

namespace Apple2;

public partial class Form1 : Form
{
    public Memory? memory { get; set; }
    public CPU? cpu { get; set; }
    Runtime.State state = new Runtime.State();
    bool running = true;
    string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    List<Task> threads = new List<Task>();
    int pixelSize = 2;
    private DirectSoundOut output = new DirectSoundOut();
    public BlockAlignReductionStream? stream;

    public Form1()
    {
        InitializeComponent();

        //pictureBox1.Width = (int)(280 * pixelSize * 1.2);
        //pictureBox1.Height = (int)(192 * pixelSize * 1.2);
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

        if (assemblyPath != null)
            assemblyPath += "/";

        openFileDialog1.FileName = assemblyPath + "roms/teste.dsk";
        string[] parts = openFileDialog1.FileName.Split('\\');
        disk1.Text = parts[parts.Length - 1];
        openFileDialog2.FileName = "";
        PowerOn();
        timerClockSpeed.Interval = 500;
        timerClockSpeed.Start();



        this.Shown += Form1_Shown;
    }

    public void PowerOn()
    {
        running = true;

        memory = new Memory(state);
        memory.adjust1Mhz = false;
        btnClockAdjust.Text = "Max";

        memory.LoadROM(0xf800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF800.rom"));
        memory.LoadROM(0xf000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF000.rom"));
        memory.LoadROM(0xe800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE800.rom"));
        memory.LoadROM(0xe000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE000.rom"));
        memory.LoadROM(0xd800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD800.rom"));
        memory.LoadROM(0xd000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD000.rom"));
        memory.RegisterOverlay(new CpuSoftswitchesOvl());
        memory.RegisterOverlay(new SlotsSoftSwitchesOvl());
        memory.RegisterOverlay(new DiskIISlot6Ovl());
        memory.LoadChars(File.ReadAllBytes(assemblyPath + "roms/CharROM.bin"));
        memory.drive1 = new DiskDrive(openFileDialog1.FileName, memory);
        memory.drive2 = new DiskDrive(openFileDialog2.FileName, memory);
        cpu = new CPU(state, memory);
        Keyboard keyboard = new Keyboard(memory, cpu);
        richTextBox1.KeyDown += keyboard.OnKeyDown;
        richTextBox1.KeyPress += keyboard.OnKeyPress;
        richTextBox1.TextChanged += keyboard.Keyb_TextChanged;


        threads.Add(Task.Run(() =>
        {
            if (memory.adjust1Mhz)
            {


                WaveTone tone = new WaveTone(memory);
                stream = new BlockAlignReductionStream(tone);
                output.Init(stream);
                output.Play();
                while (running)
                {
                    //memory.softswitches.SoundClick = false;
                    Thread.Sleep(100);
                }
            }
        }));

        cpu.Reset();

        threads.Add(Task.Run(() =>
        {
            while (running)
            {
                lock (memory.displayLock)
                {
                    try
                    {
                        pictureBox1.Image = VideoGenerator.Generate(memory, pixelSize, true);
                    }
                    catch { }
                }
                Thread.Sleep(20);
            }
        }));

        threads.Add(Task.Run(() =>
        {
            int countFreq = 0;
            DateTime lastBlock = DateTime.Now;
            DateTime countTime = DateTime.Now;
            DateTime cpuDelay = DateTime.Now;
            memory.cpuCycles = 0;
            int soundCycles = 0;
            Stopwatch sw2;
            float cycleTotalTime = 2.5f;

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 100000000; i++)
                ;
            sw.Stop();

            Console.WriteLine("1000000 in " + sw.Elapsed.TotalMicroseconds.ToString("000000.0") + " microseconds ");
            double microsecondLoops = (int)(100000000f / sw.Elapsed.TotalMicroseconds) - 50;
            Console.WriteLine(microsecondLoops + " loops per microsecond");
            //Thread.Sleep(5000);
            while (running)
            {
                if (memory.adjust1Mhz)
                {
                    sw2 = Stopwatch.StartNew();
                    sw = Stopwatch.StartNew();
                    cpu.deleyloops = cycleTotalTime * microsecondLoops / 2;
                    cpu.RunCycle();
                    sw.Stop();

                    double elepsedCycleTime = (cycleTotalTime - sw.Elapsed.TotalMicroseconds) * microsecondLoops;

                    for (int i = 0; i < (elepsedCycleTime > 0 ? elepsedCycleTime : 0); i++)
                        ;
                    sw2.Stop();

                    //Console.WriteLine(" "+ sw2.Elapsed.TotalMicroseconds.ToString("000.0"));



                    if (soundCycles > 5)
                    {
                        countFreq++;

                        if (memory.softswitches.SoundClick)
                        {
                            memory.clickEvent.Enqueue(0x80);
                        }
                        else
                        {
                            memory.clickEvent.Enqueue(0);
                        }
                        // Sound routine

                        TimeSpan delta2 = DateTime.Now - countTime;
                        if (delta2.TotalMilliseconds >= 1000)
                        {
                            Console.WriteLine("Sound Freq = " + countFreq + " cpu Freq = "
                            + memory.cpuCycles + " Empty Queue = " + memory.EmptyQueue);

                            countFreq = 0;
                            countTime = DateTime.Now;
                            memory.cpuCycles = 0;
                            memory.EmptyQueue = 0;
                        }
                        soundCycles = 0;
                    }
                    else
                    {
                        soundCycles++;
                    }


                }
                else
                {
                    cpu.deleyloops = 0;
                    cpu.RunCycle();
                    
                }
                if (memory.cpuCycles >= 1000000)
                {
                    sw.Stop();
                    memory.clockSpeed = sw.Elapsed.TotalMilliseconds;
                    memory.cpuCycles = 0;
                    sw = Stopwatch.StartNew();
                }
            }

        }));

    }

    private void Form1_Shown(object? sender, EventArgs e)
    {
        richTextBox1.Focus();
    }

    private void button_dsk1_Click(object sender, EventArgs e)
    {
        if (openFileDialog1.ShowDialog() == DialogResult.OK)
        {
            string[] parts = openFileDialog1.FileName.Split('\\');
            disk1.Text = parts[parts.Length - 1];
            if (memory != null)
                memory.drive1 = new DiskDrive(openFileDialog1.FileName, memory);
            richTextBox1.Focus();
        }
    }
    private void button_dsk2_Click(object sender, EventArgs e)
    {
        if (openFileDialog2.ShowDialog() == DialogResult.OK)
        {
            string[] parts = openFileDialog2.FileName.Split('\\');
            disk2.Text = parts[parts.Length - 1];
            if (memory != null)
                memory.drive2 = new DiskDrive(openFileDialog2.FileName, memory);
            richTextBox1.Focus();
        }
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
        richTextBox1.Focus();
    }

    private void btn_restart_Click(object sender, EventArgs e)
    {
        running = false;
        foreach (var item in threads)
        {
            if (item != null)
                item.Wait();
        }
        PowerOn();
        richTextBox1.Focus();
    }

    private void btnClockAdjust_Click(object sender, EventArgs e)
    {
        if (memory != null)
        {
            memory.adjust1Mhz = !memory.adjust1Mhz;
            if (memory.adjust1Mhz)
                btnClockAdjust.Text = "Max";
            else
                btnClockAdjust.Text = "1Mhz";
        }
        richTextBox1.Focus();
    }

    private void timerClockSpeed_Tick(object sender, EventArgs e)
    {
        if (memory != null)
            lblClockSpeed.Text = (1000 / memory.clockSpeed).ToString("0.00") + " Mhz";
    }
}

public class WaveTone : WaveStream
{
    Memory _memory;

    public double frequency { get; set; }

    public WaveTone(Memory memory)
    {
        _memory = memory;
    }
    public override WaveFormat WaveFormat
    {
        get
        {
            return new WaveFormat(44100, 8, 1);
        }
    }

    public override long Length
    {
        get
        {
            return 1;
        }
    }

    public override long Position { get; set; }


    public override int Read(byte[] buffer, int offset, int count)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            if (_memory.clickEvent.Any())
            {
                buffer[i] = _memory.clickEvent.Dequeue();
            }
            else
            {
                buffer[i] = 0;
                _memory.clickEvent.Clear();
                _memory.EmptyQueue++;
            }
        }
        return count;
    }
}
