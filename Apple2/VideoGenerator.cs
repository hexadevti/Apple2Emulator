
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Apple2;

public static class VideoGenerator
{
    public static Bitmap Generate(Runtime.Memory memory, bool color)
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
                        bool[] blocklineAnt = new bool[] { false, false, false, false, false, false, false, false };
                        for (ushort c = 0; c < 0x28; c++)
                        {
                            var chr = memory.memory[(ushort)(((0x2000) + (b * 0x28) + (l * 0x80) + c) + block * 0x400)];
                            bool[] blockline = memory.ConvertByteToBoolArray(chr);
                            if (color)
                            {
                                int p1,p2,p3,p4,p5,p6,p7;
                                if (byteid % 2 == 0) // Odd
                                {
                                    p1 = (blockline[0] ? 4 : 0) + (blockline[7] ? 2 : 0) + (blocklineAnt[1] ? 1 : 0);
                                    p2 = (blockline[0] ? 4 : 0) + (blockline[7] ? 2 : 0) + (blockline[6] ? 1 : 0);
                                    p3 = (blockline[0] ? 4 : 0) + (blockline[5] ? 2 : 0) + (blockline[6] ? 1 : 0);
                                    p4 = (blockline[0] ? 4 : 0) + (blockline[5] ? 2 : 0) + (blockline[4] ? 1 : 0);
                                    p5 = (blockline[0] ? 4 : 0) + (blockline[3] ? 2 : 0) + (blockline[4] ? 1 : 0);
                                    p6 = (blockline[0] ? 4 : 0) + (blockline[3] ? 2 : 0) + (blockline[2] ? 1 : 0);
                                    p7 = (blockline[0] ? 4 : 0) + (blockline[1] ? 2 : 0) + (blockline[2] ? 1 : 0);
                                }
                                else //Even
                                {
                                    p1 = (blockline[0] ? 4 : 0) + (blocklineAnt[1] ? 2 : 0) + (blockline[7] ? 1 : 0);
                                    p2 = (blockline[0] ? 4 : 0) + (blockline[6] ? 2 : 0) + (blockline[7] ? 1 : 0);
                                    p3 = (blockline[0] ? 4 : 0) + (blockline[6] ? 2 : 0) + (blockline[5] ? 1 : 0);
                                    p4 = (blockline[0] ? 4 : 0) + (blockline[4] ? 2 : 0) + (blockline[5] ? 1 : 0);
                                    p5 = (blockline[0] ? 4 : 0) + (blockline[4] ? 2 : 0) + (blockline[3] ? 1 : 0);
                                    p6 = (blockline[0] ? 4 : 0) + (blockline[2] ? 2 : 0) + (blockline[3] ? 1 : 0);
                                    p7 = (blockline[0] ? 4 : 0) + (blockline[2] ? 2 : 0) + (blockline[1] ? 1 : 0);
                                }
                                bmp[byteid] = (byte)p1;    
                                byteid++;
                                bmp[byteid] = (byte)p2;    
                                byteid++;
                                bmp[byteid] = (byte)p3;    
                                byteid++;
                                bmp[byteid] = (byte)p4;    
                                byteid++;
                                bmp[byteid] = (byte)p5;    
                                byteid++;
                                bmp[byteid] = (byte)p6;    
                                byteid++;
                                bmp[byteid] = (byte)p7;    
                                byteid++;

                                blocklineAnt = blockline;
                            }
                            else
                            {
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
        ColorPalette pal = bitmap.Palette;
        pal.Entries[0x00] = Color.Black;
        pal.Entries[0x01] = Color.Lime;
        pal.Entries[0x02] = Color.Orchid;
        pal.Entries[0x03] = Color.White;
        pal.Entries[0x04] = Color.Black;
        pal.Entries[0x05] = Color.OrangeRed;
        pal.Entries[0x06] = Color.DeepSkyBlue;
        pal.Entries[0x07] = Color.White;
        pal.Entries[0xff] = Color.White;
    
        bitmap.Palette = pal;
        BitmapData bmData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
        IntPtr pNative = bmData.Scan0;
        Marshal.Copy(bmp, 0, pNative, 280*192);
        bitmap.UnlockBits(bmData);
        return bitmap;


    }
}
