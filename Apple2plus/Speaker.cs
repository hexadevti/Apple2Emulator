using Runtime;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Apple2;

public class Speaker : WaveStream
{
    private MainBoard _mainBoard;
    private WaveFormat _waveFormat;

    public double frequency { get; set; }
    public byte sample;
    byte actualSample = 0;


    public Speaker(MainBoard mainBoard, WaveFormat waveFormat)
    {
        _mainBoard = mainBoard;
        _waveFormat = waveFormat;
    }
    public override WaveFormat WaveFormat
    {
        get
        {
            return _waveFormat;
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
        
        if (_mainBoard.clickBuffer.TryDequeue(out bytes))
        {
            for (int i = 0; i < count; i++)
            {
                buffer[i] = bytes[i];
            }
        }
        
        return count;
    }
}