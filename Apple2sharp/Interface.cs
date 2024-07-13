using System.Reflection;
using NAudio.Wave;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Threading;
using Apple2.Mainboard;
using Apple2.IO;
using Apple2.CPU;
using Apple2.Mainboard.Interfaces;
using Apple2.Mainboard.Cards;
using Apple2.Mainboard.Enums;
using Apple2.CPU.Mos6502;

namespace Apple2
{
    public partial class Interface : Form
    {
        const int pixelSize = 2;
        private Apple2Board mainBoard { get; set; }
        private IProcessor cpu { get; set; }
        private State state = new State();

        private string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private List<Task> threads = new List<Task>();
        private DirectSoundOut soundOutput = new DirectSoundOut();
        public Speaker speaker;
        private WaveFormat waveFormat;
        private List<ComboBox> cbSlots = new List<ComboBox>();
        private List<List<KeyValuePair<string, string>>> cbDatasource = new List<List<KeyValuePair<string, string>>>();

        public Interface()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            if (assemblyPath != null)
                assemblyPath += "/";

            this.Shown += Form1_Shown;
            mainBoard = new Apple2Board();
            mainBoard.adjust1Mhz = true;
            btnClockAdjust.Text = "Fast";
            cpu = new Mos6502(state, mainBoard);
            Keyboard keyboard = new Keyboard(mainBoard, cpu);
            richTextBox1.KeyDown += keyboard.OnKeyDown;
            richTextBox1.TextChanged += keyboard.Keyb_TextChanged;
            mainBoard.LoadROM(0xf800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF800.bin"));
            mainBoard.LoadROM(0xf000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF000.bin"));
            mainBoard.LoadROM(0xe800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE800.bin"));
            mainBoard.LoadROM(0xe000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE000.bin"));
            mainBoard.LoadROM(0xd800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD800.bin"));
            mainBoard.LoadROM(0xd000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD000.bin"));
            LoadCardsCombos();
            LoadContext();
            mainBoard.LoadChars(File.ReadAllBytes(assemblyPath + "roms/CharROM.bin"));
            this.FormClosing += FormCloseEvent;
            tbSpeed.Enabled = false;
            tbSpeed.ValueChanged += tbSpeed_ValueChanged;
            StartSpeaker();
            InitSlots();
            cpu.WarmStart();
            LoadThreads();
        }

        private void LoadContext()
        {
            string Disk1Path = Apple2plus.Properties.Settings.Default.Disk1Path;
            if (!string.IsNullOrEmpty(Disk1Path))
            {
                openFileDialog1.FileName = Disk1Path;
                string[] parts1 = openFileDialog1.FileName.Split('\\');
                disk1.Text = parts1[parts1.Length - 1];
            }
            else
            {
                openFileDialog1.FileName = "";
                disk1.Text = "";
            }
            string Disk2Path = Apple2plus.Properties.Settings.Default.Disk2Path;
            if (!string.IsNullOrEmpty(Disk2Path))
            {
                openFileDialog2.FileName = Disk2Path;
                string[] parts2 = openFileDialog2.FileName.Split('\\');
                disk2.Text = parts2[parts2.Length - 1];
            }
            else
            {
                openFileDialog2.FileName = "";
                disk2.Text = "";
            }


            for (int i = 0; i < 8; i++)
                cbSlots[i].SelectedValue = Apple2plus.Properties.Settings.Default["Slot" + i + "Card"];
        }
        private void LoadCardsCombos()
        {
            cbSlots = new List<ComboBox>() { cbSlot0, cbSlot1, cbSlot2, cbSlot3, cbSlot4, cbSlot5, cbSlot6, cbSlot7 };
            cbDatasource = new List<List<KeyValuePair<string, string>>>() {
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("LanguageCard","Language Card"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128k RAM Card"),
                    new KeyValuePair<string, string>("EmptySlot","Empty")
                },
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("EmptySlot","Empty"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128k RAM Card")
                },
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("EmptySlot","Empty"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128k RAM Card")
                },
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("Cols80Card","Videx 80 Column Card"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128,k RAM Card"),
                    new KeyValuePair<string, string>("EmptySlot","Empty")
                },
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("EmptySlot","Empty"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128k RAM Card")
                },
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("EmptySlot","Empty"),
                    new KeyValuePair<string, string>("DiskIICard","Disk II Card"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128k RAM Card")
                },
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("DiskIICard","Disk II Card"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128k RAM Card"),
                    new KeyValuePair<string, string>("EmptySlot","Empty")
                },
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("EmptySlot","Empty"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128k RAM Card")
                }
            };

            for (int i = 0; i < 8; i++)
            {
                cbSlots[i].DataSource = cbDatasource[i];
                cbSlots[i].DisplayMember = "Value";
                cbSlots[i].ValueMember = "Key";
                cbSlots[i].SelectedValueChanged += cbSlot_SelectedValueChanged;
            }

        }

        private void cbSlot_SelectedValueChanged(object? sender, EventArgs e)
        {
            string settings = ((ComboBox)sender).Name.Replace("cb", "") + "Card";
            Apple2plus.Properties.Settings.Default[settings] = ((ComboBox)sender).SelectedValue;
            Apple2plus.Properties.Settings.Default.Save();

        }

        private void InitSlots()
        {
            for (int i = 0; i < 8; i++)
                mainBoard.slots[i] = (ICard)GetInstance(cbSlots[i].SelectedValue.ToString(), i);
        }

        public object? GetInstance(string type, int slot)
        {
            if (type == "LanguageCard")
                return new LanguageCard();
            else if (type == "RamCard")
                return new RamCard(slot, 8);
            else if (type == "Cols80Card")
                return new Cols80Card(3, Tools.LoadROM(File.ReadAllBytes(assemblyPath + "roms/Videx Videoterm ROM 2.4.bin"), 0x300),
                                            Tools.LoadExtendedSlotsROM(0xc800, File.ReadAllBytes(assemblyPath + "roms/Videx Videoterm ROM 2.4.bin")),
                                            Tools.Load80Chars(File.ReadAllBytes(assemblyPath + "roms/Videx Videoterm Character ROM Normal.bin")));
            else if (type == "DiskIICard")
                return new DiskIICard(slot, File.ReadAllBytes(assemblyPath + "roms/DiskIICardRom.bin"), openFileDialog1.FileName, openFileDialog2.FileName);
            else
                return new EmptySlot();

        }

        private void tbSpeed_ValueChanged(object? sender, EventArgs e)
        {
            mainBoard.clickBuffer.Clear();
            richTextBox1.Focus();
        }

        private void FormCloseEvent(object? sender, FormClosingEventArgs e)
        {
            cpu.cpuState = CpuState.Stopped;
        }


        public void LoadThreads()
        {
            threads.Add(Task.Run(() =>
            {
                while (cpu.cpuState != CpuState.Stopped)
                {
                    mainBoard.audioJumpInterval = ReadTrackBar(tbSpeed);
                    SetLabel(lblClockSpeed, (1000 / mainBoard.clockSpeed).ToString("0.00") + " Mhz");
                    DiskIICard actualDiskCard = null;
                    if (mainBoard.slots[5].GetType() == typeof(DiskIICard))
                        actualDiskCard = (DiskIICard)mainBoard.slots[5];
                    else if (mainBoard.slots[6].GetType() == typeof(DiskIICard))
                        actualDiskCard = (DiskIICard)mainBoard.slots[6];

                    if (actualDiskCard != null)
                    {
                        SetLabel(D1T, "T: " + actualDiskCard.drive1.track.ToString());
                        SetLabel(D1S, "S: " + (actualDiskCard.drive1.sector > 16 ? "?" : actualDiskCard.drive1.sector.ToString()));
                        SetCheckbox(D1O, actualDiskCard.drive1.on);
                        SetLabel(D2T, "T: " + actualDiskCard.drive2.track.ToString());
                        SetLabel(D2S, "S: " + (actualDiskCard.drive2.sector > 16 ? "?" : actualDiskCard.drive2.sector.ToString()));
                        SetCheckbox(D2O, actualDiskCard.drive2.on);
                    }
                    string text = "";
                    for (int i = 0; i < mainBoard.screenLog.Count; i++)
                    {
                        if (mainBoard.screenLog.TryDequeue(out text))
                            SetRichTextBox(richTextBox2, text + Environment.NewLine);
                    }
                    Thread.Sleep(50);
                }
            }));


            threads.Add(Task.Run(() =>
            {
                while (cpu.cpuState != CpuState.Stopped)
                {
                    lock (mainBoard.displayLock)
                    {
                        try
                        {
                            if (mainBoard.softswitches.Cols40_80)
                                pictureBox1.Image = Video.Generate(mainBoard, pixelSize);
                            else
                            {
                                if (mainBoard.slots[3].GetType() == typeof(Cols80Card))
                                    pictureBox1.Image = ((Cols80Card)mainBoard.slots[3]).Generate(mainBoard, pixelSize);
                            }
                        }
                        catch { }
                    }
                    Thread.Sleep(50);
                }
            }));

            threads.Add(Task.Run(() => cpu.Run()));
        }

        private void StartSpeaker()
        {
            waveFormat = new WaveFormat(120000, 8, 1);
            mainBoard.audioBuffer = 4800;
            speaker = new Speaker(mainBoard, waveFormat);
            soundOutput.Init(speaker);
            soundOutput.Play();
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
                Apple2plus.Properties.Settings.Default["Disk1Path"] = openFileDialog1.FileName;
                Apple2plus.Properties.Settings.Default.Save();
                UpdateDisks();
                richTextBox1.Focus();
            }
        }
        private void button_dsk2_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                string[] parts = openFileDialog2.FileName.Split('\\');
                disk2.Text = parts[parts.Length - 1];
                Apple2plus.Properties.Settings.Default["Disk2Path"] = openFileDialog2.FileName;
                Apple2plus.Properties.Settings.Default.Save();
                UpdateDisks();
                richTextBox1.Focus();
            }
        }

        private void UpdateDisks()
        {
            ICard actualDiskCard = null;
            if (mainBoard.slots[5].GetType() == typeof(DiskIICard))
                actualDiskCard = (DiskIICard)mainBoard.slots[5];
            else if (mainBoard.slots[6].GetType() == typeof(DiskIICard))
                actualDiskCard = (DiskIICard)mainBoard.slots[6];

            if (actualDiskCard != null)
            {
                ((DiskIICard)actualDiskCard).drive1 = new DiskDrive(openFileDialog1.FileName, (DiskIICard)actualDiskCard);
                ((DiskIICard)actualDiskCard).drive2 = new DiskDrive(openFileDialog2.FileName, (DiskIICard)actualDiskCard);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            richTextBox1.Focus();
        }

        private void btn_restart_Click(object sender, EventArgs e)
        {
            InitSlots();
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
                control.Text = text;
        }

        public static int ReadTrackBar(TrackBar control)
        {
            if (control.InvokeRequired)
                return (int)control.Invoke(new Func<int>(() => ReadTrackBar(control)));
            else
                return control.Value;
        }

        public void SetCheckbox(CheckBox control, bool check)
        {
            if (D1T.InvokeRequired)
            {
                Action safeWrite = delegate { SetCheckbox(control, check); };
                control.Invoke(safeWrite);
            }
            else
                control.Checked = check;
        }

        public void SetRichTextBox(RichTextBox control, string text)
        {
            if (control.InvokeRequired)
            {
                Action safeWrite = delegate { SetRichTextBox(control, text); };
                control.Invoke(safeWrite);
            }
            else
            {
                control.AppendText(text);
                control.ScrollToCaret();
            }

        }

        private void ckbColor_Click(object sender, EventArgs e)
        {
            if (mainBoard != null)
                mainBoard.videoColor = ckbColor.Checked;
        }
    }
}