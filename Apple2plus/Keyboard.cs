using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apple2.CPU;
using Apple2.Mainboard;

namespace Apple2
{
    public class Keyboard
    {
        public Apple2Board mainBoard { get; set; }

        private Processor cpu { get; set; }

        private List<char> buffer = new List<char>();

        public Keyboard(Apple2Board mainBoard, Processor cpu)
        {
            this.mainBoard = mainBoard;
            this.cpu = cpu;
        }

        public void OnKeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 32 || e.KeyChar >= 127)
                return;

            mainBoard.KeyPressedBuffer = (byte)(Encoding.ASCII.GetBytes(new[] { e.KeyChar.ToString().ToUpper()[0] })[0] | 0b10000000);
            e.Handled = true;
        }

        public void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.F12:
                        e.Handled = true;
                        cpu.WarmStart();
                        break;
                }
            }
            else if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.F12:
                        e.Handled = true;
                        cpu.Reset();
                        break;
                    case Keys.F5:
                        e.Handled = true;
                        break;
                    case Keys.V:
                        return;
                    default:
                        if (e.KeyValue > 0x40 && e.KeyValue < 0x60)
                            mainBoard.KeyPressedBuffer = (byte)(e.KeyValue + 0x40);
                        break;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        mainBoard.KeyPressedBuffer = 0x88;
                        break;
                    case Keys.Back:
                        mainBoard.KeyPressedBuffer = 0x88;
                        break;
                    case Keys.Right:
                        mainBoard.KeyPressedBuffer = 0x95;
                        break;
                    case Keys.Up:
                        mainBoard.KeyPressedBuffer = 0x8b;
                        break;
                    case Keys.Down:
                        mainBoard.KeyPressedBuffer = 0x8a;
                        break;
                    case Keys.Escape:
                        mainBoard.KeyPressedBuffer = 0x9b;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.Enter:
                        mainBoard.KeyPressedBuffer = 0x8d;
                        e.SuppressKeyPress = true;
                        break;
                }
            }

            e.Handled = true;
        }

        public void Keyb_TextChanged(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                RichTextBox txb = ((RichTextBox)sender);

                buffer = txb.Text.ToList();

                Task.Run(() =>
                {
                    foreach (var item in buffer)
                    {
                        if (item == '\n')
                        {
                            mainBoard.KeyPressedBuffer = 0x8d;
                            Thread.Sleep(100);
                        }
                        else
                            mainBoard.KeyPressedBuffer = (byte)(Encoding.ASCII.GetBytes(new[] { item.ToString().ToUpper()[0] })[0] | 0b10000000);
                        Thread.Sleep(1);
                    }
                });
                txb.Text = "";
            }

        }

    }
}