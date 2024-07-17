using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apple2.Mainboard;
using Apple2.Mainboard.Interfaces;

namespace Apple2
{
    public class Keyboard
    {
        public Apple2Board mainBoard { get; set; }

        private IProcessor cpu { get; set; }

        private List<char> buffer = new List<char>();

        public Keyboard(Apple2Board mainBoard, IProcessor cpu)
        {
            this.mainBoard = mainBoard;
            this.cpu = cpu;
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
                    default:
                        if (mainBoard.joystick)
                        {
                            mainBoard.softswitches.Pb0 = true;
                            mainBoard.softswitches.Pb1 = true;
                        }
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
                        else if (mainBoard.joystick)
                            mainBoard.softswitches.Pb0 = true;
                        break;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Back:
                        mainBoard.KeyPressedBuffer = 0x88;
                        break;
                    case Keys.Escape:
                        mainBoard.KeyPressedBuffer = 0x9b;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.Enter:
                        mainBoard.KeyPressedBuffer = 0x8d;
                        e.SuppressKeyPress = true;
                        break;
                    default:
                        if (mainBoard.joystick)
                        {
                            switch (e.KeyCode)
                            {
                                case Keys.Left:
                                    mainBoard.timerpdl0 = 10;
                                    mainBoard.KeyPressedBuffer = 0x88;
                                    break;
                                case Keys.Right:
                                    mainBoard.timerpdl0 = 3580;
                                    mainBoard.KeyPressedBuffer = 0x95;
                                    break;
                                case Keys.Up:
                                    mainBoard.timerpdl1 = 10;
                                    mainBoard.KeyPressedBuffer = 0x8b;
                                    break;
                                case Keys.Down:
                                    mainBoard.timerpdl1 = 3580;
                                    mainBoard.KeyPressedBuffer = 0x8a;
                                    break;
                                // case Keys.A:
                                //     mainBoard.timerpdl0 = 10;
                                //     break;
                                // case Keys.D:
                                //     mainBoard.timerpdl0 = 3580;
                                //     break;
                                // case Keys.W:
                                //     mainBoard.timerpdl1 = 10;
                                //     break;
                                // case Keys.S:
                                //     mainBoard.timerpdl1 = 3580;
                                //     break;
                                case Keys.ControlKey:
                                    mainBoard.softswitches.Pb0 = true;
                                    break;
                                case Keys.ShiftKey:
                                    mainBoard.softswitches.Pb1 = true;
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
                                case Keys.Right:
                                    mainBoard.KeyPressedBuffer = 0x95;
                                    break;
                                case Keys.Up:
                                    mainBoard.KeyPressedBuffer = 0x8b;
                                    break;
                                case Keys.Down:
                                    mainBoard.KeyPressedBuffer = 0x8a;
                                    break;
                            }
                        }
                        break;
                }
            }

            e.Handled = true;
        }

        public void OnKeyUp(object? sender, KeyEventArgs e)
        {
            if (mainBoard.joystick)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        mainBoard.timerpdl0 = 1790;
                        break;
                    case Keys.Right:
                        mainBoard.timerpdl0 = 1790;
                        break;
                    case Keys.Up:
                        mainBoard.timerpdl1 = 1790;
                        break;
                    case Keys.Down:
                        mainBoard.timerpdl1 = 1790;
                        break;
                    // case Keys.A:
                    //     mainBoard.timerpdl0 = 1790;
                    //     break;
                    // case Keys.D:
                    //     mainBoard.timerpdl0 = 1790;
                    //     break;
                    // case Keys.W:
                    //     mainBoard.timerpdl1 = 1790;
                    //     break;
                    // case Keys.S:
                    //     mainBoard.timerpdl1 = 1790;
                    //     break;
                    case Keys.ControlKey:
                        mainBoard.softswitches.Pb0 = false;
                        break;
                    case Keys.ShiftKey:
                        mainBoard.softswitches.Pb1 = false;
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
                            mainBoard.KeyPressedBuffer = (byte)(Encoding.ASCII.GetBytes(new[] { item })[0] | 0b10000000);
                        Thread.Sleep(1);
                    }
                });
                txb.Text = "";
            }

        }

    }
}