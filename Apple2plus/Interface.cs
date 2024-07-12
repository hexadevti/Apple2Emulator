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
using Apple2.Mainboard.Abstractions;
using Apple2.Mainboard.Cards;

namespace Apple2
{
    public partial class Interface : Form
    {
        public Apple2Board mainBoard { get; set; }
        public Processor cpu { get; set; }
        State state = new State();

        string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        List<Task> threads = new List<Task>();
        int pixelSize = 2;
        private DirectSoundOut soundOutput = new DirectSoundOut();

        public Speaker speaker;

        public System.Windows.Forms.MethodInvoker inv;

        public WaveFormat waveFormat;

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
            cpu = new Processor(state, mainBoard);
            Keyboard keyboard = new Keyboard(mainBoard, cpu);
            richTextBox1.KeyDown += keyboard.OnKeyDown;
            richTextBox1.KeyPress += keyboard.OnKeyPress;
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
            disk1.TextChanged+= disk_TextChanged;

            StartSpeaker();
            InitSlots();
            cpu.WarmStart();
            LoadThreads();
            

        }

        private void disk_TextChanged(object? sender, EventArgs e)
        {
            
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

            cbSlot0.SelectedValue = Apple2plus.Properties.Settings.Default.Slot0Card;
            cbSlot1.SelectedValue = Apple2plus.Properties.Settings.Default.Slot1Card;
            cbSlot2.SelectedValue = Apple2plus.Properties.Settings.Default.Slot2Card;
            cbSlot3.SelectedValue = Apple2plus.Properties.Settings.Default.Slot3Card;
            cbSlot4.SelectedValue = Apple2plus.Properties.Settings.Default.Slot4Card;
            cbSlot5.SelectedValue = Apple2plus.Properties.Settings.Default.Slot5Card;
            cbSlot6.SelectedValue = Apple2plus.Properties.Settings.Default.Slot6Card;
            cbSlot7.SelectedValue = Apple2plus.Properties.Settings.Default.Slot7Card;


            
            
        }
        private void LoadCardsCombos()
        {
            List<KeyValuePair<string,string>> cards0 = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("LanguageCard","Language Card"),
                new KeyValuePair<string, string>("RamCard","Saturn 128k RAM Card"),
                new KeyValuePair<string, string>("EmptySlot","Empty")
            };
            List<KeyValuePair<string,string>> cards1 = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("EmptySlot","Empty"),
                new KeyValuePair<string, string>("RamCard","Saturn 128k RAM Card")
            };
            List<KeyValuePair<string,string>> cards2 = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("EmptySlot","Empty"),
                new KeyValuePair<string, string>("RamCard","Saturn 128k RAM Card")
            };
            List<KeyValuePair<string,string>> cards3 = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("Cols80Card","Videx 80 Column Card"),
                new KeyValuePair<string, string>("RamCard","Saturn 128,k RAM Card"),
                new KeyValuePair<string, string>("EmptySlot","Empty")
            };
            List<KeyValuePair<string,string>> cards4 = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("EmptySlot","Empty"),
                new KeyValuePair<string, string>("RamCard","Saturn 128k RAM Card")
            };
            List<KeyValuePair<string,string>> cards5 = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("EmptySlot","Empty"),
                new KeyValuePair<string, string>("DiskIICard","Disk II Card"),
                new KeyValuePair<string, string>("RamCard","Saturn 128k RAM Card")
            };
            List<KeyValuePair<string,string>> cards6 = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("DiskIICard","Disk II Card"),
                new KeyValuePair<string, string>("RamCard","Saturn 128k RAM Card"),
                new KeyValuePair<string, string>("EmptySlot","Empty")
            };
            List<KeyValuePair<string,string>> cards7 = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("EmptySlot","Empty"),
                new KeyValuePair<string, string>("RamCard","Saturn 128k RAM Card")
            };
            cbSlot0.DataSource = cards0;
            cbSlot0.DisplayMember = "Value";
            cbSlot0.ValueMember = "Key";
            cbSlot0.SelectedValueChanged += cbSlot_SelectedValueChanged;
            
            cbSlot1.DataSource = cards1;
            cbSlot1.DisplayMember = "Value";
            cbSlot1.ValueMember = "Key";
            cbSlot1.SelectedValueChanged += cbSlot_SelectedValueChanged;
            
            cbSlot2.DataSource = cards2;
            cbSlot2.DisplayMember = "Value";
            cbSlot2.ValueMember = "Key";
            cbSlot2.SelectedValueChanged += cbSlot_SelectedValueChanged;

            cbSlot3.DataSource = cards3;
            cbSlot3.DisplayMember = "Value";
            cbSlot3.ValueMember = "Key";
            cbSlot3.SelectedValueChanged += cbSlot_SelectedValueChanged;

            cbSlot4.DataSource = cards4;
            cbSlot4.DisplayMember = "Value";
            cbSlot4.ValueMember = "Key";
            cbSlot4.SelectedValueChanged += cbSlot_SelectedValueChanged;

            cbSlot5.DataSource = cards5;
            cbSlot5.DisplayMember = "Value";
            cbSlot5.ValueMember = "Key";
            cbSlot5.SelectedValueChanged += cbSlot_SelectedValueChanged;

            cbSlot6.DataSource = cards6;
            cbSlot6.DisplayMember = "Value";
            cbSlot6.ValueMember = "Key";
            cbSlot6.SelectedValueChanged += cbSlot_SelectedValueChanged;

            cbSlot7.DataSource = cards7;
            cbSlot7.DisplayMember = "Value";
            cbSlot7.ValueMember = "Key";
            cbSlot7.SelectedValueChanged += cbSlot_SelectedValueChanged;

        }

        private void cbSlot_SelectedValueChanged(object? sender, EventArgs e)
        {
            string settings = ((ComboBox)sender).Name.Replace("cb", "") + "Card";
            Apple2plus.Properties.Settings.Default[settings] = ((ComboBox)sender).SelectedValue;
            Apple2plus.Properties.Settings.Default.Save();

        }

        private void InitSlots()
        {
            mainBoard.slot0 = (ICard)GetInstance(cbSlot0.SelectedValue.ToString(), 0);
            mainBoard.slot1 = (ICard)GetInstance(cbSlot1.SelectedValue.ToString(), 1);
            mainBoard.slot2 = (ICard)GetInstance(cbSlot2.SelectedValue.ToString(), 2);
            mainBoard.slot3 = (ICard)GetInstance(cbSlot3.SelectedValue.ToString(), 3);
            mainBoard.slot4 = (ICard)GetInstance(cbSlot4.SelectedValue.ToString(), 4);
            mainBoard.slot5 = (ICard)GetInstance(cbSlot5.SelectedValue.ToString(), 5);
            mainBoard.slot6 = (ICard)GetInstance(cbSlot6.SelectedValue.ToString(), 6);
            mainBoard.slot7 = (ICard)GetInstance(cbSlot7.SelectedValue.ToString(), 7);
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
                    if (mainBoard.slot5.GetType() == typeof(DiskIICard))
                    {
                        actualDiskCard = (DiskIICard)mainBoard.slot5;
                    } 
                    else if (mainBoard.slot6.GetType() == typeof(DiskIICard))
                    {
                        actualDiskCard = (DiskIICard)mainBoard.slot6;
                    }

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
                                if (mainBoard.slot3.GetType() == typeof(Cols80Card))
                                    pictureBox1.Image = ((Cols80Card)mainBoard.slot3).Generate(mainBoard, pixelSize);
                            }
                        }
                        catch { }
                    }
                    Thread.Sleep(50);
                }
            }));

            threads.Add(Task.Run(() => cpu.DelayedRun()));
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
            if (mainBoard.slot5.GetType() == typeof(DiskIICard))
            {
                actualDiskCard = (DiskIICard)mainBoard.slot5;
            } 
            else if (mainBoard.slot6.GetType() == typeof(DiskIICard))
            {
                actualDiskCard = (DiskIICard)mainBoard.slot6;
            }

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
            {
                control.Text = text;
            }

        }

        public static int ReadTrackBar(TrackBar control)
        {
            if (control.InvokeRequired)
            {
                return (int)control.Invoke(new Func<int>(() => ReadTrackBar(control)));
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