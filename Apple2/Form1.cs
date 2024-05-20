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
    
    float zoom = 1.0f;
    int z80Width = 280;
    int z80Height = 192;

    object lockObj = new object();

    Runtime.State state = new Runtime.State();

    public Form1()
    {
        InitializeComponent();
        zoom = 4f;
        this.Width = (int)(z80Width * zoom + 27);
        this.Height = (int)(z80Height * zoom + 60);
        pictureBox1.Width = (int)(z80Width * zoom);
        pictureBox1.Height = (int)(z80Height * zoom);
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

        
        var roms = new Dictionary<ushort, byte[]>();
        this.KeyDown += OnKeyDown;
        this.KeyPress += OnKeyPress;    
        string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (assemblyPath != null)
            assemblyPath += "/";


        //roms.Add(0xf800, File.ReadAllBytes(assemblyPath + "roms/OriginalF800.rom"));
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

        cpu = new CPU(state, memory, true);
        cpu.Reset();

        threads.Add(Task.Run(() => {
            while (running)
            {
                lock (lockObj) {
                    cpu.RunCycle();
                }
            }
        }));
        Thread.Sleep(1000);
        threads.Add(Task.Run(() => {
            while (running)
            {
                try 
                {
                    lock (lockObj)
                    {
                        pictureBox1.Image = this.VideoGenerator();
                    }
                }
                catch (Exception ex) 
                {
                    
                }
                Thread.Sleep(10);
            }
         }));
    }

    void Reset() {
        
    }
    private void OnKeyPress(object? sender, KeyPressEventArgs e)
    {
        if (e.KeyChar < 32 || e.KeyChar >= 127)
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
                case Keys.B:
                    memory.KeyPressed = 0x82;
                    break;
                case Keys.F12:
                    lock (lockObj)
                    {
                        Thread.Sleep(100);
                        state.PC = 0;
                    }
                
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
                case Keys.Back:
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
                case Keys.Enter:
                    memory.KeyPressed = 0x8d;
                    break;
            }
        }
       

    }

    
    public Bitmap VideoGenerator()
    {
        byte[] bmp = new byte[280 * 192];
        int byteid = 0;
        var cursorH = memory.memory[0x24];
        var cursorV = memory.memory[0x25];

        int posH = 0;
        int posV = 0;
        byte[] linha = new byte[0x28];

            for (int b = 0; b < 3; b++)
            {
                posV = b * 8;
                for (int l = 0; l < 8; l++)
                {
                    linha = new byte[0x28];
                    if ((memory.softswitches.Graphics_Text && memory.softswitches.DisplayFull_Split) || 
                        (memory.softswitches.Graphics_Text && !memory.softswitches.DisplayFull_Split && (b < 2 || (b == 2 && l < 4))))
                    {
                        for (int block = 0; block < 8; block++)
                        {
                            for (ushort c = 0; c < 0x28; c++)
                            {
                                var chr = memory.memory[(ushort)(((0x2000) + (b * 0x28) + (l * 0x80) + c) + block * 0x400)];

                                bool[] blockline = memory.ConvertByteToBoolArray(chr);
                                
                                for (int i = 7; i > 0; i--)
                                {
                                    if (blockline[i])
                                        bmp[byteid] = 0xff;
                                    else
                                        bmp[byteid] = 0x00;

                                    byteid++;
                                }
                            }
                        }
                    } 
                    else
                    {
                        for (ushort c = 0; c < 0x28; c++)
                        {
                            posH = c;

                            var chr = memory.memory[(ushort)(0x400 + (b * 0x28) + (l * 0x80) + c)];

                            if (posV == cursorV && posH == cursorH)
                                chr = DateTime.Now.Millisecond > 500 ? chr : (byte)(chr | 0b10000000);                                                                                                                         

                            linha[c] = chr;
                        }

                        for (int i = 0; i < 8; i++)
                        {

                            for (int j = 0; j < 0x28; j++)
                            {
                                for (int k = 0; k < 7; k++)
                                {
                                    object? objout = memory.charSet[linha[j]].GetValue(i, k);
                                    if (objout != null)
                                    {
                                        if ((bool)objout)
                                            bmp[byteid] = 0xff;
                                        else
                                            bmp[byteid] = 0x0;
                                    }
                                    else
                                        bmp[byteid] = 0x0;
                                    byteid++;
                                }
                            }
                        }
                        posV = posV + 1;
                    }
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