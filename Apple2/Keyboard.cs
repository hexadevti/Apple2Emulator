using System.Text;
using Runtime;

namespace Apple2;

public class Keyboard
{
    public Runtime.Memory memory {get; set;}
    public State state {get; set;}

    public Object lockObj {get; set;}

    public Keyboard(Runtime.Memory memory, State state, Object lockObj)
    {
        this.memory = memory;
        this.state = state;
        this.lockObj = lockObj;
    }

    public void OnKeyPress(object? sender, KeyPressEventArgs e)
    {
        if (e.KeyChar < 32 || e.KeyChar >= 127)
            return;
         memory.KeyPressed = (byte)(Encoding.ASCII.GetBytes( new[] { e.KeyChar.ToString().ToUpper()[0]})[0] | 0b10000000);
    }

    public void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Control)
        {
            switch (e.KeyCode)
            {
                case Keys.C:
                    memory.KeyPressed = 0x83;
                    break;
                case Keys.B:
                    memory.KeyPressed = 0x82;
                    break;
                case Keys.F12:
                    lock (lockObj)
                    {
                        Thread.Sleep(100);
                        state.PC = 0;
                    }
                
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
                    break;
                case Keys.Enter:
                    memory.KeyPressed = 0x8d;
                    break;
            }
        }
       

    }

}
