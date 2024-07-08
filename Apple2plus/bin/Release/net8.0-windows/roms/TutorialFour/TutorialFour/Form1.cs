using System;
using System.Windows.Forms;

using NAudio.Wave;

namespace TutorialFour
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private DirectSoundOut output = null;
        private BlockAlignReductionStream stream = null;

        private void button1_Click(object sender, EventArgs e)
        {
            WaveTone tone = new WaveTone(3000, 0.1);
            stream = new BlockAlignReductionStream(tone);

            output = new DirectSoundOut();
            output.Init(stream);
            output.Play();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (output != null) output.Stop();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (output != null)
            {
                output.Dispose();
                output = null;
            }
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
        }
    }

    public class WaveTone : WaveStream
    {
        private double frequency;
        private double amplitude;
        private double time;

        public WaveTone(double f, double a)
        {
            this.time = 0;
            this.frequency = f;
            this.amplitude = a;
        }

        public override long Position
        {
            get;
            set;
        }

        public override long Length
        {
            get { return long.MaxValue; }
        }

        public override WaveFormat WaveFormat
        {
            get { return new WaveFormat(44100, 16, 1); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int samples = count / 2;
            for (int i = 0; i < samples; i++)
            {
                double sine = amplitude * Math.Sin(Math.PI * 2 * frequency * time);
                time += 1.0 / 44100;
                short truncated = (short)Math.Round(sine * (Math.Pow(2, 15) - 1));
                buffer[i * 2] = (byte)(truncated & 0x00ff);
                buffer[i * 2 + 1] = (byte)((truncated & 0xff00) >> 8);
            }

            return count;
        }
    }
}
