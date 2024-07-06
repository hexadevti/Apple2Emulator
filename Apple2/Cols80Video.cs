
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Apple2;

public static class Cols80Video
{

    public static Bitmap Generate(Runtime.Memory memory, int pixelSize, bool color)
    {
        int byteid = 0;
        var cursorH = memory.baseRAM[0x57b];
        var cursorV = memory.baseRAM[0x5fb];
        byte[] bmp = new byte[640 * pixelSize * 216 * pixelSize*2];
        int posH = 0;
        byte[] linha = new byte[0x50];

        for (int posV = 0; posV < 24; posV++)
        {
            linha = new byte[0x50];

            for (ushort c = 0; c < 0x50; c++)
            {
                posH = c;
                var chr = memory.cols80RAM[(ushort)((c + (posV * 0x50) + memory.baseRAM[0x6fb] * 0x10) % 0x800)];
                if (posV == cursorV && posH == cursorH)
                {
                    chr = Math.Floor((float)(DateTime.Now.Millisecond / 500)) % 2 == 0 ? (byte)(chr + 0x80) : (byte)(chr);
                }

                linha[c] = chr;
            }

            for (int i = 0; i < 9; i++)
            {
                for (int ps1 = 0; ps1 < pixelSize*2; ps1++)
                {
                    for (int j = 0; j < 0x50; j++)
                    {
                        for (int k = 0; k < 8; k++)
                        {
                            bool invert = false;
                            byte rChar = linha[j];
                            if (rChar > 0x80)
                            {
                                invert = true;
                                rChar = (byte)(rChar - 0x80);
                            }
                            
                            object? objout = memory.cols80CharSet[rChar].GetValue(i, k);
                            for (int ps2 = 0; ps2 < pixelSize; ps2++)
                            {
                                if (objout != null)
                                {
                                    if ((bool)objout)
                                        bmp[byteid] = (byte)(invert ? 0x00 : 0xff);
                                    else
                                        bmp[byteid] = (byte)(invert ? 0xff : 0x00);
                                }
                                else
                                    bmp[byteid] = 0x0;
                                byteid++;
                            }
                        }
                    }
                }
            }

        }


        Bitmap bitmap = new Bitmap(640 * pixelSize, 216 * pixelSize * 2, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
        ColorPalette pal = bitmap.Palette;
        pal.Entries[0x00] = Color.Black;
        pal.Entries[0xff] = Color.White;

        bitmap.Palette = pal;
        BitmapData bmData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
        IntPtr pNative = bmData.Scan0;
        Marshal.Copy(bmp, 0, pNative, 640 * pixelSize * 216 * pixelSize * 2);
        bitmap.UnlockBits(bmData);
        return bitmap;

    }
}
