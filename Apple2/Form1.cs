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
    private Memory memory { get; set; }

    private CPU cpu { get; set; }

    Runtime.State state = new Runtime.State();



    bool running = true;

    string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    List<Task> threads = new List<Task>();

    int pixelSize = 2;

    private DirectSoundOut output = null;
    private BlockAlignReductionStream stream = null;

    public Form1()
    {
        InitializeComponent();

        //pictureBox1.Width = (int)(280 * pixelSize * 1.2);
        //pictureBox1.Height = (int)(192 * pixelSize * 1.2);
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

        if (assemblyPath != null)
            assemblyPath += "/";

        openFileDialog1.FileName = assemblyPath + "roms/karateka.dsk";
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

        output = new DirectSoundOut();


        running = true;


        memory = new Memory(state);
        memory.adjust1Mhz = true;


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


        cpu = new CPU(state, memory, false);
        Keyboard keyboard = new Keyboard(memory, state);


        richTextBox1.KeyDown += keyboard.OnKeyDown;
        richTextBox1.KeyPress += keyboard.OnKeyPress;
        richTextBox1.TextChanged += keyboard.Keyb_TextChanged;


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
                WaveTone tone = new WaveTone(memory);
                stream = new BlockAlignReductionStream(tone);
                output.Init(stream);
                output.Play();
                while (running)
                {
                    Thread.Sleep(10);
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
        memory.adjust1Mhz = !memory.adjust1Mhz;
        if (memory.adjust1Mhz)
            btnClockAdjust.Text = "Max";
        else
            btnClockAdjust.Text = "1Mhz";
        richTextBox1.Focus();
    }

    private void timerClockSpeed_Tick(object sender, EventArgs e)
    {
        lblClockSpeed.Text = (1000 / memory.clockSpeed).ToString("0.00") + " Mhz";
    }
}

public class WaveTone : WaveStream
{
    Memory _memory;


    public double frequency { get; set; }
    private double amplitude;
    private double time;

    private long clickCount;

    private bool emptyStream = true;

    private long click;

    private long nextClick;


    public WaveTone(Memory memory)
    {
        _memory = memory;
        clickCount = 0;

    }
    public override WaveFormat WaveFormat
    {
        get
        {
            return new WaveFormat(88200, 8, 1);
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
            buffer[i] = (byte)(_memory.clickEvent.Any() && _memory.clickEvent.Dequeue() ? 0xff : 0x00);
        }
        return count;
    }
}
