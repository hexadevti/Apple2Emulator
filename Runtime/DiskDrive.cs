using System.Collections;

namespace Runtime;

public class DiskDrive
{
    private Memory memory { get; set; }



    public byte[] diskImage { get; set; }

    public int offset { get; set; }
    public int catalog_track { get; set; }
    public int catalog_sector { get; set; }
    public byte[] disk_info { get; set; }

    public int offset_to_disk_info { get; set; }

    public DiskDrive(string dskPath, Memory memory)
    {
        this.memory = memory;
        this.disk_info = new byte[] {
                0,       // unused
                17, 15,  // track/sector
                3,       // release number
                0, 0,    // unused
                254,     // volume number
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0, 0,
                122,     // max number of track/sector pairs
                0, 0, 0, 0, 0, 0, 0, 0,
                18,      // last track sectors were allocated
                1,       // direction of track allocation
                0, 0,
                35, 16,  // max tracks, max sectors per track
        };

        this.diskImage = File.ReadAllBytes(dskPath);
        offset_to_disk_info = GetOffset(17, 0);
        offset = offset_to_disk_info;

        for (int i = 0; i < disk_info.Length; i++)
        {
            diskImage[offset + i] = disk_info[i];
        }


        for (int i = 0x38; i < 0xc4; i += 4)
        {
            diskImage[offset + i + 0] = 0xff;
            diskImage[offset + i + 1] = 0xff;

        }

        MarkSectorUsed(17, 0);

        for (int sector = 15; sector >= 1; sector--)
        {
            offset = GetOffset(17, sector);

            if (sector != 1)
            {
                diskImage[offset + 1] = 17;
                diskImage[offset + 2] = (byte)(sector - 1);
            }

            MarkSectorUsed(17, sector);
        }

        catalog_track = diskImage[offset_to_disk_info + 1];
        catalog_sector = diskImage[offset_to_disk_info + 2];


    }

    public void MarkSectorUsed(int track, int sector)
    {
        offset = offset_to_disk_info;

        var byte_pair_offset = offset + 0x38 + (track * 4);
        var current = (diskImage[byte_pair_offset] << 8) |
              diskImage[byte_pair_offset + 1];

        current &= 0xffff ^ (1 << (sector));

        diskImage[byte_pair_offset + 0] = (byte)(current >> 8);
        diskImage[byte_pair_offset + 1] = (byte)(current & 0xff);
    }


    public string DiskInfo()
    {
        string info = "";
        var totalTracks = diskImage[offset + 0x34];
        info += "================ Disk Info ===================";
        info += "       Catalog Track: " + diskImage[offset + 0x01] + "\r\n";
        info += "      Catalog Sector: " + diskImage[offset + 0x02] + "\r\n";
        info += "      Release Number: " + diskImage[offset + 0x03] + "\r\n";
        info += "       Volume Number: " + diskImage[offset + 0x06] + "\r\n";
        info += "   Max Track/Sectors: " + diskImage[offset + 0x27] + "\r\n";
        info += "     Last Used Track: " + diskImage[offset + 0x30] + "\r\n";
        info += "Track Alloc Dirction: " + diskImage[offset + 0x31] + "\r\n";
        info += "     Tracks Per Disk: " + diskImage[offset + 0x34] + "\r\n";
        info += "    Sectors Per Disk: " + diskImage[offset + 0x35] + "\r\n";
        info += "    Bytes Per Sector: " + GetInt16(diskImage, offset + 0x36) + "\r\n";

        for (int i = 0x38; i <= 0xff; i += 4)
        {
            if ((i - 0x38) / 4 >= totalTracks)
            { break; }
            info += string.Format("         Track {0}: [{1:X2} {2:X2} {3:X2} {4:X2}]\r\n",
                (i - 0x36) / 4,
                diskImage[offset + i + 0],
                diskImage[offset + i + 1],
                diskImage[offset + i + 2],
                diskImage[offset + i + 3]);
        }


        return info;

    }

    public string PrintCatalog()
    {
        string info = "";
        var track = this.catalog_track;
        var sector = this.catalog_sector;

        info += "================ Catalog ===================\r\n";

        while (track != 0)
        {
            info += String.Format("Track: {0}  Sector: {1}\r\n", track, sector);

            offset = GetOffset(track, sector);

            // For each possible catalog entry.
            for (int i = 0x0b; i < 256; i += 0x23)
            {
                info += String.Format("{0:X2}: ", i);

                // Print file name.
                for (int n = 0x03; n <= 0x20; n++)
                {
                    var ch = (int)(diskImage[offset + i + n]);
                    ch &= 0x7f;
                    if (ch >= 32 && ch < 127)
                    {
                        info += System.Convert.ToChar(ch);
                    }
                    else
                    {
                        info += " ";
                    }
                }

                // File type and flags.
                var file_type = diskImage[offset + i + 2];
                string type_string;
                var locked = false;

                type_string = " ";

                if ((file_type & 0x80) != 0)
                {
                    file_type &= 0x7f;
                    locked = true;
                }

                if (file_type == 0x00)
                {
                    type_string += "TXT ";
                }
                else
                {
                    if ((file_type & 0x01) != 0) { type_string += "IBAS "; }
                    if ((file_type & 0x02) != 0) { type_string += "ABAS "; }
                    if ((file_type & 0x04) != 0) { type_string += "BIN "; }
                    if ((file_type & 0x08) != 0) { type_string += "SEQ "; }
                    if ((file_type & 0x10) != 0) { type_string += "REL "; }
                    if ((file_type & 0x20) != 0) { type_string += "A "; }
                    if ((file_type & 0x40) != 0) { type_string += "B "; }
                }

                if (locked) { type_string += "LKD "; }

                // Track and sector where the file descriptor is.
                info += String.Format("{0:X2}/{1:X2}", diskImage[offset + i + 0],
                                  diskImage[offset + i + 1]);
                info += type_string;

                if (diskImage[offset + i + 0] == 0xff) { info += " DEL"; }

                // Count of sectors used for this file.
                var sectors = GetInt16(diskImage, offset + i + 0x21);

                info += String.Format("sectors: {0}", sectors);

                info += "\r\n";
            }

            // Next sector in the linked list of Catalog entries.
            track = diskImage[offset + 1];
            sector = diskImage[offset + 2];

        }

        return info;
    }

    public string DumpSector(int track, int sector)
    {
        string info = "";
        var text = new byte[16];

        var offset = GetOffset(track, sector);

        info += String.Format("===== Track: {0}   Sector: {1}   Offset: {2} =====\n", track, sector, offset);

        for (int i = 0; i < 256; i++)
        {
            if ((i % 16) == 0)
            {
                info += String.Format("{0:X2}: ", i);
            }

            info += String.Format(" {0:X2}", diskImage[offset + i]);

            var ch = (int)diskImage[offset + i];
            ch &= 0x7f;

            if (ch >= 32 && ch < 127)
            {
                text[i % 16] = (byte)ch;
            }
            else
            {
                text[i % 16] = (byte)'.';
            }

            if ((i % 16) == 15)
            {
                info += String.Format("  ");
              for (int n = 0; n < 16; n++)
                {
                    info += System.Convert.ToChar(text[n]);
                }
                info += "\r\n";
            }
        }

        info += "\r\n";

        return info;
    }

    public byte[] GetSectorData(int track, int sector)
    {
        var offset = GetOffset(track, sector);
        byte[] output = new byte[256];

        for (int i = 0; i < 256; i++)
        {
            output[i] = diskImage[offset + i];
        }
        return output;
    }

    public byte[] EncodeByte(byte data)
    {
        byte[] output = new byte[2];
        bool[] bitsEncoded = new bool[16];
        BitArray bitsData = new BitArray(new byte[] { data });

        for (int i = 0;i < 16; i++)
        {
            if (i % 2 == 0)
                bitsEncoded[i] = true;
            else
            {
                if (i > 8)
                    bitsEncoded[i] = bitsData[8-(i-7)];
                else
                    bitsEncoded[i] = bitsData[8-i];
            }
        }

        for (int i = 0;i<8;i++)
        {
            output[0] = (byte)(output[0] + (bitsEncoded[i] ? Math.Pow(2,7-i) : 0));
            output[1] = (byte)(output[1] + (bitsEncoded[i+8] ? Math.Pow(2,7-i) : 0));
        }
        return output;
    }

    public byte[] Checksum(byte volume, byte sector, byte track)
    {
        byte[] output = new byte[2];
        bool[] checkedBits = new bool[16];
        bool[] checkedBitsInverted = new bool[16];
        BitArray bitsVolume = new BitArray(EncodeByte(volume));
        BitArray bitsSector = new BitArray(EncodeByte(sector));
        BitArray bitsTrack = new BitArray(EncodeByte(track));

        for (int i = 0;i < 16; i++)
        {
            int sumBits = (bitsVolume[i] ? 1 : 0) + (bitsSector[i] ? 1 : 0) + (bitsTrack[i] ? 1 : 0);
            switch (sumBits)
            {
                case 0:
                    checkedBits[i] = false;
                    break;
                case 1:
                    checkedBits[i] = true;
                    break;
                case 2:
                    checkedBits[i] = false;
                    break;
                case 3:
                    checkedBits[i] = true;
                    break;
            }
        }
        for (int i = 0;i<16;i++)
        {
            if (i < 8)
                checkedBitsInverted[i] = checkedBits[7-i];
            else
                checkedBitsInverted[i] = checkedBits[23-i];
        }
        for (int i = 0;i<8;i++)
        {
            output[0] = (byte)(output[0] + (checkedBitsInverted[i] ? Math.Pow(2,7-i) : 0));
            output[1] = (byte)(output[1] + (checkedBitsInverted[i+8] ? Math.Pow(2,7-i) : 0));
        }
        return output;
    }


    public int GetOffset(int track, int sector)
    {
        return (sector * 256) + (track * (256 * 16));
    }

    public int GetInt16(byte[] data, int offset)
    {
        return (data[offset]) | ((data[offset + 1]) << 8);
    }
}
