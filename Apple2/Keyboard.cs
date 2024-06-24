using System;
using System.Text;
using System.Windows.Forms;
using Runtime;

namespace Apple2;

public class Keyboard
{
    public Runtime.Memory memory { get; set; }

    private CPU cpu { get; set;}

    private List<char> buffer = new List<char>();

    public Keyboard(Memory memory, CPU cpu)
    {
        this.memory = memory;
        this.cpu = cpu;
    }

    public void OnKeyPress(object? sender, KeyPressEventArgs e)
    {
        if (e.KeyChar < 32 || e.KeyChar >= 127)
            return;
        memory.KeyPressed = (byte)(Encoding.ASCII.GetBytes(new[] { e.KeyChar.ToString().ToUpper()[0] })[0] | 0b10000000);
        e.Handled = true;
    }

    public void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Control)
        {
            switch (e.KeyCode)
            {
                case Keys.F12:
                    e.Handled = true;
                    cpu.Reset();
                    break;
                case Keys.V:
                    return;
                default:
                    if (e.KeyValue > 0x40 && e.KeyValue < 0x60)
                        memory.KeyPressed = (byte)(e.KeyValue + 0x40);
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
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Enter:
                    memory.KeyPressed = 0x8d;
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
                        memory.KeyPressed = 0x8d;
                        Thread.Sleep(100);
                    }
                    else
                        memory.KeyPressed = (byte)(Encoding.ASCII.GetBytes(new[] { item.ToString().ToUpper()[0] })[0] | 0b10000000);
                    Thread.Sleep(1);
                }
            });
            txb.Text = "";
        }
        
    }

}
