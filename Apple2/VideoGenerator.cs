
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Apple2;

public static class VideoGenerator
{
    public static Bitmap Generate(Runtime.Memory memory, bool color)
    {
        byte[] bmp = new byte[280 * 192];
        int byteid = 0;
        var cursorH = memory.baseRAM[0x24];
        var cursorV = memory.baseRAM[0x25];
        ushort graphicsPage = 0x2000;
        ushort textPage = 0x400;


        int posH = 0;
        int posV = 0;
        byte[] linha = new byte[0x28];

        graphicsPage = (ushort)(memory.softswitches.TextPage1_Page2 ? 0x2000 : 0x4000);
        textPage = (ushort)(memory.softswitches.TextPage1_Page2 ? 0x400 : 0x800);

        for (int b = 0; b < 3; b++)
        {
            posV = b * 8;
            for (int l = 0; l < 8; l++)
            {
                linha = new byte[0x28];
                if ((memory.softswitches.Graphics_Text && memory.softswitches.DisplayFull_Split) ||
                    (memory.softswitches.Graphics_Text && !memory.softswitches.DisplayFull_Split && (b < 2 || (b == 2 && l < 4))))
                {
                    if (memory.softswitches.LoRes_HiRes)
                    {
                        for (ushort c = 0; c < 0x28; c++)
                        {
                            var chr = memory.baseRAM[(ushort)(textPage + (b * 0x28) + (l * 0x80) + c)];
                            linha[c] = chr;
                        }

                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 0x28; j++)
                            {
                                for (int k = 0; k < 7; k++)
                                {
                                    var firstColor = linha[j] & 0b11110000 >> 4;
                                    var secondColor = linha[j] & 0b00001111;
                                    if (i < 4)
                                    {
                                        bmp[byteid] = (byte)secondColor;
                                        byteid++;
                                    }
                                    else
                                    {
                                        bmp[byteid] = (byte)firstColor;
                                        byteid++;
                                    }
                                    
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int block = 0; block < 8; block++)
                        {
                            bool[] blocklineAnt = [false, false, false, false, false, false, false, false];
                            for (ushort c = 0; c < 0x28; c++)
                            {
                                var chr = memory.baseRAM[(ushort)(((graphicsPage) + (b * 0x28) + (l * 0x80) + c) + block * 0x400)];
                                bool[] blockline = memory.ConvertByteToBoolArray(chr);
                                if (color)
                                {
                                    int p1, p2, p3, p4, p5, p6, p7;
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
                }
                else
                {
                    for (ushort c = 0; c < 0x28; c++)
                    {
                        posH = c;

                        var chr = memory.baseRAM[(ushort)(textPage + (b * 0x28) + (l * 0x80) + c)];

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
        if (memory.softswitches.LoRes_HiRes)
        {
            pal.Entries[0x00] = Color.Black;
            pal.Entries[0x01] = Color.MediumVioletRed;
            pal.Entries[0x02] = Color.RoyalBlue;
            pal.Entries[0x03] = Color.Fuchsia;
            pal.Entries[0x04] = Color.MediumSeaGreen;
            pal.Entries[0x05] = Color.Silver;
            pal.Entries[0x06] = Color.DeepSkyBlue;
            pal.Entries[0x07] = Color.Thistle;
            pal.Entries[0x08] = Color.SaddleBrown;
            pal.Entries[0x09] = Color.Tomato;
            pal.Entries[0x0a] = Color.LightSlateGray;
            pal.Entries[0x0b] = Color.HotPink;
            pal.Entries[0x0c] = Color.LimeGreen;
            pal.Entries[0x0d] = Color.Gold;
            pal.Entries[0x0e] = Color.MediumSpringGreen;
            pal.Entries[0x0f] = Color.White;
            pal.Entries[0xff] = Color.White;
        }
        else
        {
            pal.Entries[0x00] = Color.Black;
            pal.Entries[0x01] = Color.Lime;
            pal.Entries[0x02] = Color.Orchid;
            pal.Entries[0x03] = Color.White;
            pal.Entries[0x04] = Color.Black;
            pal.Entries[0x05] = Color.OrangeRed;
            pal.Entries[0x06] = Color.DeepSkyBlue;
            pal.Entries[0x07] = Color.White;
            pal.Entries[0xff] = Color.White;
        }
        

        bitmap.Palette = pal;
        BitmapData bmData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
        IntPtr pNative = bmData.Scan0;
        Marshal.Copy(bmp, 0, pNative, 280 * 192);
        bitmap.UnlockBits(bmData);
        return bitmap;


    }
}
