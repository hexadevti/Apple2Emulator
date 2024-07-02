using Runtime;
using System.Reflection;
using NAudio.Wave;
using System.Diagnostics;

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

    public Speaker tone;

    public Form1()
    {
        InitializeComponent();
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

        if (assemblyPath != null)
            assemblyPath += "/";

        openFileDialog1.FileName = assemblyPath + "roms/teste.dsk";
        string[] parts = openFileDialog1.FileName.Split('\\');
        disk1.Text = parts[parts.Length - 1];
        openFileDialog2.FileName = "";
        timerClockSpeed.Interval = 500;
        timerClockSpeed.Start();
        this.Shown += Form1_Shown;
        memory = new Memory(state);
        memory.adjust1Mhz = true;
        btnClockAdjust.Text = "Max";
        cpu = new CPU(state, memory);
        Keyboard keyboard = new Keyboard(memory, cpu);
        richTextBox1.KeyDown += keyboard.OnKeyDown;
        richTextBox1.KeyPress += keyboard.OnKeyPress;
        richTextBox1.TextChanged += keyboard.Keyb_TextChanged;
        memory.LoadROM(0xf800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF800.rom"));
        memory.LoadROM(0xf000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF000.rom"));
        memory.LoadROM(0xe800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE800.rom"));
        memory.LoadROM(0xe000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE000.rom"));
        memory.LoadROM(0xd800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD800.rom"));
        memory.LoadROM(0xd000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD000.rom"));
        memory.LoadSlotsROM(0xc600, File.ReadAllBytes(assemblyPath + "roms/diskinterface.rom"));
        memory.LoadChars(File.ReadAllBytes(assemblyPath + "roms/CharROM.bin"));
        memory.drive1 = new DiskDrive(openFileDialog1.FileName, memory);
        memory.drive2 = new DiskDrive(openFileDialog2.FileName, memory);
        running = true;
        cpu.WarmStart();
        LoadThreads();
    }

    public void LoadThreads()
    {

        threads.Add(Task.Run(() =>
        {
            StartSpeaker();
            while (running)
            {
                Thread.Sleep(500);
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
                        pictureBox1.Image = Video.Generate(memory, pixelSize, true);
                    }
                    catch { }
                }
                Thread.Sleep(20);
            }
        }));

        threads.Add(Task.Run(() => cpu.DelayedRun(running)));

    }

    private void StartSpeaker()
    {
        tone = new Speaker(memory);
        stream = new BlockAlignReductionStream(tone);
        output.Init(stream);
        output.Play();
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
        cpu.WarmStart();
        richTextBox1.Focus();
    }

    private void btnClockAdjust_Click(object sender, EventArgs e)
    {
        if (memory != null)
        {
            memory.adjust1Mhz = !memory.adjust1Mhz;
            if (memory.adjust1Mhz)
            {
                StartSpeaker();
                btnClockAdjust.Text = "Max";
            }
            else
            {
                stream.Dispose();
                output.Dispose();
                btnClockAdjust.Text = "1Mhz";
            }
        }
        richTextBox1.Focus();
    }

    private void timerClockSpeed_Tick(object sender, EventArgs e)
    {
        if (memory != null)
        {
            lblClockSpeed.Text = (1000 / memory.clockSpeed).ToString("0.00") + " Mhz";
            string text = "";

            while (memory.newText.TryDequeue(out text))
            {
                richTextBox2.AppendText(text + Environment.NewLine);
            }
        }
    }
}
