using Runtime;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.AxHost;
using System.Windows.Input;
using Microsoft.VisualBasic.Devices;

namespace Apple2;

public partial class Form1 : Form
{
    private Memory memory { get; set; }

    private CPU cpu { get; set;}
    
    float pixelSize = 1.0f;
    int z80Width = 280;
    int z80Height = 192;

    public Form1()
    {
        InitializeComponent();
        pixelSize = 4;
        this.Width = (int)(z80Width * pixelSize + 27);
        this.Height = (int)(z80Height * pixelSize + 60);
        
        pictureBox1.Width = (int)(z80Width * pixelSize);
        pictureBox1.Height = (int)(z80Height * pixelSize);
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        var roms = new Dictionary<ushort, byte[]>();
        this.KeyDown += OnKeyDown;
        this.KeyPress += OnKeyPress;    
        string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (assemblyPath != null)
            assemblyPath += "/";

        roms.Add(0xf800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF800.rom"));
        roms.Add(0xf000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftF000.rom"));
        roms.Add(0xe800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE800.rom"));
        roms.Add(0xe000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftE000.rom"));
        roms.Add(0xd800, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD800.rom"));
        roms.Add(0xd000, File.ReadAllBytes(assemblyPath + "roms/ApplesoftD000.rom"));
        memory = new Memory(0xffff);

        foreach (var item in roms)
        {
            memory.WriteAt(item.Key, item.Value);
        }

        memory.LoadChars(File.ReadAllBytes(assemblyPath + "roms/CharROM.rom"));

        List<Task> threads = new List<Task>();

        bool running = true;

        cpu = new CPU(new Runtime.State(), memory, true);
        cpu.Reset();

        threads.Add(Task.Run(() => {
            while (running)
            {
                cpu.RunCycle();
            }
        }));
        threads.Add(Task.Run(() => {
            while (running)
            {
                Bitmap bitmap = this.VideoGenerator();
                pictureBox1.Image = bitmap;
                
                Thread.Sleep(10);
            }
        }));

        //Task.WaitAll(threads.ToArray());

       

    }

    private void OnKeyPress(object? sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == 27)
            return;
         memory.KeyPressed = (byte)(Encoding.ASCII.GetBytes( new[] { e.KeyChar.ToString().ToUpper()[0]})[0] | 0b10000000);
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Control)
        {
            switch (e.KeyCode)
            {
                case Keys.C:
                    memory.KeyPressed = 0x83;
                    break;
            }
        }
        else
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    memory.KeyPressed = 0x88;
                    break;
                case Keys.Right:
                    memory.KeyPressed = 0x95;
                    break;
                case Keys.Up:
                    memory.KeyPressed = 0x8b;
                    break;
                case Keys.Down:
                    memory.KeyPressed = 0x8a;
                    break;
                case Keys.Escape:
                    memory.KeyPressed = 0x9b;
                    break;
            }
        }
       

    }

    private void OnPreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
    {
         switch (e.KeyCode)
            {
                case Keys.Alt:
                    switch (e.KeyCode)
                    {
                        case Keys.C:
                           memory.KeyPressed = 0x83;
                           break; 
                        case Keys.F12:
                           cpu.Reset();
                           break; 
                    }
                    break;
            
                default:
                    switch (e.KeyCode)
                    {
                        case Keys.Left:
                            memory.KeyPressed = 0x88;
                            break;
                        case Keys.Right:
                            memory.KeyPressed = 0x95;
                            break;
                        case Keys.Up:
                            memory.KeyPressed = 0x8b;
                            break;
                        case Keys.Down:
                            memory.KeyPressed = 0x8a;
                            break;
                        case Keys.Enter:
                            memory.KeyPressed = 0x8D;
                            break;
                        case Keys.Escape:
                            memory.KeyPressed = 0x9b;
                            break;
                        default:
                            memory.KeyPressed = (byte)(e.KeyValue | 0b10000000);
                            break;
                    }
                    break;
            }
    }

    private void Form1_KeyPress(object? sender, KeyPressEventArgs e)
    {
        var consoleKeyInfo = e.KeyChar;
        
    }

    public Bitmap VideoGenerator()
    {
        var cursorH = memory.memory[0x24];
        var cursorV = memory.memory[0x25];

        int outputRow = 0;

        byte[] bmp = new byte[280 * 192];

        int posH = 0;
        int posV = 0;
        Pen pen1 = new Pen(Color.White);
        Pen pen2 = new Pen(Color.Black);
        pen1.Width = pixelSize / 2;

        int byteid = 0;

        for (int b = 0; b < 3; b++)
        {
            posV = b * 8;
            for (int l = 0; l < 8; l++)
            {

                byte[] linha = new byte[0x28];
                for (ushort c = 0; c < 0x28; c++)
                {
                    posH = c;

                    var chr = memory.memory[(ushort)(0x400 + (b * 0x28) + (l * 0x80) + c)];

                    if (posV == cursorV && posH == cursorH)
                        chr = DateTime.Now.Millisecond > 500 ? chr : (byte)192;

                    if (chr == 96)
                        chr = DateTime.Now.Millisecond > 500 ? (byte)192 : (byte)160;
                    if (chr == 0)
                        chr = 160;
                    linha[c] = chr;
                }

                for (int i = 0; i < 8; i++)
                {

                    for (int j = 0; j < 0x28; j++)
                    {
                        for (int k = 0; k < 7; k++)
                        {
                            if ((bool)memory.charSet[linha[j]].GetValue(i, k))
                                bmp[byteid] = 0xff;
                            else
                                bmp[byteid] = 0x0;
                            byteid++;
                            
                        }
                    }



                    // print graphic line

                    outputRow++;
                }
                posV = posV + 1;

            }
        }

        Bitmap bitmap = new Bitmap(280, 192, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
        BitmapData bmData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
        IntPtr pNative = bmData.Scan0;
        Marshal.Copy(bmp, 0, pNative, 280*192);
        bitmap.UnlockBits(bmData);

        return bitmap;


    }
}