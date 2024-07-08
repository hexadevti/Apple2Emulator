using Runtime;
using System.Reflection;
using NAudio.Wave;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
using System.ComponentModel;
using Runtime.Cards;

namespace Apple2;

public partial class Interface : Form
{
    public MainBoard? mainBoard { get; set; }
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

    public Interface()
    {
        InitializeComponent();
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

        if (assemblyPath != null)
            assemblyPath += "/";

        openFileDialog1.FileName = assemblyPath + "roms/Apple DOS 3.3 January 1983.dsk";
        string[] parts = openFileDialog1.FileName.Split('\\');
        disk1.Text = parts[parts.Length - 1];
        openFileDialog2.FileName = "";
        this.Shown += Form1_Shown;
        mainBoard = new MainBoard(state);
        mainBoard.adjust1Mhz = true;
        btnClockAdjust.Text = "Fast";
        cpu = new CPU(state, mainBoard);
        Keyboard keyboard = new Keyboard(mainBoard, cpu);
        richTextBox1.KeyDown += keyboard.OnKeyDown;
        richTextBox1.KeyPress += keyboard.OnKeyPress;
        richTextBox1.TextChanged += keyboard.Keyb_TextChanged;
        mainBoard.LoadROM(0xf800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF800.rom"));
        mainBoard.LoadROM(0xf000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF000.rom"));
        mainBoard.LoadROM(0xe800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE800.rom"));
        mainBoard.LoadROM(0xe000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE000.rom"));
        mainBoard.LoadROM(0xd800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD800.rom"));
        mainBoard.LoadROM(0xd000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD000.rom"));
        mainBoard.slot0 = new LanguageCard();
        mainBoard.slot1 = new EmptySlot(1);
        mainBoard.slot2 = new EmptySlot(2);
        mainBoard.slot3 = new Cols80Card(3, Tools.LoadROM(File.ReadAllBytes(assemblyPath + "roms/Videx Videoterm ROM 2.4.bin"), 0x300),
                                        Tools.LoadExtendedSlotsROM(0xc800, File.ReadAllBytes(assemblyPath + "roms/Videx Videoterm ROM 2.4.bin")),
                                        Tools.Load80Chars(File.ReadAllBytes(assemblyPath + "roms/Videx Videoterm Character ROM Normal.bin")));
        mainBoard.slot4 = new EmptySlot(4);
        mainBoard.slot5 = new EmptySlot(5);
        mainBoard.slot6 = new DiskIICard(6, File.ReadAllBytes(assemblyPath + "roms/diskinterface.rom"),
                                        new DiskDrive(openFileDialog1.FileName, (DiskIICard)mainBoard.slot6),
                                        new DiskDrive(openFileDialog2.FileName, (DiskIICard)mainBoard.slot6));
        mainBoard.slot7 = new EmptySlot(7);
        mainBoard.LoadChars(File.ReadAllBytes(assemblyPath + "roms/CharROM.bin"));
        this.FormClosing += FormCloseEvent;
        tbSpeed.Enabled = false;
        tbSpeed.ValueChanged += tbSpeed_ValueChanged;
        running = true;
        StartSpeaker();
        cpu.WarmStart();
        LoadThreads();
    }

    private void tbSpeed_ValueChanged(object? sender, EventArgs e)
    {
        mainBoard.clickBuffer.Clear();
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
                mainBoard.audioJumpInterval = ReadTabBar(tbSpeed);
                SetLabel(lblClockSpeed, (1000 / mainBoard.clockSpeed).ToString("0.00") + " Mhz");
                SetLabel(D1T, "T: " + ((DiskIICard)mainBoard.slot6).drive1.track.ToString());
                SetLabel(D1S, "S: " + ((DiskIICard)mainBoard.slot6).drive1.sector.ToString());
                SetCheckbox(D1O, ((DiskIICard)mainBoard.slot6).drive1.on);
                SetLabel(D2T, "T: " + ((DiskIICard)mainBoard.slot6).drive2.track.ToString());
                SetLabel(D2S, "S: " + ((DiskIICard)mainBoard.slot6).drive2.sector.ToString());
                SetCheckbox(D2O, ((DiskIICard)mainBoard.slot6).drive2.on);
                string text = "";
                if (mainBoard.screenLog.TryDequeue(out text))
                    SetRichTextBox(richTextBox2, text + Environment.NewLine);
                Thread.Sleep(100);
            }
        }));


        threads.Add(Task.Run(() =>
        {
            while (running)
            {
                lock (mainBoard.displayLock)
                {
                    try
                    {
                        if (mainBoard.softswitches.Cols40_80)
                            pictureBox1.Image = Video.Generate(mainBoard, pixelSize);
                        else
                            pictureBox1.Image = ((Cols80Card)mainBoard.slot3).Generate(mainBoard, pixelSize);
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
        tone = new Speaker(mainBoard);
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
            ((DiskIICard)mainBoard.slot6).drive1 = new DiskDrive(openFileDialog1.FileName, (DiskIICard)mainBoard.slot6);
            richTextBox1.Focus();
        }
    }
    private void button_dsk2_Click(object sender, EventArgs e)
    {
        if (openFileDialog2.ShowDialog() == DialogResult.OK)
        {
            string[] parts = openFileDialog2.FileName.Split('\\');
            disk2.Text = parts[parts.Length - 1];
            ((DiskIICard)mainBoard.slot6).drive2 = new DiskDrive(openFileDialog2.FileName, (DiskIICard)mainBoard.slot6);
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
        if (mainBoard != null)
        {
            mainBoard.adjust1Mhz = !mainBoard.adjust1Mhz;
            if (mainBoard.adjust1Mhz)
            {
                mainBoard.clickBuffer.Clear();
                tbSpeed.Value = 1;
                tbSpeed.Enabled = false;
                btnClockAdjust.Text = "Fast";
            }
            else
            {

                mainBoard.clickBuffer.Clear();
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

    private void ckbColor_Click(object sender, EventArgs e)
    {
        if (mainBoard != null)
            mainBoard.videoColor = ckbColor.Checked;
    }
}