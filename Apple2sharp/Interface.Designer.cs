using System.Drawing;
using System.Windows.Forms;

namespace Apple2Sharp
{



    partial class Interface
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Interface));
            pictureBox1 = new PictureBox();
            disk1 = new TextBox();
            lbldisk1 = new Label();
            D1S = new Label();
            D1T = new Label();
            openFileDialog1 = new OpenFileDialog();
            btn_restart = new Button();
            richTextBox1 = new RichTextBox();
            btn1Mhz = new Button();
            openFileDialog2 = new OpenFileDialog();
            lblClockSpeed = new Label();
            richTextBox2 = new RichTextBox();
            tbSpeed = new TrackBar();
            cbslot0 = new ComboBox();
            drivePanel1 = new Panel();
            D1ON = new PictureBox();
            D1OFF = new PictureBox();
            panel1 = new Panel();
            D2ON = new PictureBox();
            D2OFF = new PictureBox();
            label1 = new Label();
            D2S = new Label();
            disk2 = new TextBox();
            D2T = new Label();
            panel2 = new Panel();
            cbslot1 = new ComboBox();
            cbslot2 = new ComboBox();
            cbslot3 = new ComboBox();
            cbslot4 = new ComboBox();
            cbslot5 = new ComboBox();
            cbslot6 = new ComboBox();
            cbslot7 = new ComboBox();
            btnTurbo = new Button();
            btnColor = new Button();
            btnScanLines = new Button();
            btnPaused = new Button();
            btnIdealized = new Button();
            btnJoystick = new Button();
            pictureBox2 = new PictureBox();
            btnDebug = new Button();
            pictureBox3 = new PictureBox();
            lblslot1 = new Label();
            lblslot0 = new Label();
            lblslot2 = new Label();
            lblslot3 = new Label();
            lblslot4 = new Label();
            lblslot5 = new Label();
            lblslot6 = new Label();
            lblslot7 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbSpeed).BeginInit();
            drivePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)D1ON).BeginInit();
            ((System.ComponentModel.ISupportInitialize)D1OFF).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)D2ON).BeginInit();
            ((System.ComponentModel.ISupportInitialize)D2OFF).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Black;
            pictureBox1.Location = new Point(16, 16);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1250, 963);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // disk1
            // 
            disk1.BackColor = SystemColors.ActiveCaptionText;
            disk1.BorderStyle = BorderStyle.None;
            disk1.Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            disk1.ForeColor = SystemColors.ControlLightLight;
            disk1.Location = new Point(18, 98);
            disk1.Margin = new Padding(0);
            disk1.Name = "disk1";
            disk1.Size = new Size(375, 28);
            disk1.TabIndex = 1;
            disk1.TextAlign = HorizontalAlignment.Center;
            disk1.Click += disk1_TextChanged;
            // 
            // lbldisk1
            // 
            lbldisk1.AutoSize = true;
            lbldisk1.Font = new Font("Lucida Sans Unicode", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbldisk1.ForeColor = SystemColors.ControlLightLight;
            lbldisk1.Location = new Point(42, 12);
            lbldisk1.Margin = new Padding(4, 0, 4, 0);
            lbldisk1.Name = "lbldisk1";
            lbldisk1.Size = new Size(46, 48);
            lbldisk1.TabIndex = 6;
            lbldisk1.Text = "1";
            // 
            // D1S
            // 
            D1S.AutoSize = true;
            D1S.BackColor = Color.Transparent;
            D1S.Font = new Font("Lucida Sans Unicode", 9F);
            D1S.ForeColor = SystemColors.ControlLightLight;
            D1S.Location = new Point(344, 39);
            D1S.Margin = new Padding(4, 0, 4, 0);
            D1S.Name = "D1S";
            D1S.Size = new Size(40, 22);
            D1S.TabIndex = 4;
            D1S.Text = "S: ?";
            // 
            // D1T
            // 
            D1T.AutoSize = true;
            D1T.BackColor = Color.Transparent;
            D1T.Font = new Font("Lucida Sans Unicode", 9F);
            D1T.ForeColor = SystemColors.ControlLightLight;
            D1T.Location = new Point(285, 39);
            D1T.Margin = new Padding(4, 0, 4, 0);
            D1T.Name = "D1T";
            D1T.Size = new Size(41, 22);
            D1T.TabIndex = 3;
            D1T.Text = "T: ?";
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // btn_restart
            // 
            btn_restart.BackColor = Color.Red;
            btn_restart.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btn_restart.Location = new Point(1279, 16);
            btn_restart.Margin = new Padding(4);
            btn_restart.Name = "btn_restart";
            btn_restart.Size = new Size(200, 50);
            btn_restart.TabIndex = 5;
            btn_restart.Text = "Power";
            btn_restart.UseVisualStyleBackColor = false;
            btn_restart.Click += btn_restart_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(0, 0);
            richTextBox1.Margin = new Padding(0);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(0, 1);
            richTextBox1.TabIndex = 7;
            richTextBox1.Text = "";
            // 
            // btn1Mhz
            // 
            btn1Mhz.BackColor = Color.SteelBlue;
            btn1Mhz.Location = new Point(1279, 129);
            btn1Mhz.Name = "btn1Mhz";
            btn1Mhz.Size = new Size(95, 50);
            btn1Mhz.TabIndex = 13;
            btn1Mhz.Text = "1Mhz";
            btn1Mhz.UseVisualStyleBackColor = false;
            btn1Mhz.Click += btnClockAdjust_Click;
            // 
            // openFileDialog2
            // 
            openFileDialog2.FileName = "openFileDialog1";
            // 
            // lblClockSpeed
            // 
            lblClockSpeed.AutoSize = true;
            lblClockSpeed.BackColor = SystemColors.ActiveCaptionText;
            lblClockSpeed.Font = new Font("Lucida Sans Unicode", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblClockSpeed.ForeColor = SystemColors.ControlLightLight;
            lblClockSpeed.Location = new Point(278, 16);
            lblClockSpeed.Margin = new Padding(4, 0, 4, 0);
            lblClockSpeed.Name = "lblClockSpeed";
            lblClockSpeed.Size = new Size(64, 23);
            lblClockSpeed.TabIndex = 14;
            lblClockSpeed.Text = "? Mhz";
            lblClockSpeed.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // richTextBox2
            // 
            richTextBox2.BackColor = Color.Black;
            richTextBox2.Font = new Font("Consolas", 8F);
            richTextBox2.ForeColor = Color.White;
            richTextBox2.Location = new Point(844, 986);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.Size = new Size(1068, 235);
            richTextBox2.TabIndex = 15;
            richTextBox2.Text = "";
            richTextBox2.Visible = false;
            // 
            // tbSpeed
            // 
            tbSpeed.BackColor = SystemColors.ControlText;
            tbSpeed.Location = new Point(4, 0);
            tbSpeed.Margin = new Padding(4);
            tbSpeed.Name = "tbSpeed";
            tbSpeed.Size = new Size(250, 69);
            tbSpeed.TabIndex = 16;
            tbSpeed.Value = 1;
            // 
            // cbslot0
            // 
            cbslot0.BackColor = Color.FromArgb(90, 64, 70);
            cbslot0.FlatStyle = FlatStyle.Flat;
            cbslot0.Font = new Font("Lucida Sans Unicode", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cbslot0.ForeColor = SystemColors.ControlLightLight;
            cbslot0.FormattingEnabled = true;
            cbslot0.IntegralHeight = false;
            cbslot0.Location = new Point(1492, 140);
            cbslot0.Margin = new Padding(0);
            cbslot0.Name = "cbslot0";
            cbslot0.Size = new Size(386, 52);
            cbslot0.TabIndex = 20;
            // 
            // drivePanel1
            // 
            drivePanel1.BackColor = Color.Transparent;
            drivePanel1.BackgroundImage = (Image)resources.GetObject("drivePanel1.BackgroundImage");
            drivePanel1.BackgroundImageLayout = ImageLayout.Zoom;
            drivePanel1.Controls.Add(D1ON);
            drivePanel1.Controls.Add(D1OFF);
            drivePanel1.Controls.Add(lbldisk1);
            drivePanel1.Controls.Add(D1S);
            drivePanel1.Controls.Add(disk1);
            drivePanel1.Controls.Add(D1T);
            drivePanel1.Location = new Point(16, 982);
            drivePanel1.Margin = new Padding(0);
            drivePanel1.Name = "drivePanel1";
            drivePanel1.Size = new Size(412, 254);
            drivePanel1.TabIndex = 35;
            // 
            // D1ON
            // 
            D1ON.BackgroundImage = (Image)resources.GetObject("D1ON.BackgroundImage");
            D1ON.BackgroundImageLayout = ImageLayout.Zoom;
            D1ON.Location = new Point(64, 156);
            D1ON.Margin = new Padding(4);
            D1ON.Name = "D1ON";
            D1ON.Size = new Size(45, 45);
            D1ON.TabIndex = 8;
            D1ON.TabStop = false;
            // 
            // D1OFF
            // 
            D1OFF.BackgroundImage = (Image)resources.GetObject("D1OFF.BackgroundImage");
            D1OFF.BackgroundImageLayout = ImageLayout.Zoom;
            D1OFF.Location = new Point(64, 156);
            D1OFF.Margin = new Padding(4);
            D1OFF.Name = "D1OFF";
            D1OFF.Size = new Size(45, 45);
            D1OFF.TabIndex = 7;
            D1OFF.TabStop = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            panel1.BackgroundImage = (Image)resources.GetObject("panel1.BackgroundImage");
            panel1.BackgroundImageLayout = ImageLayout.Zoom;
            panel1.Controls.Add(D2ON);
            panel1.Controls.Add(D2OFF);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(D2S);
            panel1.Controls.Add(disk2);
            panel1.Controls.Add(D2T);
            panel1.Location = new Point(429, 982);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new Size(412, 254);
            panel1.TabIndex = 36;
            // 
            // D2ON
            // 
            D2ON.BackgroundImage = (Image)resources.GetObject("D2ON.BackgroundImage");
            D2ON.BackgroundImageLayout = ImageLayout.Zoom;
            D2ON.Location = new Point(64, 156);
            D2ON.Margin = new Padding(4);
            D2ON.Name = "D2ON";
            D2ON.Size = new Size(45, 45);
            D2ON.TabIndex = 8;
            D2ON.TabStop = false;
            // 
            // D2OFF
            // 
            D2OFF.BackgroundImage = (Image)resources.GetObject("D2OFF.BackgroundImage");
            D2OFF.BackgroundImageLayout = ImageLayout.Zoom;
            D2OFF.Location = new Point(64, 156);
            D2OFF.Margin = new Padding(4);
            D2OFF.Name = "D2OFF";
            D2OFF.Size = new Size(45, 45);
            D2OFF.TabIndex = 7;
            D2OFF.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Lucida Sans Unicode", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ControlLightLight;
            label1.Location = new Point(42, 12);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(46, 48);
            label1.TabIndex = 6;
            label1.Text = "2";
            // 
            // D2S
            // 
            D2S.AutoSize = true;
            D2S.BackColor = Color.Transparent;
            D2S.Font = new Font("Lucida Sans Unicode", 9F);
            D2S.ForeColor = SystemColors.ControlLightLight;
            D2S.Location = new Point(344, 39);
            D2S.Margin = new Padding(4, 0, 4, 0);
            D2S.Name = "D2S";
            D2S.Size = new Size(40, 22);
            D2S.TabIndex = 4;
            D2S.Text = "S: ?";
            // 
            // disk2
            // 
            disk2.BackColor = SystemColors.ActiveCaptionText;
            disk2.BorderStyle = BorderStyle.None;
            disk2.Font = new Font("Lucida Sans Unicode", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            disk2.ForeColor = SystemColors.ControlLightLight;
            disk2.Location = new Point(18, 98);
            disk2.Margin = new Padding(0);
            disk2.Name = "disk2";
            disk2.Size = new Size(375, 28);
            disk2.TabIndex = 1;
            disk2.TextAlign = HorizontalAlignment.Center;
            disk2.Click += disk2_TextChanged;
            // 
            // D2T
            // 
            D2T.AutoSize = true;
            D2T.BackColor = Color.Transparent;
            D2T.Font = new Font("Lucida Sans Unicode", 9F);
            D2T.ForeColor = SystemColors.ControlLightLight;
            D2T.Location = new Point(285, 39);
            D2T.Margin = new Padding(4, 0, 4, 0);
            D2T.Name = "D2T";
            D2T.Size = new Size(41, 22);
            D2T.TabIndex = 3;
            D2T.Text = "T: ?";
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.ActiveCaptionText;
            panel2.Controls.Add(tbSpeed);
            panel2.Controls.Add(lblClockSpeed);
            panel2.Location = new Point(1492, 18);
            panel2.Margin = new Padding(4);
            panel2.Name = "panel2";
            panel2.Size = new Size(387, 63);
            panel2.TabIndex = 37;
            // 
            // cbslot1
            // 
            cbslot1.BackColor = Color.FromArgb(90, 64, 70);
            cbslot1.FlatStyle = FlatStyle.Flat;
            cbslot1.Font = new Font("Lucida Sans Unicode", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cbslot1.ForeColor = SystemColors.ControlLightLight;
            cbslot1.FormattingEnabled = true;
            cbslot1.IntegralHeight = false;
            cbslot1.Location = new Point(1492, 254);
            cbslot1.Margin = new Padding(0);
            cbslot1.Name = "cbslot1";
            cbslot1.Size = new Size(386, 52);
            cbslot1.TabIndex = 38;
            // 
            // cbslot2
            // 
            cbslot2.BackColor = Color.FromArgb(90, 64, 70);
            cbslot2.FlatStyle = FlatStyle.Flat;
            cbslot2.Font = new Font("Lucida Sans Unicode", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cbslot2.ForeColor = SystemColors.ControlLightLight;
            cbslot2.FormattingEnabled = true;
            cbslot2.IntegralHeight = false;
            cbslot2.Location = new Point(1492, 363);
            cbslot2.Margin = new Padding(0);
            cbslot2.Name = "cbslot2";
            cbslot2.Size = new Size(386, 52);
            cbslot2.TabIndex = 39;
            // 
            // cbslot3
            // 
            cbslot3.BackColor = Color.FromArgb(90, 64, 70);
            cbslot3.FlatStyle = FlatStyle.Flat;
            cbslot3.Font = new Font("Lucida Sans Unicode", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cbslot3.ForeColor = SystemColors.ControlLightLight;
            cbslot3.FormattingEnabled = true;
            cbslot3.IntegralHeight = false;
            cbslot3.Location = new Point(1492, 472);
            cbslot3.Margin = new Padding(0);
            cbslot3.Name = "cbslot3";
            cbslot3.Size = new Size(386, 52);
            cbslot3.TabIndex = 40;
            // 
            // cbslot4
            // 
            cbslot4.BackColor = Color.FromArgb(90, 64, 70);
            cbslot4.FlatStyle = FlatStyle.Flat;
            cbslot4.Font = new Font("Lucida Sans Unicode", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cbslot4.ForeColor = SystemColors.ControlLightLight;
            cbslot4.FormattingEnabled = true;
            cbslot4.IntegralHeight = false;
            cbslot4.Location = new Point(1492, 582);
            cbslot4.Margin = new Padding(0);
            cbslot4.Name = "cbslot4";
            cbslot4.Size = new Size(386, 52);
            cbslot4.TabIndex = 41;
            // 
            // cbslot5
            // 
            cbslot5.BackColor = Color.FromArgb(90, 64, 70);
            cbslot5.FlatStyle = FlatStyle.Flat;
            cbslot5.Font = new Font("Lucida Sans Unicode", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cbslot5.ForeColor = SystemColors.ControlLightLight;
            cbslot5.FormattingEnabled = true;
            cbslot5.IntegralHeight = false;
            cbslot5.Location = new Point(1492, 694);
            cbslot5.Margin = new Padding(0);
            cbslot5.Name = "cbslot5";
            cbslot5.Size = new Size(386, 52);
            cbslot5.TabIndex = 42;
            // 
            // cbslot6
            // 
            cbslot6.BackColor = Color.FromArgb(90, 64, 70);
            cbslot6.FlatStyle = FlatStyle.Flat;
            cbslot6.Font = new Font("Lucida Sans Unicode", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cbslot6.ForeColor = SystemColors.ControlLightLight;
            cbslot6.FormattingEnabled = true;
            cbslot6.IntegralHeight = false;
            cbslot6.Location = new Point(1492, 804);
            cbslot6.Margin = new Padding(0);
            cbslot6.Name = "cbslot6";
            cbslot6.Size = new Size(386, 52);
            cbslot6.TabIndex = 43;
            // 
            // cbslot7
            // 
            cbslot7.BackColor = Color.FromArgb(90, 64, 70);
            cbslot7.FlatStyle = FlatStyle.Flat;
            cbslot7.Font = new Font("Lucida Sans Unicode", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cbslot7.ForeColor = SystemColors.ControlLightLight;
            cbslot7.FormattingEnabled = true;
            cbslot7.IntegralHeight = false;
            cbslot7.Location = new Point(1492, 912);
            cbslot7.Margin = new Padding(0);
            cbslot7.Name = "cbslot7";
            cbslot7.Size = new Size(386, 52);
            cbslot7.TabIndex = 44;
            // 
            // btnTurbo
            // 
            btnTurbo.BackColor = SystemColors.ControlText;
            btnTurbo.ForeColor = SystemColors.ControlLightLight;
            btnTurbo.Location = new Point(1384, 129);
            btnTurbo.Name = "btnTurbo";
            btnTurbo.Size = new Size(95, 50);
            btnTurbo.TabIndex = 45;
            btnTurbo.Text = "Turbo";
            btnTurbo.UseVisualStyleBackColor = false;
            btnTurbo.Click += btnTurbo_Click;
            // 
            // btnColor
            // 
            btnColor.BackColor = Color.SteelBlue;
            btnColor.Location = new Point(1279, 185);
            btnColor.Name = "btnColor";
            btnColor.Size = new Size(200, 50);
            btnColor.TabIndex = 46;
            btnColor.Text = "Color";
            btnColor.UseVisualStyleBackColor = false;
            btnColor.Click += btnColor_Click;
            // 
            // btnScanLines
            // 
            btnScanLines.BackColor = Color.SteelBlue;
            btnScanLines.Location = new Point(1279, 241);
            btnScanLines.Name = "btnScanLines";
            btnScanLines.Size = new Size(200, 50);
            btnScanLines.TabIndex = 47;
            btnScanLines.Text = "Scan Lines";
            btnScanLines.UseVisualStyleBackColor = false;
            btnScanLines.Click += btnScanLines_Click;
            // 
            // btnPaused
            // 
            btnPaused.BackColor = SystemColors.ControlText;
            btnPaused.ForeColor = SystemColors.ControlLightLight;
            btnPaused.Location = new Point(1279, 73);
            btnPaused.Name = "btnPaused";
            btnPaused.Size = new Size(200, 50);
            btnPaused.TabIndex = 49;
            btnPaused.Text = "Paused";
            btnPaused.UseVisualStyleBackColor = false;
            btnPaused.Click += btnPaused_Click;
            // 
            // btnIdealized
            // 
            btnIdealized.BackColor = Color.SteelBlue;
            btnIdealized.Location = new Point(1279, 297);
            btnIdealized.Name = "btnIdealized";
            btnIdealized.Size = new Size(200, 50);
            btnIdealized.TabIndex = 50;
            btnIdealized.Text = "Idealized";
            btnIdealized.UseVisualStyleBackColor = false;
            btnIdealized.Click += btnIdealized_Click;
            // 
            // btnJoystick
            // 
            btnJoystick.BackColor = SystemColors.ControlText;
            btnJoystick.ForeColor = SystemColors.ControlLightLight;
            btnJoystick.Location = new Point(1279, 353);
            btnJoystick.Name = "btnJoystick";
            btnJoystick.Size = new Size(200, 50);
            btnJoystick.TabIndex = 51;
            btnJoystick.Text = "Joystick";
            btnJoystick.UseVisualStyleBackColor = false;
            btnJoystick.Click += btnJoystick_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.Transparent;
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(788, 996);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(771, 208);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 52;
            pictureBox2.TabStop = false;
            // 
            // btnDebug
            // 
            btnDebug.BackColor = SystemColors.ControlText;
            btnDebug.ForeColor = SystemColors.ControlLightLight;
            btnDebug.Location = new Point(1279, 930);
            btnDebug.Name = "btnDebug";
            btnDebug.Size = new Size(200, 50);
            btnDebug.TabIndex = 53;
            btnDebug.Text = "Debug Window";
            btnDebug.UseVisualStyleBackColor = false;
            btnDebug.Click += btnDebug_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = Color.Transparent;
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(1496, 967);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(383, 249);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 54;
            pictureBox3.TabStop = false;
            // 
            // lblslot1
            // 
            lblslot1.AutoSize = true;
            lblslot1.BackColor = Color.Transparent;
            lblslot1.Font = new Font("Lucida Sans Unicode", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblslot1.ForeColor = SystemColors.ControlLightLight;
            lblslot1.Location = new Point(1882, 261);
            lblslot1.Margin = new Padding(4, 0, 4, 0);
            lblslot1.Name = "lblslot1";
            lblslot1.RightToLeft = RightToLeft.No;
            lblslot1.Size = new Size(38, 39);
            lblslot1.TabIndex = 9;
            lblslot1.Text = "1";
            // 
            // lblslot0
            // 
            lblslot0.AutoSize = true;
            lblslot0.BackColor = Color.Transparent;
            lblslot0.Font = new Font("Lucida Sans Unicode", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblslot0.ForeColor = SystemColors.ControlLightLight;
            lblslot0.Location = new Point(1882, 147);
            lblslot0.Margin = new Padding(4, 0, 4, 0);
            lblslot0.Name = "lblslot0";
            lblslot0.RightToLeft = RightToLeft.No;
            lblslot0.Size = new Size(38, 39);
            lblslot0.TabIndex = 55;
            lblslot0.Text = "0";
            // 
            // lblslot2
            // 
            lblslot2.AutoSize = true;
            lblslot2.BackColor = Color.Transparent;
            lblslot2.Font = new Font("Lucida Sans Unicode", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblslot2.ForeColor = SystemColors.ControlLightLight;
            lblslot2.Location = new Point(1882, 370);
            lblslot2.Margin = new Padding(4, 0, 4, 0);
            lblslot2.Name = "lblslot2";
            lblslot2.RightToLeft = RightToLeft.No;
            lblslot2.Size = new Size(38, 39);
            lblslot2.TabIndex = 56;
            lblslot2.Text = "2";
            // 
            // lblslot3
            // 
            lblslot3.AutoSize = true;
            lblslot3.BackColor = Color.Transparent;
            lblslot3.Font = new Font("Lucida Sans Unicode", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblslot3.ForeColor = SystemColors.ControlLightLight;
            lblslot3.Location = new Point(1882, 479);
            lblslot3.Margin = new Padding(4, 0, 4, 0);
            lblslot3.Name = "lblslot3";
            lblslot3.RightToLeft = RightToLeft.No;
            lblslot3.Size = new Size(38, 39);
            lblslot3.TabIndex = 57;
            lblslot3.Text = "3";
            // 
            // lblslot4
            // 
            lblslot4.AutoSize = true;
            lblslot4.BackColor = Color.Transparent;
            lblslot4.Font = new Font("Lucida Sans Unicode", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblslot4.ForeColor = SystemColors.ControlLightLight;
            lblslot4.Location = new Point(1882, 589);
            lblslot4.Margin = new Padding(4, 0, 4, 0);
            lblslot4.Name = "lblslot4";
            lblslot4.RightToLeft = RightToLeft.No;
            lblslot4.Size = new Size(38, 39);
            lblslot4.TabIndex = 58;
            lblslot4.Text = "4";
            // 
            // lblslot5
            // 
            lblslot5.AutoSize = true;
            lblslot5.BackColor = Color.Transparent;
            lblslot5.Font = new Font("Lucida Sans Unicode", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblslot5.ForeColor = SystemColors.ControlLightLight;
            lblslot5.Location = new Point(1882, 701);
            lblslot5.Margin = new Padding(4, 0, 4, 0);
            lblslot5.Name = "lblslot5";
            lblslot5.RightToLeft = RightToLeft.No;
            lblslot5.Size = new Size(38, 39);
            lblslot5.TabIndex = 59;
            lblslot5.Text = "5";
            // 
            // lblslot6
            // 
            lblslot6.AutoSize = true;
            lblslot6.BackColor = Color.Transparent;
            lblslot6.Font = new Font("Lucida Sans Unicode", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblslot6.ForeColor = SystemColors.ControlLightLight;
            lblslot6.Location = new Point(1882, 811);
            lblslot6.Margin = new Padding(4, 0, 4, 0);
            lblslot6.Name = "lblslot6";
            lblslot6.RightToLeft = RightToLeft.No;
            lblslot6.Size = new Size(38, 39);
            lblslot6.TabIndex = 60;
            lblslot6.Text = "6";
            // 
            // lblslot7
            // 
            lblslot7.AutoSize = true;
            lblslot7.BackColor = Color.Transparent;
            lblslot7.Font = new Font("Lucida Sans Unicode", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblslot7.ForeColor = SystemColors.ControlLightLight;
            lblslot7.Location = new Point(1882, 919);
            lblslot7.Margin = new Padding(4, 0, 4, 0);
            lblslot7.Name = "lblslot7";
            lblslot7.RightToLeft = RightToLeft.No;
            lblslot7.Size = new Size(38, 39);
            lblslot7.TabIndex = 61;
            lblslot7.Text = "7";
            // 
            // Interface
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1924, 1228);
            Controls.Add(lblslot7);
            Controls.Add(lblslot6);
            Controls.Add(lblslot5);
            Controls.Add(lblslot4);
            Controls.Add(lblslot3);
            Controls.Add(lblslot2);
            Controls.Add(lblslot0);
            Controls.Add(lblslot1);
            Controls.Add(btnDebug);
            Controls.Add(richTextBox2);
            Controls.Add(panel1);
            Controls.Add(pictureBox1);
            Controls.Add(btnJoystick);
            Controls.Add(btnIdealized);
            Controls.Add(btnPaused);
            Controls.Add(btnScanLines);
            Controls.Add(btnColor);
            Controls.Add(btnTurbo);
            Controls.Add(cbslot7);
            Controls.Add(cbslot6);
            Controls.Add(cbslot5);
            Controls.Add(cbslot4);
            Controls.Add(cbslot3);
            Controls.Add(cbslot2);
            Controls.Add(cbslot1);
            Controls.Add(panel2);
            Controls.Add(drivePanel1);
            Controls.Add(btn1Mhz);
            Controls.Add(richTextBox1);
            Controls.Add(btn_restart);
            Controls.Add(cbslot0);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Interface";
            Text = "Apple II+";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbSpeed).EndInit();
            drivePanel1.ResumeLayout(false);
            drivePanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)D1ON).EndInit();
            ((System.ComponentModel.ISupportInitialize)D1OFF).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)D2ON).EndInit();
            ((System.ComponentModel.ISupportInitialize)D2OFF).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private TextBox disk1;
        private OpenFileDialog openFileDialog1;
        private Button btn_restart;
        private RichTextBox richTextBox1;
        private Button btn1Mhz;
        private OpenFileDialog openFileDialog2;
        private Label lblClockSpeed;
        private RichTextBox richTextBox2;
        private Label D1S;
        private Label D1T;
        private TrackBar tbSpeed;
        private ComboBox cbslot0;
        private Label lbldisk1;
        private Panel drivePanel1;
        private PictureBox D1ON;
        private PictureBox D1OFF;
        private Panel panel1;
        private PictureBox D2ON;
        private PictureBox D2OFF;
        private Label label1;
        private Label D2S;
        private TextBox disk2;
        private Label D2T;
        private Panel panel2;
        private ComboBox cbslot1;
        private ComboBox cbslot2;
        private ComboBox cbslot3;
        private ComboBox cbslot4;
        private ComboBox cbslot5;
        private ComboBox cbslot6;
        private ComboBox cbslot7;
        private Button btnTurbo;
        private Button btnColor;
        private Button btnScanLines;
        private Button btnPaused;
        private Button btnIdealized;
        private Button btnJoystick;
        private PictureBox pictureBox2;
        private Button btnDebug;
        private PictureBox pictureBox3;
        private Label label2;
        private Label lblslot1;
        private Label lblslot0;
        private Label lblslot2;
        private Label lblslot3;
        private Label lblslot4;
        private Label lblslot5;
        private Label lblslot6;
        private Label lblslot7;
    }
}