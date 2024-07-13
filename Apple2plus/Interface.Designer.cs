using System.Drawing;
using System.Windows.Forms;

namespace Apple2
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
            groupBox1 = new GroupBox();
            D1O = new CheckBox();
            D1S = new Label();
            D1T = new Label();
            button_dsk1 = new Button();
            openFileDialog1 = new OpenFileDialog();
            btn_restart = new Button();
            groupBox2 = new GroupBox();
            D2O = new CheckBox();
            D2S = new Label();
            D2T = new Label();
            button_dsk2 = new Button();
            disk2 = new TextBox();
            richTextBox1 = new RichTextBox();
            btnClockAdjust = new Button();
            openFileDialog2 = new OpenFileDialog();
            lblClockSpeed = new Label();
            richTextBox2 = new RichTextBox();
            tbSpeed = new TrackBar();
            ckbColor = new CheckBox();
            lbl0 = new Label();
            groupBox3 = new GroupBox();
            cbSlot7 = new ComboBox();
            label7 = new Label();
            cbSlot6 = new ComboBox();
            label6 = new Label();
            cbSlot5 = new ComboBox();
            label5 = new Label();
            cbSlot4 = new ComboBox();
            label4 = new Label();
            cbSlot3 = new ComboBox();
            label3 = new Label();
            cbSlot2 = new ComboBox();
            label2 = new Label();
            cbSlot1 = new ComboBox();
            label1 = new Label();
            cbSlot0 = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tbSpeed).BeginInit();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Black;
            pictureBox1.Location = new Point(11, 0);
            pictureBox1.Margin = new Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(880, 555);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // disk1
            // 
            disk1.Location = new Point(6, 22);
            disk1.Name = "disk1";
            disk1.Size = new Size(520, 23);
            disk1.TabIndex = 1;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(D1O);
            groupBox1.Controls.Add(D1S);
            groupBox1.Controls.Add(D1T);
            groupBox1.Controls.Add(button_dsk1);
            groupBox1.Controls.Add(disk1);
            groupBox1.Location = new Point(11, 589);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(706, 60);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "DISK 1";
            // 
            // D1O
            // 
            D1O.AutoSize = true;
            D1O.Location = new Point(615, 18);
            D1O.Name = "D1O";
            D1O.Size = new Size(15, 14);
            D1O.TabIndex = 5;
            D1O.UseVisualStyleBackColor = true;
            // 
            // D1S
            // 
            D1S.AutoSize = true;
            D1S.Location = new Point(647, 38);
            D1S.Name = "D1S";
            D1S.Size = new Size(24, 15);
            D1S.TabIndex = 4;
            D1S.Text = "S: ?";
            // 
            // D1T
            // 
            D1T.AutoSize = true;
            D1T.Location = new Point(613, 38);
            D1T.Name = "D1T";
            D1T.Size = new Size(24, 15);
            D1T.TabIndex = 3;
            D1T.Text = "T: ?";
            // 
            // button_dsk1
            // 
            button_dsk1.Location = new Point(531, 20);
            button_dsk1.Name = "button_dsk1";
            button_dsk1.Size = new Size(75, 23);
            button_dsk1.TabIndex = 2;
            button_dsk1.Text = "Select...";
            button_dsk1.UseVisualStyleBackColor = true;
            button_dsk1.Click += button_dsk1_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // btn_restart
            // 
            btn_restart.Location = new Point(11, 561);
            btn_restart.Name = "btn_restart";
            btn_restart.Size = new Size(75, 23);
            btn_restart.TabIndex = 5;
            btn_restart.Text = "Power";
            btn_restart.UseVisualStyleBackColor = true;
            btn_restart.Click += btn_restart_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(D2O);
            groupBox2.Controls.Add(D2S);
            groupBox2.Controls.Add(D2T);
            groupBox2.Controls.Add(button_dsk2);
            groupBox2.Controls.Add(disk2);
            groupBox2.Location = new Point(11, 651);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(706, 60);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "DISK 2";
            // 
            // D2O
            // 
            D2O.AutoSize = true;
            D2O.Location = new Point(616, 17);
            D2O.Name = "D2O";
            D2O.Size = new Size(15, 14);
            D2O.TabIndex = 8;
            D2O.UseVisualStyleBackColor = true;
            // 
            // D2S
            // 
            D2S.AutoSize = true;
            D2S.Location = new Point(648, 37);
            D2S.Name = "D2S";
            D2S.Size = new Size(24, 15);
            D2S.TabIndex = 7;
            D2S.Text = "S: ?";
            // 
            // D2T
            // 
            D2T.AutoSize = true;
            D2T.Location = new Point(614, 37);
            D2T.Name = "D2T";
            D2T.Size = new Size(24, 15);
            D2T.TabIndex = 6;
            D2T.Text = "T: ?";
            // 
            // button_dsk2
            // 
            button_dsk2.Location = new Point(531, 18);
            button_dsk2.Name = "button_dsk2";
            button_dsk2.Size = new Size(75, 23);
            button_dsk2.TabIndex = 2;
            button_dsk2.Text = "Select...";
            button_dsk2.UseVisualStyleBackColor = true;
            button_dsk2.Click += button_dsk2_Click;
            // 
            // disk2
            // 
            disk2.Location = new Point(6, 22);
            disk2.Name = "disk2";
            disk2.Size = new Size(520, 23);
            disk2.TabIndex = 1;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(0, 0);
            richTextBox1.Margin = new Padding(0);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(1, 2);
            richTextBox1.TabIndex = 7;
            richTextBox1.Text = "";
            // 
            // btnClockAdjust
            // 
            btnClockAdjust.Location = new Point(91, 561);
            btnClockAdjust.Margin = new Padding(2);
            btnClockAdjust.Name = "btnClockAdjust";
            btnClockAdjust.Size = new Size(78, 23);
            btnClockAdjust.TabIndex = 13;
            btnClockAdjust.Text = "1Mhz";
            btnClockAdjust.UseVisualStyleBackColor = true;
            btnClockAdjust.Click += btnClockAdjust_Click;
            // 
            // openFileDialog2
            // 
            openFileDialog2.FileName = "openFileDialog1";
            // 
            // lblClockSpeed
            // 
            lblClockSpeed.AutoSize = true;
            lblClockSpeed.Location = new Point(403, 564);
            lblClockSpeed.Name = "lblClockSpeed";
            lblClockSpeed.Size = new Size(38, 15);
            lblClockSpeed.TabIndex = 14;
            lblClockSpeed.Text = "? Mhz";
            // 
            // richTextBox2
            // 
            richTextBox2.BackColor = Color.Black;
            richTextBox2.Font = new Font("Consolas", 8F);
            richTextBox2.ForeColor = Color.White;
            richTextBox2.Location = new Point(899, 0);
            richTextBox2.Margin = new Padding(2);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.Size = new Size(429, 287);
            richTextBox2.TabIndex = 15;
            richTextBox2.Text = "";
            // 
            // tbSpeed
            // 
            tbSpeed.Location = new Point(174, 560);
            tbSpeed.Name = "tbSpeed";
            tbSpeed.Size = new Size(223, 45);
            tbSpeed.TabIndex = 16;
            tbSpeed.Value = 1;
            // 
            // ckbColor
            // 
            ckbColor.AutoSize = true;
            ckbColor.Checked = true;
            ckbColor.CheckState = CheckState.Checked;
            ckbColor.Location = new Point(478, 564);
            ckbColor.Name = "ckbColor";
            ckbColor.Size = new Size(55, 19);
            ckbColor.TabIndex = 18;
            ckbColor.Text = "Color";
            ckbColor.TextAlign = ContentAlignment.MiddleCenter;
            ckbColor.UseVisualStyleBackColor = true;
            ckbColor.Click += ckbColor_Click;
            // 
            // lbl0
            // 
            lbl0.AutoSize = true;
            lbl0.Location = new Point(8, 27);
            lbl0.Name = "lbl0";
            lbl0.Size = new Size(36, 15);
            lbl0.TabIndex = 19;
            lbl0.Text = "Slot 0";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(cbSlot7);
            groupBox3.Controls.Add(label7);
            groupBox3.Controls.Add(cbSlot6);
            groupBox3.Controls.Add(label6);
            groupBox3.Controls.Add(cbSlot5);
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(cbSlot4);
            groupBox3.Controls.Add(label4);
            groupBox3.Controls.Add(cbSlot3);
            groupBox3.Controls.Add(label3);
            groupBox3.Controls.Add(cbSlot2);
            groupBox3.Controls.Add(label2);
            groupBox3.Controls.Add(cbSlot1);
            groupBox3.Controls.Add(label1);
            groupBox3.Controls.Add(cbSlot0);
            groupBox3.Controls.Add(lbl0);
            groupBox3.Location = new Point(902, 289);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(309, 264);
            groupBox3.TabIndex = 20;
            groupBox3.TabStop = false;
            groupBox3.Text = "Slots";
            // 
            // cbSlot7
            // 
            cbSlot7.FormattingEnabled = true;
            cbSlot7.Location = new Point(50, 228);
            cbSlot7.Name = "cbSlot7";
            cbSlot7.Size = new Size(240, 23);
            cbSlot7.TabIndex = 34;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(8, 231);
            label7.Name = "label7";
            label7.Size = new Size(36, 15);
            label7.TabIndex = 33;
            label7.Text = "Slot 7";
            // 
            // cbSlot6
            // 
            cbSlot6.FormattingEnabled = true;
            cbSlot6.Location = new Point(50, 198);
            cbSlot6.Name = "cbSlot6";
            cbSlot6.Size = new Size(240, 23);
            cbSlot6.TabIndex = 32;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(8, 201);
            label6.Name = "label6";
            label6.Size = new Size(36, 15);
            label6.TabIndex = 31;
            label6.Text = "Slot 6";
            // 
            // cbSlot5
            // 
            cbSlot5.FormattingEnabled = true;
            cbSlot5.Location = new Point(50, 169);
            cbSlot5.Name = "cbSlot5";
            cbSlot5.Size = new Size(240, 23);
            cbSlot5.TabIndex = 30;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(8, 172);
            label5.Name = "label5";
            label5.Size = new Size(36, 15);
            label5.TabIndex = 29;
            label5.Text = "Slot 5";
            // 
            // cbSlot4
            // 
            cbSlot4.FormattingEnabled = true;
            cbSlot4.Location = new Point(50, 140);
            cbSlot4.Name = "cbSlot4";
            cbSlot4.Size = new Size(240, 23);
            cbSlot4.TabIndex = 28;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(8, 143);
            label4.Name = "label4";
            label4.Size = new Size(36, 15);
            label4.TabIndex = 27;
            label4.Text = "Slot 4";
            // 
            // cbSlot3
            // 
            cbSlot3.FormattingEnabled = true;
            cbSlot3.Location = new Point(50, 111);
            cbSlot3.Name = "cbSlot3";
            cbSlot3.Size = new Size(240, 23);
            cbSlot3.TabIndex = 26;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 114);
            label3.Name = "label3";
            label3.Size = new Size(36, 15);
            label3.TabIndex = 25;
            label3.Text = "Slot 3";
            // 
            // cbSlot2
            // 
            cbSlot2.FormattingEnabled = true;
            cbSlot2.Location = new Point(50, 82);
            cbSlot2.Name = "cbSlot2";
            cbSlot2.Size = new Size(240, 23);
            cbSlot2.TabIndex = 24;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 85);
            label2.Name = "label2";
            label2.Size = new Size(36, 15);
            label2.TabIndex = 23;
            label2.Text = "Slot 2";
            // 
            // cbSlot1
            // 
            cbSlot1.FormattingEnabled = true;
            cbSlot1.Location = new Point(50, 53);
            cbSlot1.Name = "cbSlot1";
            cbSlot1.Size = new Size(240, 23);
            cbSlot1.TabIndex = 22;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 56);
            label1.Name = "label1";
            label1.Size = new Size(36, 15);
            label1.TabIndex = 21;
            label1.Text = "Slot 1";
            // 
            // cbSlot0
            // 
            cbSlot0.FormattingEnabled = true;
            cbSlot0.Location = new Point(50, 24);
            cbSlot0.Name = "cbSlot0";
            cbSlot0.Size = new Size(240, 23);
            cbSlot0.TabIndex = 20;
            // 
            // Interface
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1332, 721);
            Controls.Add(groupBox3);
            Controls.Add(ckbColor);
            Controls.Add(groupBox1);
            Controls.Add(tbSpeed);
            Controls.Add(richTextBox2);
            Controls.Add(lblClockSpeed);
            Controls.Add(btnClockAdjust);
            Controls.Add(richTextBox1);
            Controls.Add(groupBox2);
            Controls.Add(btn_restart);
            Controls.Add(pictureBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            Name = "Interface";
            Text = "Apple II+";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tbSpeed).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private TextBox disk1;
        private GroupBox groupBox1;
        private Button button_dsk1;
        private OpenFileDialog openFileDialog1;
        private Button btn_restart;
        private GroupBox groupBox2;
        private Button button_dsk2;
        private TextBox disk2;
        private RichTextBox richTextBox1;
        private Button btnClockAdjust;
        private OpenFileDialog openFileDialog2;
        private Label lblClockSpeed;
        private RichTextBox richTextBox2;
        private CheckBox D1O;
        private Label D1S;
        private Label D1T;
        private CheckBox D2O;
        private Label D2S;
        private Label D2T;
        private TrackBar tbSpeed;
        private CheckBox ckbColor;
        private Label lbl0;
        private GroupBox groupBox3;
        private ComboBox cbSlot7;
        private Label label7;
        private ComboBox cbSlot6;
        private Label label6;
        private ComboBox cbSlot5;
        private Label label5;
        private ComboBox cbSlot4;
        private Label label4;
        private ComboBox cbSlot3;
        private Label label3;
        private ComboBox cbSlot2;
        private Label label2;
        private ComboBox cbSlot1;
        private Label label1;
        private ComboBox cbSlot0;
    }
}