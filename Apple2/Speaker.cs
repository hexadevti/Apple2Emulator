using Runtime;
using NAudio.Wave;

namespace Apple2;

public class Speaker : WaveStream
{
    private Memory _memory;

    public double frequency { get; set; }
    public byte sample;
    byte actualSample = 0;

    public Speaker(Memory memory)
    {
        _memory = memory;
    }
    public override WaveFormat WaveFormat
    {
        get
        {
            return new WaveFormat(160000, 8, 1);
        }
    }

    public override long Length
    {
        get
        {
            return 1;
        }
    }

    public override long Position { get; set; }


    public override int Read(byte[] buffer, int offset, int count)
    {
        byte[] bytes = new byte[count];
        
        if (_memory.clickBuffer.TryDequeue(out bytes))
        {
            for (int i = 0; i < count; i++)
            {
                buffer[i] = bytes[i];
            }
        }
        return count;
    }
}