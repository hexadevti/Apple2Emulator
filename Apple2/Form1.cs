using Runtime;
using System.Reflection;
using NAudio.Wave;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
using System.ComponentModel;

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

    public System.Windows.Forms.MethodInvoker inv;

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
        this.Shown += Form1_Shown;
        memory = new Memory(state);
        memory.adjust1Mhz = true;
        btnClockAdjust.Text = "Fast";
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
        memory.LoadExtendedSlotsROM(0xc800, File.ReadAllBytes(assemblyPath + "roms/Videx Videoterm ROM 2.4.bin"));
        memory.LoadSlotsROM(0xc300, File.ReadAllBytes(assemblyPath + "roms/Videx Videoterm ROM 2.4.bin"), 0x300);
        memory.LoadChars(File.ReadAllBytes(assemblyPath + "roms/CharROM.bin"));
        memory.Load80Chars(File.ReadAllBytes(assemblyPath + "roms/Videx Videoterm Character ROM Normal.bin"));
        memory.drive1 = new DiskDrive(openFileDialog1.FileName, memory);
        memory.drive2 = new DiskDrive(openFileDialog2.FileName, memory);
        this.FormClosing+=FormCloseEvent;
        tbSpeed.Enabled = false;
        tbSpeed.ValueChanged += tbSpeed_ValueChanged;
        running = true;
        StartSpeaker();
        cpu.WarmStart();
        LoadThreads();
    }

    private void tbSpeed_ValueChanged(object? sender, EventArgs e)
    {
        memory.clickBuffer.Clear();
        richTextBox1.Focus();
    }

    private void FormCloseEvent(object? sender, FormClosingEventArgs e)
    {
        running = false;
    }
        

    public void LoadThreads()
    {
        threads.Add(Task.Run(() =>
        {
            while (running)
            {
                memory.audioJumpInterval = ReadTabBar(tbSpeed);
                SetLabel(lblClockSpeed, (1000 / memory.clockSpeed).ToString("0.00") + " Mhz");
                SetLabel(D1T, "T: " + memory.drive1.track.ToString());
                SetLabel(D1S, "S: " + memory.drive1.sector.ToString());
                SetCheckbox(D1O, memory.drive1.on);
                SetLabel(D2T, "T: " + memory.drive2.track.ToString());
                SetLabel(D2S, "S: " + memory.drive2.sector.ToString());
                SetCheckbox(D2O, memory.drive2.on);
                string text = "";
                if (memory.screenLog.TryDequeue(out text))
                    SetRichTextBox(richTextBox2, text + Environment.NewLine);
                Thread.Sleep(100);

                // for (int j = 0;j < 24;j++)
                // {
                //     for (int i = 0;i < 80;i++)
                //     {
                //         ushort pos = (ushort)((i + (j * 0x50) + memory.baseRAM[0x6fb] * 0x10) % 0x800);
                //         Console.SetCursorPosition(i, j);
                //         Console.Write(System.Text.Encoding.ASCII.GetString(new [] { memory.cols80RAM[pos] }));
                //     }
                // }

                // Console.SetCursorPosition(0, 25);
                // Console.WriteLine(memory.baseRAM[0x4fb].ToString("X2") + memory.baseRAM[0x47b].ToString("X2"));
                // Console.SetCursorPosition(0, 26);
                // Console.WriteLine(memory.baseRAM[0x6fb].ToString("X2"));
                // Console.SetCursorPosition(0, 27);
                // Console.WriteLine(memory.baseRAM[0x5fb].ToString("X4"));
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
                        if (memory.softswitches.Cols40_80)
                            pictureBox1.Image = Video.Generate(memory, pixelSize, true);
                        else
                            pictureBox1.Image = Cols80Video.Generate(memory, pixelSize, true);
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
                memory.clickBuffer.Clear();
                tbSpeed.Value = 1;
                tbSpeed.Enabled = false;
                btnClockAdjust.Text = "Fast";
            }
            else
            {

                memory.clickBuffer.Clear();
                tbSpeed.Value = 10;
                tbSpeed.Enabled = true;
                btnClockAdjust.Text = "1Mhz";
            }
        }
        richTextBox1.Focus();
    }

    public void SetLabel(Control control, string text)
    {
        if (D1T.InvokeRequired)
        {
            Action safeWrite = delegate { SetLabel(control, text); };
            control.Invoke(safeWrite);
        }
        else
        {
            control.Text = text;
        }
            
    }

    public static int ReadTabBar(TrackBar control)
    {
        if (control.InvokeRequired)
        {
            return (int)control.Invoke(new Func<int>(() => ReadTabBar(control)));
        }
        else
        {
            return control.Value;
        }
            
    }

    public void SetCheckbox(CheckBox control, bool check)
    {
        if (D1T.InvokeRequired)
        {
            Action safeWrite = delegate { SetCheckbox(control, check); };
            control.Invoke(safeWrite);
        }
        else
        {
            control.Checked = check;
        }
            
    }

    public void SetRichTextBox(RichTextBox control, string text)
    {
        if (control.InvokeRequired)
        {
            Action safeWrite = delegate { SetLabel(control, text); };
            control.Invoke(safeWrite);
        }
        else
        {
            control.AppendText(text);
        }
            
    }


    
}