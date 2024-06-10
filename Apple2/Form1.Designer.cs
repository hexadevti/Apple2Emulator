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
        pictureBox1 = new PictureBox();
        disk1 = new TextBox();
        groupBox1 = new GroupBox();
        disk1prodos = new RadioButton();
        disk1dos = new RadioButton();
        button_dsk1 = new Button();
        openFileDialog1 = new OpenFileDialog();
        btn_restart = new Button();
        groupBox2 = new GroupBox();
        disk2prodos = new RadioButton();
        button_dsk2 = new Button();
        disk2dos = new RadioButton();
        disk2 = new TextBox();
        richTextBox1 = new RichTextBox();
        waveViewer1 = new NAudio.Gui.WaveViewer();
        button1 = new Button();
        button2 = new Button();
        button3 = new Button();
        button4 = new Button();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        groupBox1.SuspendLayout();
        groupBox2.SuspendLayout();
        SuspendLayout();
        // 
        // pictureBox1
        // 
        pictureBox1.BackColor = Color.Black;
        pictureBox1.Location = new Point(16, 0);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(956, 790);
        pictureBox1.TabIndex = 0;
        pictureBox1.TabStop = false;
        pictureBox1.Click += pictureBox1_Click;
        // 
        // disk1
        // 
        disk1.Location = new Point(9, 37);
        disk1.Margin = new Padding(4, 5, 4, 5);
        disk1.Name = "disk1";
        disk1.Size = new Size(741, 31);
        disk1.TabIndex = 1;
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(disk1prodos);
        groupBox1.Controls.Add(disk1dos);
        groupBox1.Controls.Add(button_dsk1);
        groupBox1.Controls.Add(disk1);
        groupBox1.Location = new Point(16, 855);
        groupBox1.Margin = new Padding(4, 5, 4, 5);
        groupBox1.Name = "groupBox1";
        groupBox1.Padding = new Padding(4, 5, 4, 5);
        groupBox1.Size = new Size(1025, 100);
        groupBox1.TabIndex = 2;
        groupBox1.TabStop = false;
        groupBox1.Text = "DISK 1";
        // 
        // disk1prodos
        // 
        disk1prodos.AutoSize = true;
        disk1prodos.Location = new Point(877, 60);
        disk1prodos.Name = "disk1prodos";
        disk1prodos.Size = new Size(109, 29);
        disk1prodos.TabIndex = 4;
        disk1prodos.TabStop = true;
        disk1prodos.Text = "PRODOS";
        disk1prodos.UseVisualStyleBackColor = true;
        // 
        // disk1dos
        // 
        disk1dos.AutoSize = true;
        disk1dos.Checked = true;
        disk1dos.Location = new Point(877, 25);
        disk1dos.Name = "disk1dos";
        disk1dos.Size = new Size(74, 29);
        disk1dos.TabIndex = 3;
        disk1dos.TabStop = true;
        disk1dos.Text = "DOS";
        disk1dos.UseVisualStyleBackColor = true;
        // 
        // button_dsk1
        // 
        button_dsk1.Location = new Point(758, 34);
        button_dsk1.Margin = new Padding(4, 5, 4, 5);
        button_dsk1.Name = "button_dsk1";
        button_dsk1.Size = new Size(107, 38);
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
        btn_restart.Location = new Point(16, 807);
        btn_restart.Margin = new Padding(4, 5, 4, 5);
        btn_restart.Name = "btn_restart";
        btn_restart.Size = new Size(107, 38);
        btn_restart.TabIndex = 5;
        btn_restart.Text = "Power";
        btn_restart.UseVisualStyleBackColor = true;
        btn_restart.Click += btn_restart_Click;
        // 
        // groupBox2
        // 
        groupBox2.Controls.Add(disk2prodos);
        groupBox2.Controls.Add(button_dsk2);
        groupBox2.Controls.Add(disk2dos);
        groupBox2.Controls.Add(disk2);
        groupBox2.Location = new Point(16, 958);
        groupBox2.Margin = new Padding(4, 5, 4, 5);
        groupBox2.Name = "groupBox2";
        groupBox2.Padding = new Padding(4, 5, 4, 5);
        groupBox2.Size = new Size(1025, 100);
        groupBox2.TabIndex = 6;
        groupBox2.TabStop = false;
        groupBox2.Text = "DISK 2";
        // 
        // disk2prodos
        // 
        disk2prodos.AutoSize = true;
        disk2prodos.Location = new Point(877, 54);
        disk2prodos.Name = "disk2prodos";
        disk2prodos.Size = new Size(109, 29);
        disk2prodos.TabIndex = 6;
        disk2prodos.TabStop = true;
        disk2prodos.Text = "PRODOS";
        disk2prodos.UseVisualStyleBackColor = true;
        // 
        // button_dsk2
        // 
        button_dsk2.Location = new Point(758, 30);
        button_dsk2.Margin = new Padding(4, 5, 4, 5);
        button_dsk2.Name = "button_dsk2";
        button_dsk2.Size = new Size(107, 38);
        button_dsk2.TabIndex = 2;
        button_dsk2.Text = "Select...";
        button_dsk2.UseVisualStyleBackColor = true;
        button_dsk2.Click += button_dsk2_Click;
        // 
        // disk2dos
        // 
        disk2dos.AutoSize = true;
        disk2dos.Checked = true;
        disk2dos.Location = new Point(877, 19);
        disk2dos.Name = "disk2dos";
        disk2dos.Size = new Size(74, 29);
        disk2dos.TabIndex = 5;
        disk2dos.TabStop = true;
        disk2dos.Text = "DOS";
        disk2dos.UseVisualStyleBackColor = true;
        // 
        // disk2
        // 
        disk2.Location = new Point(9, 37);
        disk2.Margin = new Padding(4, 5, 4, 5);
        disk2.Name = "disk2";
        disk2.Size = new Size(741, 31);
        disk2.TabIndex = 1;
        // 
        // richTextBox1
        // 
        richTextBox1.Location = new Point(0, 0);
        richTextBox1.Margin = new Padding(0);
        richTextBox1.Name = "richTextBox1";
        richTextBox1.Size = new Size(0, 0);
        richTextBox1.TabIndex = 7;
        richTextBox1.Text = "";
        // 
        // waveViewer1
        // 
        waveViewer1.Location = new Point(978, 0);
        waveViewer1.Name = "waveViewer1";
        waveViewer1.SamplesPerPixel = 128;
        waveViewer1.Size = new Size(881, 295);
        waveViewer1.StartPosition = 0L;
        waveViewer1.TabIndex = 8;
        waveViewer1.WaveStream = null;
        // 
        // button1
        // 
        button1.Location = new Point(1004, 323);
        button1.Name = "button1";
        button1.Size = new Size(112, 34);
        button1.TabIndex = 9;
        button1.Text = "button1";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        // 
        // button2
        // 
        button2.Location = new Point(1134, 325);
        button2.Name = "button2";
        button2.Size = new Size(112, 34);
        button2.TabIndex = 10;
        button2.Text = "button2";
        button2.UseVisualStyleBackColor = true;
        button2.Click += button2_Click;
        // 
        // button3
        // 
        button3.Location = new Point(1265, 323);
        button3.Name = "button3";
        button3.Size = new Size(112, 34);
        button3.TabIndex = 11;
        button3.Text = "button3";
        button3.UseVisualStyleBackColor = true;
        button3.Click += button3_Click;
        // 
        // button4
        // 
        button4.Location = new Point(1400, 325);
        button4.Name = "button4";
        button4.Size = new Size(112, 34);
        button4.TabIndex = 12;
        button4.Text = "button4";
        button4.UseVisualStyleBackColor = true;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1861, 1115);
        Controls.Add(button4);
        Controls.Add(button3);
        Controls.Add(button2);
        Controls.Add(button1);
        Controls.Add(waveViewer1);
        Controls.Add(richTextBox1);
        Controls.Add(groupBox2);
        Controls.Add(btn_restart);
        Controls.Add(groupBox1);
        Controls.Add(pictureBox1);
        Name = "Form1";
        Text = "Apple II+";
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        groupBox2.ResumeLayout(false);
        groupBox2.PerformLayout();
        ResumeLayout(false);
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
    private NAudio.Gui.WaveViewer waveViewer1;
    private Button button1;
    private Button button2;
    private Button button3;
    private Button button4;
    private RadioButton disk1prodos;
    private RadioButton disk1dos;
    private RadioButton disk2prodos;
    private RadioButton disk2dos;
}
