namespace Apple2;

partial class Form1
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
        components = new System.ComponentModel.Container();
        pictureBox1 = new PictureBox();
        disk1 = new TextBox();
        groupBox1 = new GroupBox();
        button_dsk1 = new Button();
        openFileDialog1 = new OpenFileDialog();
        btn_restart = new Button();
        groupBox2 = new GroupBox();
        button_dsk2 = new Button();
        disk2 = new TextBox();
        richTextBox1 = new RichTextBox();
        btnClockAdjust = new Button();
        openFileDialog2 = new OpenFileDialog();
        lblClockSpeed = new Label();
        timerClockSpeed = new System.Windows.Forms.Timer(components);
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        groupBox1.SuspendLayout();
        groupBox2.SuspendLayout();
        SuspendLayout();
        // 
        // pictureBox1
        // 
        pictureBox1.BackColor = Color.Black;
        pictureBox1.Location = new Point(11, 0);
        pictureBox1.Margin = new Padding(2, 2, 2, 2);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(669, 474);
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
        groupBox1.Controls.Add(button_dsk1);
        groupBox1.Controls.Add(disk1);
        groupBox1.Location = new Point(11, 513);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(669, 60);
        groupBox1.TabIndex = 2;
        groupBox1.TabStop = false;
        groupBox1.Text = "DISK 1";
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
        btn_restart.Location = new Point(11, 484);
        btn_restart.Name = "btn_restart";
        btn_restart.Size = new Size(75, 23);
        btn_restart.TabIndex = 5;
        btn_restart.Text = "Power";
        btn_restart.UseVisualStyleBackColor = true;
        btn_restart.Click += btn_restart_Click;
        // 
        // groupBox2
        // 
        groupBox2.Controls.Add(button_dsk2);
        groupBox2.Controls.Add(disk2);
        groupBox2.Location = new Point(11, 575);
        groupBox2.Name = "groupBox2";
        groupBox2.Size = new Size(669, 60);
        groupBox2.TabIndex = 6;
        groupBox2.TabStop = false;
        groupBox2.Text = "DISK 2";
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
        btnClockAdjust.Location = new Point(91, 485);
        btnClockAdjust.Margin = new Padding(2, 2, 2, 2);
        btnClockAdjust.Name = "btnClockAdjust";
        btnClockAdjust.Size = new Size(78, 20);
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
        lblClockSpeed.Location = new Point(180, 487);
        lblClockSpeed.Name = "lblClockSpeed";
        lblClockSpeed.Size = new Size(38, 15);
        lblClockSpeed.TabIndex = 14;
        lblClockSpeed.Text = "label1";
        // 
        // timerClockSpeed
        // 
        timerClockSpeed.Tick += timerClockSpeed_Tick;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(690, 637);
        Controls.Add(lblClockSpeed);
        Controls.Add(btnClockAdjust);
        Controls.Add(richTextBox1);
        Controls.Add(groupBox2);
        Controls.Add(btn_restart);
        Controls.Add(groupBox1);
        Controls.Add(pictureBox1);
        Margin = new Padding(2, 2, 2, 2);
        Name = "Form1";
        Text = "Apple II+";
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        groupBox2.ResumeLayout(false);
        groupBox2.PerformLayout();
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
    private System.Windows.Forms.Timer timerClockSpeed;
}
