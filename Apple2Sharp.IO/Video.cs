using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;
using Apple2Sharp.Mainboard;

namespace Apple2Sharp
{
    public static class Video
    {

        public static Bitmap Generate(Apple2Board mainBoard, int pixelSize)
        {
            int byteid = 0;

            ushort graphicsPage = 0x2000;
            ushort textPage = 0x400;
            byte[] bmp = new byte[280 * pixelSize * 192 * pixelSize];


            int posH = 0;
            int posV = 0;
            byte[] linha = new byte[0x28];
            byte[] linha80 = new byte[0x50];
            mainBoard.softswitches.Vertical_blankingOn_Off = false;

            graphicsPage = (ushort)(mainBoard.softswitches.Page1_Page2 ? 0x2000 : 0x4000);
            textPage = (ushort)(mainBoard.softswitches.Page1_Page2 ? 0x400 : 0x800);

            for (int b = 0; b < 3; b++)
            {
                posV = b * 8;
                for (int l = 0; l < 8; l++)
                {

                    linha = new byte[0x28];
                    if ((mainBoard.softswitches.Graphics_Text && mainBoard.softswitches.DisplayFull_Split) ||
                        (mainBoard.softswitches.Graphics_Text && !mainBoard.softswitches.DisplayFull_Split && (b < 2 || (b == 2 && l < 4))))
                    {
                        if (mainBoard.softswitches.LoRes_HiRes)
                        {
                            for (ushort c = 0; c < 0x28; c++)
                            {
                                var chr = mainBoard.baseRAM[(ushort)(textPage + (b * 0x28) + (l * 0x80) + c)];
                                linha[c] = chr;
                            }

                            for (int i = 0; i < 8; i++)
                            {
                                for (int ps1 = 0; ps1 < pixelSize; ps1++)
                                {
                                    for (int j = 0; j < 0x28; j++)
                                    {
                                        for (int k = 0; k < 7; k++)
                                        {
                                            for (int ps2 = 0; ps2 < pixelSize; ps2++)
                                            {
                                                var firstColor = (linha[j] & 0b11110000) >> 4;
                                                var secondColor = linha[j] & 0b00001111;
                                                if (mainBoard.scanLines && pixelSize > 2 && ps1 == pixelSize - 1)
                                                {
                                                    bmp[byteid] = 0;
                                                }
                                                else if (i < 4)
                                                {
                                                    bmp[byteid] = (byte)secondColor;
                                                }
                                                else
                                                {
                                                    bmp[byteid] = (byte)firstColor;
                                                }
                                                byteid++;
                                            }

                                        }
                                    }
                                }
                            }
                        } 
                        else if (mainBoard.softswitches.DHiResOn_Off)
                        {
                            for (int block = 0; block < 8; block++)
                            {
                                for (int ps1 = 0; ps1 < pixelSize; ps1++)
                                {
                                    bool[] line = new bool[0x50 * 7];
                                    int lineId = 0;

                                    for (ushort c = 0; c < 0x50; c++)
                                    {
                                        byte chr;
                                        if (c % 2 == 0)
                                        {
                                            chr = mainBoard.auxRAM[0, (ushort)((0x2000 + (b * 0x28) + (l * 0x80) + c/2) + block * 0x400)];
                                        }
                                        else
                                        {
                                            chr = mainBoard.baseRAM[(ushort)((0x2000 + (b * 0x28) + (l * 0x80) + (c-1)/2) + block * 0x400)];
                                        }
                                        bool[] blockline = Tools.ConvertByteToBoolArray(chr, true);


                                        if (mainBoard.videoColor)
                                        {
                                            for (int i = 7; i > 0; i--)
                                            {
                                                line[lineId] = blockline[i];
                                                lineId++;
                                            }
                                        }
                                        else
                                        {
                                            for (int i = 7; i > 0; i--)
                                            {
                                                for (int ps2 = 0; ps2 < pixelSize / 2; ps2++)
                                                {
                                                    if (mainBoard.scanLines && pixelSize > 2 && ps1 == pixelSize - 1)
                                                        bmp[byteid] = 0;
                                                    else
                                                    {
                                                        if (blockline[i])
                                                            bmp[byteid] = 0xff;
                                                        else
                                                            bmp[byteid] = 0x00;
                                                    }
                                                    byteid++;
                                                }
                                            }
                                        }
                                    }

                                    if (mainBoard.videoColor)
                                    {
                                        for (int i = 0; i < line.Length; i = i + 4)
                                        {
                                            for ( int ps2 = 0; ps2 < pixelSize * 2; ps2++)
                                            {
                                                if (mainBoard.scanLines && pixelSize > 2 && ps1 == pixelSize - 1)
                                                    bmp[byteid] = 0;
                                                else
                                                {
                                                    int color = (line[i] ? 8 : 0) + (line[i + 1] ? 4 : 0) + (line[i+2] ? 2 : 0) + (line[i + 3] ? 1 : 0);
                                                    bmp[byteid] = (byte)color;
                                                }
                                                byteid++;
                                            }
                                        }
                                    }
                                    
                                }
                            }
                        }
                        else
                        {
                            for (int block = 0; block < 8; block++)
                            {
                                for (int ps1 = 0; ps1 < pixelSize; ps1++)
                                {
                                    int[] line = new int[0x28 * 7];
                                    bool[] blocklineAnt = new bool[] { false, false, false, false, false, false, false, false };
                                    for (ushort c = 0; c < 0x28; c++)
                                    {
                                        var chr = mainBoard.baseRAM[(ushort)(((graphicsPage) + (b * 0x28) + (l * 0x80) + c) + block * 0x400)];
                                        bool[] blockline = Tools.ConvertByteToBoolArray(chr);
                                        if (mainBoard.videoColor)
                                        {
                                            int[] pixels = new int[7];
                                            if ((byteid / pixelSize) % 2 == 0) // Odd
                                            {
                                                pixels[0] = (blockline[0] ? 4 : 0) + (blockline[7] ? 2 : 0) + (blocklineAnt[1] ? 1 : 0);
                                                pixels[1] = (blockline[0] ? 4 : 0) + (blockline[7] ? 2 : 0) + (blockline[6] ? 1 : 0);
                                                pixels[2] = (blockline[0] ? 4 : 0) + (blockline[5] ? 2 : 0) + (blockline[6] ? 1 : 0);
                                                pixels[3] = (blockline[0] ? 4 : 0) + (blockline[5] ? 2 : 0) + (blockline[4] ? 1 : 0);
                                                pixels[4] = (blockline[0] ? 4 : 0) + (blockline[3] ? 2 : 0) + (blockline[4] ? 1 : 0);
                                                pixels[5] = (blockline[0] ? 4 : 0) + (blockline[3] ? 2 : 0) + (blockline[2] ? 1 : 0);
                                                pixels[6] = (blockline[0] ? 4 : 0) + (blockline[1] ? 2 : 0) + (blockline[2] ? 1 : 0);

                                            }
                                            else //Even
                                            {
                                                pixels[0] = (blockline[0] ? 4 : 0) + (blocklineAnt[1] ? 2 : 0) + (blockline[7] ? 1 : 0);
                                                pixels[1] = (blockline[0] ? 4 : 0) + (blockline[6] ? 2 : 0) + (blockline[7] ? 1 : 0);
                                                pixels[2] = (blockline[0] ? 4 : 0) + (blockline[6] ? 2 : 0) + (blockline[5] ? 1 : 0);
                                                pixels[3] = (blockline[0] ? 4 : 0) + (blockline[4] ? 2 : 0) + (blockline[5] ? 1 : 0);
                                                pixels[4] = (blockline[0] ? 4 : 0) + (blockline[4] ? 2 : 0) + (blockline[3] ? 1 : 0);
                                                pixels[5] = (blockline[0] ? 4 : 0) + (blockline[2] ? 2 : 0) + (blockline[3] ? 1 : 0);
                                                pixels[6] = (blockline[0] ? 4 : 0) + (blockline[2] ? 2 : 0) + (blockline[1] ? 1 : 0);
                                            }


                                            for (int id = 0; id < 7; id++)
                                            {
                                                for (int ps2 = 0; ps2 < pixelSize; ps2++)
                                                {
                                                    if (mainBoard.scanLines && pixelSize > 2 && ps1 == pixelSize - 1)
                                                        bmp[byteid] = 0;
                                                    else
                                                        bmp[byteid] = (byte)pixels[id];
                                                    byteid++;
                                                }

                                            }
                                            blocklineAnt = blockline;
                                        }
                                        else
                                        {
                                            for (int i = 7; i > 0; i--)
                                            {
                                                for (int ps2 = 0; ps2 < pixelSize; ps2++)
                                                {
                                                    if (mainBoard.scanLines && pixelSize > 2 && ps1 == pixelSize - 1)
                                                        bmp[byteid] = 0;
                                                    else
                                                    {
                                                        if (blockline[i])
                                                            bmp[byteid] = 0xff;
                                                        else
                                                            bmp[byteid] = 0x00;
                                                    }
                                                    byteid++;
                                                }
                                            }
                                        }
                                    }


                                }
                            }
                        }
                    }
                    else if (mainBoard.softswitches.Cols40_80)
                    {
                        for (ushort c = 0; c < 0x28; c++)
                        {
                            posH = c;

                            var chr = mainBoard.baseRAM[(ushort)(textPage + (b * 0x28) + (l * 0x80) + c)];
                            if (!mainBoard.appleIIe)
                            {
                                if (chr >= 0x40 && chr < 0x80)
                                    chr = Math.Floor((float)(DateTime.Now.Millisecond / 500)) % 2 == 0 ? (byte)(chr + 0x40) : chr;
                                if (posV == mainBoard.baseZP[0x25] && posH == mainBoard.baseZP[0x24])
                                    chr = Math.Floor((float)(DateTime.Now.Millisecond / 500)) % 2 == 0 ? (byte)(chr + 0x40) : chr;
                            }
                            else
                            {
                                // if (chr >= 0x40 && chr < 0x60)
                                //     chr = Math.Floor((float)(DateTime.Now.Millisecond / 500)) % 2 == 0 ? (byte)(chr + 0x20) : chr;
                                if (chr >= 0x60 && chr < 0x80)
                                    chr = Math.Floor((float)(DateTime.Now.Millisecond / 500)) % 2 == 0 ? (byte)(chr + 0x80) : chr;
                            }
                            linha[c] = chr;
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            for (int ps1 = 0; ps1 < pixelSize; ps1++)
                            {
                                for (int j = 0; j < 0x28; j++)
                                {
                                    for (int k = 0; k < 7; k++)
                                    {
                                        object? objout = mainBoard.charSet[linha[j]].GetValue(i, k);
                                        for (int ps2 = 0; ps2 < pixelSize; ps2++)
                                        {
                                            if (objout != null)
                                            {
                                                if (mainBoard.scanLines && pixelSize > 2 && ps1 == pixelSize - 1)
                                                    bmp[byteid] = 0;
                                                else
                                                {
                                                    if ((bool)objout)
                                                        bmp[byteid] = 0xff;
                                                    else
                                                        bmp[byteid] = 0x0;
                                                }
                                            }
                                            else
                                                bmp[byteid] = 0x0;
                                            byteid++;
                                        }
                                    }
                                }
                            }
                        }
                        posV = posV + 1;
                    }
                    else if (mainBoard.appleIIe && !mainBoard.softswitches.Cols40_80)
                    {
                        for (ushort c = 0; c < 0x50; c++)
                        {
                            byte chr;
                            if (c % 2 == 0)
                            {
                                chr = mainBoard.auxRAM[0, (ushort)(0x400 + (b * 0x28) + (l * 0x80) + c/2)];
                            }
                            else
                            {
                                chr = mainBoard.baseRAM[(ushort)(0x400 + (b * 0x28) + (l * 0x80) + (c-1)/2)];
                            }
                            linha80[c] = chr;
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            for (int ps1 = 0; ps1 < pixelSize; ps1++)
                            {
                                for (int j = 0; j < 0x50; j++)
                                {
                                    for (int k = 0; k < 7; k++)
                                    {
                                        object? objout = mainBoard.charSet[linha80[j]].GetValue(i, k);
                                        for (int ps2 = 0; ps2 < pixelSize/2; ps2++)
                                        {
                                            if (objout != null)
                                            {
                                                if (mainBoard.scanLines && pixelSize > 2 && ps1 == pixelSize - 1)
                                                    bmp[byteid] = 0;
                                                else
                                                {
                                                    if ((bool)objout)
                                                        bmp[byteid] = 0xff;
                                                    else
                                                        bmp[byteid] = 0x0;
                                                }
                                            }
                                            else
                                                bmp[byteid] = 0x0;
                                            byteid++;
                                        }
                                    }
                                }
                            }
                        }
                        posV = posV + 1;
                    }


                }
            }

            if (!mainBoard.softswitches.DHiResOn_Off && !mainBoard.softswitches.LoRes_HiRes && mainBoard.idealized)
            {

                for (int l = 0; l < 192 * pixelSize; l++)
                {
                    int lineSize = 0x28 * 7 * pixelSize;
                    int pixelSize2 = pixelSize / 2;
                    for (int i = 0; i < lineSize; i = i + pixelSize2)
                    {
                        if (i >= pixelSize && i < lineSize - pixelSize)
                        {
                            int actualPixel = lineSize * l + i;
                            int upperPixel = l > pixelSize - 1 ? actualPixel - lineSize * pixelSize : -1;
                            int bottomPixel = l < (192 * pixelSize - pixelSize) ? actualPixel + lineSize * pixelSize : -1;
                            int lastPixel = actualPixel - pixelSize;
                            int lastPixel2 = actualPixel - pixelSize * 2;
                            int nextPixel = actualPixel + pixelSize;
                            int nextPixel2 = actualPixel + pixelSize * 2;
                            int finalPixel = -1;
                            int finalPixel2 = -1;
                            if (bmp[actualPixel] == 1 || bmp[actualPixel] == 2 || bmp[actualPixel] == 5 || bmp[actualPixel] == 6) // Color
                            {
                                if (bmp[lastPixel] == 0 || bmp[lastPixel] == 4) // last black
                                {
                                    if (bmp[nextPixel] == 3 || bmp[nextPixel] == 7) // next white
                                    {
                                        finalPixel = 0; // 
                                        finalPixel2 = 7; //
                                    }
                                    else if (bmp[nextPixel] == 0 || bmp[nextPixel] == 4) // next black
                                    {
                                        finalPixel = 7; // 
                                        finalPixel2 = 0; //
                                    }

                                }
                                else if (bmp[lastPixel] == 3 || bmp[lastPixel] == 7) // last white
                                {
                                    if (bmp[nextPixel] == 0 || bmp[nextPixel] == 4) // next black
                                    {
                                        finalPixel = 7; // white | white,black | black
                                        finalPixel2 = 0; //
                                    }
                                    else if (bmp[nextPixel] == 1 || bmp[nextPixel] == 2 || bmp[nextPixel] == 5 || bmp[nextPixel] == 6) // next color
                                    {
                                        if (bmp[nextPixel2] == 3 || bmp[nextPixel2] == 7) // second next white
                                        {
                                            finalPixel = 7; // 
                                            finalPixel2 = 0; //
                                        }
                                    }
                                    else if (bmp[nextPixel] == 3 || bmp[nextPixel] == 7) // next white
                                    {
                                        finalPixel = 0; // white | white,black | black
                                        finalPixel2 = 7; //
                                    }
                                }
                            }

                            if (finalPixel != -1)
                                for (int ps2 = 0; ps2 < pixelSize2; ps2++)
                                {
                                    bmp[actualPixel + ps2] = (byte)finalPixel;
                                    bmp[actualPixel + ps2 + pixelSize2] = (byte)finalPixel2;
                                }
                        }
                    }
                }
            }
            else if (mainBoard.appleIIe && mainBoard.softswitches.DHiResOn_Off && mainBoard.videoColor && mainBoard.idealized)
            {
                int lineSize = 140 * pixelSize * 2;
                for (int l = 0; l < 192 * pixelSize; l = l + 1)
                {
                    try
                    {
                        byte lastPixel = 0x0;
                        for (int i = 0; i < lineSize; i = i + pixelSize * 2)
                        {
                            if (i < lineSize - pixelSize * 2)
                            {
                                int pixelId = lineSize * l + i;
                                byte actualPixel = bmp[pixelId];
                                byte nextPixel = bmp[pixelId + pixelSize * 2];
                                byte[] convertedByte = new byte[4];
                                if (actualPixel == 0x0)
                                {
                                    convertedByte[0] = 0x0;
                                    convertedByte[0] = 0x0;
                                    convertedByte[1] = 0x0;
                                    convertedByte[1] = 0x0;
                                    convertedByte[2] = 0x0;
                                    convertedByte[2] = 0x0;
                                    convertedByte[3] = 0x0;
                                    convertedByte[3] = 0x0;
                                }
                                else if (actualPixel == 0x1)
                                {
                                    if (lastPixel % 2 == 0)
                                    {
                                        convertedByte[0] = 0x0;
                                        convertedByte[0] = 0x0;
                                        convertedByte[1] = 0x0;
                                        convertedByte[1] = 0x0;
                                        convertedByte[2] = 0x0;
                                        convertedByte[2] = 0x0;
                                    }
                                    else
                                    {
                                        convertedByte[0] = 0x1;
                                        convertedByte[0] = 0x1;
                                        convertedByte[1] = 0x1;
                                        convertedByte[1] = 0x1;
                                        convertedByte[2] = 0x1;
                                        convertedByte[2] = 0x1;
                                    }
                                    if (nextPixel >= 0xc)
                                    {
                                        convertedByte[3] = 0xd;
                                        convertedByte[3] = 0xd;
                                    }
                                    else if (nextPixel >= 0x8)
                                    {
                                        convertedByte[3] = 0x9;
                                        convertedByte[3] = 0x9;
                                    }
                                    else if (nextPixel >= 0x4)
                                    {
                                        convertedByte[3] = 0x5;
                                        convertedByte[3] = 0x5;
                                    }
                                    else
                                    {
                                        convertedByte[3] = 0x1;
                                        convertedByte[3] = 0x1;
                                    }
                                }
                                else if (actualPixel == 0x2)
                                {
                                    if (lastPixel % 2 == 0)
                                    {
                                        if (lastPixel % 4 == 0)
                                        {
                                            convertedByte[0] = 0x0;
                                            convertedByte[0] = 0x0;
                                            convertedByte[1] = 0x0;
                                            convertedByte[1] = 0x0;
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0x2;
                                            convertedByte[0] = 0x2;
                                            convertedByte[1] = 0x2;
                                            convertedByte[1] = 0x2;
                                        }
                                    }
                                    else
                                    {
                                        convertedByte[0] = 0x3;
                                        convertedByte[0] = 0x3;
                                        convertedByte[1] = 0x3;
                                        convertedByte[1] = 0x3;
                                    }
                                    if (nextPixel >= 0xc)
                                    {
                                        convertedByte[2] = 0xa;
                                        convertedByte[2] = 0xa;
                                        convertedByte[3] = 0xe;
                                        convertedByte[3] = 0xe;
                                    }
                                    else if (nextPixel >= 0x8)
                                    {
                                        convertedByte[2] = 0xa;
                                        convertedByte[2] = 0xa;
                                        convertedByte[3] = 0xa;
                                        convertedByte[3] = 0xa;
                                    }
                                    else if (nextPixel >= 0x4)
                                    {
                                        convertedByte[2] = 0x2;
                                        convertedByte[2] = 0x2;
                                        convertedByte[3] = 0x6;
                                        convertedByte[3] = 0x6;
                                    }
                                    else if (nextPixel >= 0x2)
                                    {
                                        convertedByte[2] = 0x2;
                                        convertedByte[2] = 0x2;
                                        convertedByte[3] = 0x2;
                                        convertedByte[3] = 0x2;
                                    }
                                    else
                                    {
                                        convertedByte[2] = 0x2;
                                        convertedByte[2] = 0x2;
                                        convertedByte[3] = 0x0;
                                        convertedByte[3] = 0x0;
                                    }
                                }
                                else if (actualPixel == 0x3)
                                {
                                    if (lastPixel % 2 == 0)
                                    {
                                        if (lastPixel == 0x0 || lastPixel == 0x4 || lastPixel == 0x8 || lastPixel == 0xc)
                                        {
                                            convertedByte[0] = 0x0;
                                            convertedByte[0] = 0x0;
                                            convertedByte[1] = 0x0;
                                            convertedByte[1] = 0x0;
                                        } 
                                        else
                                        {
                                            convertedByte[0] = 0x2;
                                            convertedByte[0] = 0x2;
                                            convertedByte[1] = 0x2;
                                            convertedByte[1] = 0x2;
                                        }
                                    }
                                    else
                                    {
                                        convertedByte[0] = 0x3;
                                        convertedByte[0] = 0x3;
                                        convertedByte[1] = 0x3;
                                        convertedByte[1] = 0x3;
                                    }
                                    if (nextPixel >= 0xc)
                                    {
                                        convertedByte[2] = 0xf;
                                        convertedByte[2] = 0xf;
                                        convertedByte[3] = 0xf;
                                        convertedByte[3] = 0xf;
                                    }
                                    else if (nextPixel >= 0x8)
                                    {
                                        convertedByte[2] = 0xb;
                                        convertedByte[2] = 0xb;
                                        convertedByte[3] = 0xb;
                                        convertedByte[3] = 0xb;
                                    }
                                    else if (nextPixel >= 0x4)
                                    {
                                        convertedByte[2] = 0x3;
                                        convertedByte[2] = 0x3;
                                        convertedByte[3] = 0x7;
                                        convertedByte[3] = 0x7;
                                    }
                                    else
                                    {
                                        convertedByte[2] = 0x3;
                                        convertedByte[2] = 0x3;
                                        convertedByte[3] = 0x3;
                                        convertedByte[3] = 0x3;
                                    }
                                }
                                else if (actualPixel == 0x4)
                                {
                                    if (lastPixel % 2 == 0)
                                    {
                                        if (lastPixel % 4 == 0)
                                        {
                                            if (lastPixel % 8 == 0)
                                            {
                                                convertedByte[0] = 0x0;
                                                convertedByte[0] = 0x0;
                                                convertedByte[1] = 0x4;
                                                convertedByte[1] = 0x4;
                                            }
                                            else
                                            {
                                                convertedByte[0] = 0x4;
                                                convertedByte[0] = 0x4;
                                                convertedByte[1] = 0x4;
                                                convertedByte[1] = 0x4;
                                            }
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0x6;
                                            convertedByte[0] = 0x6;
                                            convertedByte[1] = 0x4;
                                            convertedByte[1] = 0x4;
                                        }
                                    }
                                    else
                                    {
                                        if (lastPixel == 0x1 || lastPixel == 0x5 || lastPixel == 0x9 || lastPixel == 0xd)
                                        {
                                            convertedByte[0] = 0x5;
                                            convertedByte[0] = 0x5;
                                            convertedByte[1] = 0x5;
                                            convertedByte[1] = 0x5;
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0x7;
                                            convertedByte[0] = 0x7;
                                            convertedByte[1] = 0x5;
                                            convertedByte[1] = 0x5;
                                        }
                                    }
                                    if (nextPixel >= 0x8)
                                    {
                                        convertedByte[2] = 0xc;
                                        convertedByte[2] = 0xc;
                                        convertedByte[3] = 0xc;
                                        convertedByte[3] = 0xc;
                                    }
                                    else if (nextPixel >= 0x4)
                                    {
                                        convertedByte[2] = 0x4;
                                        convertedByte[2] = 0x4;
                                        convertedByte[3] = 0x4;
                                        convertedByte[3] = 0x4;
                                    }
                                    else
                                    {
                                        convertedByte[2] = 0x0;
                                        convertedByte[2] = 0x0;
                                        convertedByte[3] = 0x0;
                                        convertedByte[3] = 0x0;
                                    }
                                }
                                else if (actualPixel == 0x5)
                                {
                                    convertedByte[1] = 0x5;
                                    convertedByte[1] = 0x5;
                                    if (lastPixel % 2 == 0)
                                    {
                                        if (lastPixel % 4 == 0)
                                        {
                                            if (lastPixel % 8 == 0)
                                            {
                                                convertedByte[0] = 0x0;
                                                convertedByte[0] = 0x0;
                                            }
                                            else
                                            {
                                                convertedByte[0] = 0x4;
                                                convertedByte[0] = 0x4;
                                            }
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0x6;
                                            convertedByte[0] = 0x6;
                                        }
                                    }
                                    else
                                    {
                                        if (lastPixel == 0x1 || lastPixel == 0x5 || lastPixel == 0x9 || lastPixel == 0xd)
                                        {
                                            convertedByte[0] = 0x5;
                                            convertedByte[0] = 0x5;
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0x7;
                                            convertedByte[0] = 0x7;
                                        }
                                    }
                                    if (nextPixel >= 0x8)
                                    {
                                        convertedByte[2] = 0xd;
                                        convertedByte[2] = 0xd;
                                        convertedByte[3] = 0xd;
                                        convertedByte[3] = 0xd;
                                    }
                                    else
                                    {
                                        convertedByte[2] = 0x5;
                                        convertedByte[2] = 0x5;
                                        convertedByte[3] = 0x5;
                                        convertedByte[3] = 0x5;
                                    }
                                }
                                else if (actualPixel == 0x6)
                                {
                                    if (lastPixel % 2 == 0)
                                    {
                                        if (lastPixel % 4 == 0)
                                        {
                                            if (lastPixel % 8 == 0)
                                            {
                                                convertedByte[0] = 0x0;
                                                convertedByte[0] = 0x0;
                                                convertedByte[1] = 0x6;
                                                convertedByte[1] = 0x6;
                                            }
                                            else
                                            {
                                                convertedByte[0] = 0x4;
                                                convertedByte[0] = 0x4;
                                                convertedByte[1] = 0x6;
                                                convertedByte[1] = 0x6;
                                            }
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0x6;
                                            convertedByte[0] = 0x6;
                                            convertedByte[1] = 0x6;
                                            convertedByte[1] = 0x6;
                                        }
                                    }
                                    else
                                    {
                                        convertedByte[0] = 0x7;
                                        convertedByte[0] = 0x7;
                                        convertedByte[1] = 0x7;
                                        convertedByte[1] = 0x7;
                                    }
                                    if (nextPixel >= 0x8)
                                    {
                                        convertedByte[2] = 0xe;
                                        convertedByte[2] = 0xe;
                                        convertedByte[3] = 0xe;
                                        convertedByte[3] = 0xe;
                                    } 
                                    else if (nextPixel >= 0x4)
                                    {
                                        convertedByte[2] = 0x6;
                                        convertedByte[2] = 0x6;
                                        convertedByte[3] = 0x6;
                                        convertedByte[3] = 0x6;
                                    }
                                    else if (nextPixel >= 0x2)
                                    {
                                        convertedByte[2] = 0x6;
                                        convertedByte[2] = 0x6;
                                        convertedByte[3] = 0x2;
                                        convertedByte[3] = 0x2;
                                    }
                                    else
                                    {
                                        convertedByte[2] = 0x6;
                                        convertedByte[2] = 0x6;
                                        convertedByte[3] = 0x0;
                                        convertedByte[3] = 0x0;
                                    }
                                }
                                else if (actualPixel == 0x7)
                                {
                                    convertedByte[1] = 0x7;
                                    convertedByte[1] = 0x7;
                                    if (lastPixel % 2 == 0)
                                    {
                                        if (lastPixel % 4 == 0)
                                        {
                                            if (lastPixel % 8 == 0)
                                            {
                                                convertedByte[0] = 0x0;
                                                convertedByte[0] = 0x0;
                                            }
                                            else
                                            {
                                                convertedByte[0] = 0x4;
                                                convertedByte[0] = 0x4;
                                            }
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0x6;
                                            convertedByte[0] = 0x6;
                                        }
                                    }
                                    else
                                    {
                                        convertedByte[0] = 0x7;
                                        convertedByte[0] = 0x7;
                                    }
                                    if (nextPixel >= 0x8)
                                    {
                                        convertedByte[2] = 0xf;
                                        convertedByte[2] = 0xf;
                                        convertedByte[3] = 0xf;
                                        convertedByte[3] = 0xf;
                                    }
                                    else
                                    {
                                        convertedByte[2] = 0x7;
                                        convertedByte[2] = 0x7;
                                        convertedByte[3] = 0x7;
                                        convertedByte[3] = 0x7;
                                    }
                                }
                                else if (actualPixel == 0x8)
                                {
                                    if (lastPixel % 2 == 0)
                                    {
                                        if (lastPixel % 4 == 0)
                                        {
                                            convertedByte[0] = 0x8;
                                            convertedByte[0] = 0x8;
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0xa;
                                            convertedByte[0] = 0xa;
                                        }
                                    }
                                    else
                                    {
                                        if (lastPixel == 0x1 || lastPixel == 0x5 || lastPixel == 0x9 || lastPixel == 0xd)
                                        {
                                            convertedByte[0] = 0x9;
                                            convertedByte[0] = 0x9;
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0xb;
                                            convertedByte[0] = 0xb;
                                        }
                                    }
                                    if (nextPixel >= 0x8)
                                    {
                                        convertedByte[1] = 0x8;
                                        convertedByte[1] = 0x8;
                                        convertedByte[2] = 0x8;
                                        convertedByte[2] = 0x8;
                                        convertedByte[3] = 0x8;
                                        convertedByte[3] = 0x8;
                                    }
                                    else
                                    {
                                        convertedByte[1] = 0x0;
                                        convertedByte[1] = 0x0;
                                        convertedByte[2] = 0x0;
                                        convertedByte[2] = 0x0;
                                        convertedByte[3] = 0x0;
                                        convertedByte[3] = 0x0;
                                    }
                                }
                                else if (actualPixel == 0x9)
                                {
                                    convertedByte[1] = 0x9;
                                    convertedByte[1] = 0x9;
                                    convertedByte[2] = 0x9;
                                    convertedByte[2] = 0x9;
                                    if (lastPixel % 2 == 0)
                                    {
                                        if (lastPixel % 4 == 0)
                                        {
                                            convertedByte[0] = 0x8;
                                            convertedByte[0] = 0x8;
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0xa;
                                            convertedByte[0] = 0xa;
                                        }
                                    }
                                    else
                                    {
                                        if (lastPixel == 0x1 || lastPixel == 0x5 || lastPixel == 0x9 || lastPixel == 0xd)
                                        {
                                            convertedByte[0] = 0x9;
                                            convertedByte[0] = 0x9;
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0xb;
                                            convertedByte[0] = 0xb;
                                        }
                                    }
                                    if (nextPixel >= 0xc)
                                    {
                                        convertedByte[3] = 0xd;
                                        convertedByte[3] = 0xd;
                                    }
                                    else if (nextPixel >= 0x8)
                                    {
                                        convertedByte[3] = 0x9;
                                        convertedByte[3] = 0x9;
                                    }
                                    else if (nextPixel >= 0x4)
                                    {
                                        convertedByte[3] = 0x5;
                                        convertedByte[3] = 0x5;
                                    }
                                    else
                                        convertedByte[3] = 0x1;
                                }
                                else if (actualPixel == 0xa)
                                {
                                    convertedByte[2] = 0xa;
                                    if (lastPixel % 2 == 0)
                                    {
                                        convertedByte[0] = 0xa;
                                        convertedByte[1] = 0xa;
                                    }
                                    else
                                    {
                                        convertedByte[0] = 0xb;
                                        convertedByte[1] = 0xb;
                                    }
                                    if (nextPixel >= 0xc)
                                    {
                                        convertedByte[3] = 0xe;
                                        convertedByte[3] = 0xe;
                                    }
                                    else if (nextPixel >= 0x8)
                                    {
                                        convertedByte[3] = 0xa;
                                        convertedByte[3] = 0xa;
                                    }
                                    else if (nextPixel >= 0x4)
                                    {
                                        convertedByte[3] = 0x6;
                                        convertedByte[3] = 0x6;
                                    }
                                    else if (nextPixel >= 0x2)
                                    {
                                        convertedByte[3] = 0x2;
                                        convertedByte[3] = 0x2;
                                    }
                                    else
                                    {
                                        convertedByte[3] = 0x0;
                                        convertedByte[3] = 0x0;
                                    }
                                }
                                else if (actualPixel == 0xb)
                                {
                                    convertedByte[1] = 0xb;
                                    convertedByte[1] = 0xb;
                                    convertedByte[2] = 0xb;
                                    convertedByte[2] = 0xb;
                                    if (lastPixel % 2 == 0)
                                    {
                                        convertedByte[0] = 0xa;
                                        convertedByte[0] = 0xa;
                                    }
                                    else
                                    {
                                        convertedByte[0] = 0xb;
                                        convertedByte[0] = 0xb;
                                    }
                                    if (nextPixel >= 0xc)
                                    {
                                        convertedByte[3] = 0xf;
                                        convertedByte[3] = 0xf;
                                    }
                                    else if (nextPixel >= 0x8)
                                    {
                                        convertedByte[3] = 0xb;
                                        convertedByte[3] = 0xb;
                                    }
                                    else if (nextPixel >= 0x4)
                                    {
                                        convertedByte[3] = 0x7;
                                        convertedByte[3] = 0x7;
                                    }
                                    else
                                    {
                                        convertedByte[3] = 0x3;
                                        convertedByte[3] = 0x3;
                                    }
                                        
                                }
                                else if (actualPixel == 0xc)
                                {
                                    if (lastPixel % 2 == 0)
                                    {
                                        if (lastPixel % 4 == 0)
                                        {
                                            convertedByte[0] = 0xc;
                                            convertedByte[0] = 0xc;
                                            convertedByte[1] = 0xc;
                                            convertedByte[1] = 0xc;
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0xe;
                                            convertedByte[0] = 0xe;
                                            convertedByte[1] = 0xc;
                                            convertedByte[1] = 0xc;
                                        }
                                    }
                                    else
                                    {
                                        if (lastPixel == 0x1 || lastPixel == 0x5 || lastPixel == 0x9 || lastPixel == 0xd)
                                        {
                                            convertedByte[0] = 0xd;
                                            convertedByte[0] = 0xd;
                                            convertedByte[1] = 0xd;
                                            convertedByte[1] = 0xd;
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0xf;
                                            convertedByte[0] = 0xf;
                                            convertedByte[1] = 0xd;
                                            convertedByte[1] = 0xd;
                                        }
                                    }
                                    if (nextPixel >= 0x8)
                                    {
                                        convertedByte[2] = 0xc;
                                        convertedByte[2] = 0xc;
                                        convertedByte[3] = 0xc;
                                        convertedByte[3] = 0xc;
                                    }
                                    else if (nextPixel >= 0x4)
                                    {
                                        convertedByte[2] = 0x4;
                                        convertedByte[2] = 0x4;
                                        convertedByte[3] = 0x4;
                                        convertedByte[3] = 0x4;
                                    }
                                    else
                                    {
                                        convertedByte[2] = 0x0;
                                        convertedByte[2] = 0x0;
                                        convertedByte[3] = 0x0;
                                        convertedByte[3] = 0x0;
                                    }
                                }
                                else if (actualPixel == 0xd)
                                {
                                    convertedByte[1] = 0xd;
                                    convertedByte[1] = 0xd;
                                    convertedByte[2] = 0xd;
                                    convertedByte[2] = 0xd;
                                    if (lastPixel % 2 == 0)
                                    {
                                        if (lastPixel % 4 == 0)
                                        {
                                            convertedByte[0] = 0xc;
                                            convertedByte[0] = 0xc;
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0xe;
                                            convertedByte[0] = 0xe;
                                        }
                                    }
                                    else
                                    {
                                        if (lastPixel == 0x1 || lastPixel == 0x5 || lastPixel == 0x9 || lastPixel == 0xd)
                                        {
                                            convertedByte[0] = 0xd;
                                            convertedByte[0] = 0xd;
                                        }
                                        else
                                        {
                                            convertedByte[0] = 0xf;
                                            convertedByte[0] = 0xf;
                                        }
                                    }
                                    if (nextPixel >= 0x8)
                                    {
                                        convertedByte[3] = 0xd;
                                        convertedByte[3] = 0xd;
                                    }
                                    else
                                    {
                                        convertedByte[3] = 0x5;
                                        convertedByte[3] = 0x5;
                                    }
                                }
                                else if (actualPixel == 0xe)
                                {
                                    convertedByte[2] = 0xe;
                                    if (lastPixel % 2 == 0)
                                    {
                                        convertedByte[0] = 0xe;
                                        convertedByte[0] = 0xe;
                                        convertedByte[1] = 0xe;
                                        convertedByte[1] = 0xe;
                                    }
                                    else
                                    {
                                        convertedByte[0] = 0xf;
                                        convertedByte[0] = 0xf;
                                        convertedByte[1] = 0xf;
                                        convertedByte[1] = 0xf;
                                    }
                                    if (nextPixel >= 0x8)
                                    {
                                        convertedByte[3] = 0xe;
                                        convertedByte[3] = 0xe;
                                    }
                                    else if (nextPixel >= 0x4)
                                    {
                                        convertedByte[3] = 0x6;
                                        convertedByte[3] = 0x6;
                                    }
                                    else if (nextPixel >= 0x2)
                                    {
                                        convertedByte[3] = 0x2;
                                        convertedByte[3] = 0x2;
                                    }
                                    else
                                    {
                                        convertedByte[3] = 0x0;
                                        convertedByte[3] = 0x0;
                                    }
                                }
                                else if (actualPixel == 0xf)
                                {
                                    convertedByte[1] = 0xf;
                                    convertedByte[1] = 0xf;
                                    convertedByte[2] = 0xf;
                                    convertedByte[2] = 0xf;
                                    if (lastPixel % 2 == 0)
                                    {
                                        convertedByte[0] = 0xe;
                                        convertedByte[0] = 0xe;
                                    }
                                    else
                                    {
                                        convertedByte[0] = 0xf;
                                        convertedByte[0] = 0xf;
                                    }
                                    if (nextPixel >= 0x8)
                                    {
                                        convertedByte[3] = 0xf;
                                        convertedByte[3] = 0xf;
                                    }
                                    else
                                    {
                                        convertedByte[3] = 0x7;
                                        convertedByte[3] = 0x7;
                                    }
                                }

                                lastPixel = actualPixel;

                                for (int k = 0; k < 4; k++)
                                {
                                    bmp[pixelId + k * 2] = convertedByte[k];
                                    bmp[pixelId + k * 2 + 1] = convertedByte[k];
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                    }

                }
            }


            Bitmap bitmap = new Bitmap(280 * pixelSize, 192 * pixelSize, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            ColorPalette pal = bitmap.Palette;
            if (mainBoard.softswitches.LoRes_HiRes)
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
            else if (mainBoard.softswitches.DHiResOn_Off)
            {
                pal.Entries[0x00] = Color.Black; // Black;
                pal.Entries[0x01] = Color.DarkMagenta; // OrangeRed;
                pal.Entries[0x02] = Color.SaddleBrown; // MediumSpringGreen;
                pal.Entries[0x03] = Color.OrangeRed; // MediumOrchid;
                pal.Entries[0x04] = Color.DarkGreen; // Green;
                pal.Entries[0x05] = Color.DarkGray; // DimGray;
                pal.Entries[0x06] = Color.Green; // LimeGreen;
                pal.Entries[0x07] = Color.Yellow; // MediumPurple;
                pal.Entries[0x08] = Color.DarkBlue; // SaddleBrown;
                pal.Entries[0x09] = Color.Purple; // Purple;
                pal.Entries[0x0a] = Color.Gray; // LightSlateGray;
                pal.Entries[0x0b] = Color.HotPink; // HotPink;
                pal.Entries[0x0c] = Color.MediumBlue; // MediumBlue;
                pal.Entries[0x0d] = Color.LightBlue; // Gold;
                pal.Entries[0x0e] = Color.Aqua; // DeepSkyBlue;
                pal.Entries[0x0f] = Color.White; // White;
                pal.Entries[0xff] = Color.White; // White;
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
            Marshal.Copy(bmp, 0, pNative, 280 * pixelSize * 192 * pixelSize);
            bitmap.UnlockBits(bmData);
            mainBoard.softswitches.Vertical_blankingOn_Off = true;
            return bitmap;

        }

        
    }
}