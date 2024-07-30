using System.Reflection;
using NAudio.Wave;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Threading;
using Apple2Sharp.Mainboard;
using Apple2Sharp.IO;
using Apple2Sharp.CPU65C02;
using Apple2Sharp.CPU6502;
using Apple2Sharp.Mainboard.Interfaces;
using Apple2Sharp.Mainboard.Cards;
using Apple2Sharp.Mainboard.Enums;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Linq;

namespace Apple2Sharp
{
    public partial class Interface : Form
    {
        const int pixelSize = 4;
        private bool formRunning = true;
        private Apple2Board mainBoard { get; set; }
        private IProcessor cpu { get; set; }

        private Clock clock { get; set; }
        private CPU6502.State state = new CPU6502.State();
        private CPU65C02.State stateC = new CPU65C02.State();

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
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            if (assemblyPath != null)
                assemblyPath += "/";

            this.Shown += Form1_Shown;
            this.FormClosing += FormCloseEvent;
            tbSpeed.ValueChanged += tbSpeed_ValueChanged;


            mainBoard = new Apple2Board();
            LoadCardsCombos();
            LoadContext();
            InitSlots();
            LoadCPU();
            LoadKeyboard();
            StartSpeaker();
            CPUThread();
            cpu.WarmStart();
            LoadThreads();




        }

        private void LoadKeyboard()
        {
            Keyboard keyboard = new Keyboard(mainBoard, cpu);
            richTextBox1.KeyDown += keyboard.OnKeyDown;
            richTextBox1.KeyUp += keyboard.OnKeyUp;
            richTextBox1.TextChanged += keyboard.Keyb_TextChanged;

        }
        private void LoadCPU()
        {
            if (mainBoard.appleIIe)
            {
                // Apple IIe
                cpu = new CPU65C02.CPU65C02(stateC, mainBoard);
                mainBoard.LoadAppleIIeInternalROM(0xc000, File.ReadAllBytes(assemblyPath + "roms/AppleIIeEnhancedC0-FF.bin"));
                mainBoard.LoadIIeChars(File.ReadAllBytes(assemblyPath + "roms/AppleIIeVideoEnhanced.bin"));
            }
            else
            {
                // Apple II+
                cpu = new CPU6502.CPU6502(state, mainBoard);
                mainBoard.LoadROM(0xf800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF800.bin"));
                mainBoard.LoadROM(0xf000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF000.bin"));
                mainBoard.LoadROM(0xe800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE800.bin"));
                mainBoard.LoadROM(0xe000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE000.bin"));
                mainBoard.LoadROM(0xd800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD800.bin"));
                mainBoard.LoadROM(0xd000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD000.bin"));
                mainBoard.LoadChars(File.ReadAllBytes(assemblyPath + "roms/CharROM.bin"));
            }
            clock = new Clock(cpu, mainBoard);
        }

        private void LoadContext()
        {
            string Disk1Path = Apple2Sharp.Properties.Settings.Default.Disk1Path;
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
            string Disk2Path = Apple2Sharp.Properties.Settings.Default.Disk2Path;
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
            {
                if (string.IsNullOrEmpty(Apple2Sharp.Properties.Settings.Default["Slot" + i + "Card"].ToString()))
                    cbSlots[i].SelectedValue = "EmptySlot";
                else
                    cbSlots[i].SelectedValue = Apple2Sharp.Properties.Settings.Default["Slot" + i + "Card"];
            }

            mainBoard.videoColor = Convert.ToBoolean(Apple2Sharp.Properties.Settings.Default["Color"]);
            mainBoard.adjust1Mhz = Convert.ToBoolean(Apple2Sharp.Properties.Settings.Default["Adjust1mhz"]);
            mainBoard.scanLines = Convert.ToBoolean(Apple2Sharp.Properties.Settings.Default["ScanLines"]);
            mainBoard.idealized = Convert.ToBoolean(Apple2Sharp.Properties.Settings.Default["Idealized"]);
            mainBoard.joystick = Convert.ToBoolean(Apple2Sharp.Properties.Settings.Default["Joystick"]);
            mainBoard.appleIIe = Convert.ToBoolean(Apple2Sharp.Properties.Settings.Default["AppleIIe"]);
            richTextBox2.Visible = Convert.ToBoolean(Apple2Sharp.Properties.Settings.Default["Debug"]);
            if (mainBoard.joystick)
            {
                mainBoard.timerpdl0 = 1790;
                mainBoard.timerpdl1 = 1790;
                mainBoard.timerpdl2 = 1790;
                mainBoard.timerpdl3 = 1790;
            }

            if (mainBoard.adjust1Mhz)
            {
                mainBoard.clickBuffer.Clear();
                tbSpeed.Value = 1;
                tbSpeed.Enabled = false;
            }
            else
            {
                mainBoard.clickBuffer.Clear();
                tbSpeed.Value = 10;
                tbSpeed.Enabled = true;
            }

            cbslot0.Visible = !mainBoard.appleIIe;
            lblslot0.Visible = !mainBoard.appleIIe;
        }
        private void LoadCardsCombos()
        {
            cbSlots = new List<ComboBox>() { cbslot0, cbslot1, cbslot2, cbslot3, cbslot4, cbslot5, cbslot6, cbslot7 };
            cbDatasource = new List<List<KeyValuePair<string, string>>>() {
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("LanguageCard","Language Card"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128k RAM"),
                    new KeyValuePair<string, string>("EmptySlot","Empty")
                },
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("EmptySlot","Empty"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128k RAM")
                },
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("EmptySlot","Empty"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128k RAM")
                },
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("Cols80Card","Videx 80 Column"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128,k RAM"),
                    new KeyValuePair<string, string>("EmptySlot","Empty")
                },
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("EmptySlot","Empty"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128k RAM")
                },
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("EmptySlot","Empty"),
                    new KeyValuePair<string, string>("DiskIICard","Disk II Card"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128k RAM")
                },
                new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("DiskIICard","Disk II Card"),
                    new KeyValuePair<string, string>("RamCard","Saturn 128k RAM"),
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
        public void LoadThreads()
        {
            threads = new List<Task>();
            threads.Add(Task.Run(() =>
            {
                while (formRunning)
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
                        SetDriveLight(D1ON, actualDiskCard.drive1.on);
                        SetLabel(D2T, "T: " + actualDiskCard.drive2.track.ToString());
                        SetLabel(D2S, "S: " + (actualDiskCard.drive2.sector > 16 ? "?" : actualDiskCard.drive2.sector.ToString()));
                        SetDriveLight(D2ON, actualDiskCard.drive2.on);
                    }

                    SetButtonActive(btnColor, mainBoard.videoColor, Color.SteelBlue);
                    SetButtonActive(btnTurbo, !mainBoard.adjust1Mhz, Color.SteelBlue);
                    SetButtonActive(btn1Mhz, mainBoard.adjust1Mhz, Color.SteelBlue);
                    SetButtonActive(btnScanLines, mainBoard.scanLines, Color.SteelBlue);
                    SetButtonActive(btnPaused, cpu.cpuState == CpuState.Paused, Color.SteelBlue);
                    SetButtonActive(btnIdealized, mainBoard.idealized, Color.SteelBlue);
                    SetButtonActive(btnJoystick, mainBoard.joystick, Color.SteelBlue);
                    SetButtonActive(btnAppleIIe, mainBoard.appleIIe, Color.SteelBlue);
                    SetButtonActive(btnDebug, richTextBox2.Visible, Color.SteelBlue);

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
                while (formRunning)
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
                                else if (mainBoard.appleIIe)
                                    pictureBox1.Image = Video.Generate(mainBoard, pixelSize);
                            }
                        }
                        catch (Exception ex) { 

                        }
                    }

                    Thread.Sleep(50);
                }
            }));
        }

        private void CPUThread()
        {
            Task.Run(() =>
            {
                cpu.cpuState = CpuState.Running;
                clock.Run();
            }
            );
        }
        private void StartSpeaker()
        {
            waveFormat = new WaveFormat(120000, 8, 1);
            speaker = new Speaker(mainBoard, waveFormat);
            soundOutput.Init(speaker);
            soundOutput.Play();
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
        public static int ReadTrackBar(TrackBar control)
        {
            if (control.InvokeRequired)
                return (int)control.Invoke(new Func<int>(() => ReadTrackBar(control)));
            else
                return control.Value;
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
        public void SetButtonActive(Button control, bool active, Color color)
        {
            if (D1T.InvokeRequired)
            {
                Action safeWrite = delegate { SetButtonActive(control, active, color); };
                control.Invoke(safeWrite);
            }
            else
            {
                if (active)
                {
                    control.BackColor = color;
                    control.ForeColor = Color.White;
                }
                else
                {
                    control.BackColor = Color.Black;
                    control.ForeColor = Color.White;
                }
            }
        }
        public void SetDriveLight(PictureBox control, bool check)
        {
            if (D1T.InvokeRequired)
            {
                Action safeWrite = delegate { SetDriveLight(control, check); };
                control.Invoke(safeWrite);
            }
            else
                control.Visible = check;
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
        public static bool IsFileInUseGeneric(string file)
        {
            FileInfo fi = new FileInfo(file);
            try
            {
                using var stream = fi.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                System.Windows.Forms.MessageBox.Show("Selected File in use by other application. Try another file.");
                return true;
            }
            return false;
        }

        private void Form1_Shown(object? sender, EventArgs e)
        {
            richTextBox1.Focus();
        }
        private void tbSpeed_ValueChanged(object? sender, EventArgs e)
        {
            mainBoard.clickBuffer.Clear();
            richTextBox1.Focus();
        }

        private void FormCloseEvent(object? sender, FormClosingEventArgs e)
        {
            cpu.cpuState = CpuState.Stopped;
            formRunning = false;
        }


        private void cbSlot_SelectedValueChanged(object? sender, EventArgs e)
        {
            string settings = ((ComboBox)sender).Name.Replace("cb", "") + "Card";
            Apple2Sharp.Properties.Settings.Default[settings] = ((ComboBox)sender).SelectedValue;
            Apple2Sharp.Properties.Settings.Default.Save();
            richTextBox1.Focus();

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
            mainBoard.adjust1Mhz = !mainBoard.adjust1Mhz;
            if (mainBoard.adjust1Mhz)
            {
                tbSpeed.Value = 1;
                tbSpeed.Enabled = false;
                soundOutput.Play();
            }
            else
            {
                mainBoard.adjust1Mhz = false;
                tbSpeed.Value = 10;
                tbSpeed.Enabled = true;
                soundOutput.Stop();
            }
            Apple2Sharp.Properties.Settings.Default["Adjust1mhz"] = mainBoard.adjust1Mhz.ToString();
            Apple2Sharp.Properties.Settings.Default.Save();
            richTextBox1.Focus();
        }
        private void btnTurbo_Click(object sender, EventArgs e)
        {
            btnClockAdjust_Click(sender, e);
        }
        private void disk1_TextChanged(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!IsFileInUseGeneric(openFileDialog1.FileName))
                {
                    string[] parts = openFileDialog1.FileName.Split('\\');
                    disk1.Text = parts[parts.Length - 1];
                    Apple2Sharp.Properties.Settings.Default["Disk1Path"] = openFileDialog1.FileName;
                    Apple2Sharp.Properties.Settings.Default.Save();
                    UpdateDisks();
                    richTextBox1.Focus();
                }
                else
                {
                    openFileDialog1.FileName = "";
                    disk1_TextChanged(sender, e);
                }
            }
        }
        private void disk2_TextChanged(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                if (!IsFileInUseGeneric(openFileDialog2.FileName))
                {
                    string[] parts = openFileDialog2.FileName.Split('\\');
                    disk2.Text = parts[parts.Length - 1];
                    Apple2Sharp.Properties.Settings.Default["Disk2Path"] = openFileDialog2.FileName;
                    Apple2Sharp.Properties.Settings.Default.Save();
                    UpdateDisks();
                    richTextBox1.Focus();
                }
                else
                {
                    openFileDialog2.FileName = "";
                    disk2_TextChanged(sender, e);
                }

            }
        }
        private void btnColor_Click(object sender, EventArgs e)
        {
            mainBoard.videoColor = !mainBoard.videoColor;
            Apple2Sharp.Properties.Settings.Default["Color"] = mainBoard.videoColor.ToString();
            Apple2Sharp.Properties.Settings.Default.Save();
            richTextBox1.Focus();
        }
        private void btnScanLines_Click(object sender, EventArgs e)
        {
            mainBoard.scanLines = !mainBoard.scanLines;
            Apple2Sharp.Properties.Settings.Default["ScanLines"] = mainBoard.scanLines.ToString();
            Apple2Sharp.Properties.Settings.Default.Save();
            richTextBox1.Focus();
        }

        private void btnPaused_Click(object sender, EventArgs e)
        {
            if (cpu.cpuState == CpuState.Paused)
                cpu.cpuState = CpuState.Running;
            else if (cpu.cpuState == CpuState.Running)
                cpu.cpuState = CpuState.Paused;
            richTextBox1.Focus();
        }
        private void btnIdealized_Click(object sender, EventArgs e)
        {
            mainBoard.idealized = !mainBoard.idealized;
            Apple2Sharp.Properties.Settings.Default["Idealized"] = mainBoard.idealized.ToString();
            Apple2Sharp.Properties.Settings.Default.Save();
            richTextBox1.Focus();
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {
            richTextBox2.Visible = !richTextBox2.Visible;
            Apple2Sharp.Properties.Settings.Default["Debug"] = richTextBox2.Visible.ToString();
            Apple2Sharp.Properties.Settings.Default.Save();
            richTextBox1.Focus();
        }
        private void btnJoystick_Click(object sender, EventArgs e)
        {
            mainBoard.joystick = !mainBoard.joystick;
            Apple2Sharp.Properties.Settings.Default["Joystick"] = mainBoard.joystick.ToString();
            Apple2Sharp.Properties.Settings.Default.Save();
            if (mainBoard.joystick)
            {
                mainBoard.timerpdl0 = 1790;
                mainBoard.timerpdl1 = 1790;
                mainBoard.timerpdl2 = 1790;
                mainBoard.timerpdl3 = 1790;
            }
            else
            {
                mainBoard.timerpdl0 = 0;
                mainBoard.timerpdl1 = 0;
                mainBoard.timerpdl2 = 0;
                mainBoard.timerpdl3 = 0;
            }
            richTextBox1.Focus();
        }

        private void btnAppleIIe_Click(object sender, EventArgs e)
        {
            mainBoard.appleIIe = !mainBoard.appleIIe;
            cpu.cpuState = CpuState.Stopped;
            LoadCPU();
            CPUThread();
            Apple2Sharp.Properties.Settings.Default["AppleIIe"] = mainBoard.appleIIe.ToString();
            Apple2Sharp.Properties.Settings.Default.Save();
            cbslot0.Visible = !mainBoard.appleIIe;
            lblslot0.Visible = !mainBoard.appleIIe;
            btn_restart_Click(sender, e);

        }

    }
}