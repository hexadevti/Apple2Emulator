using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apple2Sharp.Mainboard;
using Apple2Sharp.Mainboard.Enums;
using Apple2Sharp.Mainboard.Interfaces;

namespace Apple2Sharp
{
    public class Keyboard
    {
        public Apple2Board mainBoard { get; set; }

        private IProcessor cpu { get; set; }

        private List<char> buffer = new List<char>();

        private Dictionary<Keys, JoysticPosition> downKeys = new Dictionary<Keys, JoysticPosition>();

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
                            switch (e.KeyCode)
                            {
                                case Keys.Left:
                                    AddDownKey(e.KeyCode, JoysticPosition.Min);
                                    mainBoard.timerpdl0 = (int)JoysticPosition.Min;
                                    break;
                                case Keys.Right:
                                    AddDownKey(e.KeyCode, JoysticPosition.Max);
                                    mainBoard.timerpdl0 = (int)JoysticPosition.Max;
                                    break;
                                case Keys.Up:
                                    AddDownKey(e.KeyCode, JoysticPosition.Min);
                                    mainBoard.timerpdl1 = (int)JoysticPosition.Min;
                                    break;
                                case Keys.Down:
                                    AddDownKey(e.KeyCode, JoysticPosition.Max);
                                    mainBoard.timerpdl1 = (int)JoysticPosition.Max;
                                    break;
                            }

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
                        {
                            mainBoard.softswitches.Pb0 = true;
                            switch (e.KeyCode)
                            {
                                case Keys.Left:
                                    AddDownKey(e.KeyCode, JoysticPosition.Min);
                                    mainBoard.timerpdl0 = (int)JoysticPosition.Min;
                                    break;
                                case Keys.Right:
                                    AddDownKey(e.KeyCode, JoysticPosition.Max);
                                    mainBoard.timerpdl0 = (int)JoysticPosition.Max;
                                    break;
                                case Keys.Up:
                                    AddDownKey(e.KeyCode, JoysticPosition.Min);
                                    mainBoard.timerpdl1 = (int)JoysticPosition.Min;
                                    break;
                                case Keys.Down:
                                    AddDownKey(e.KeyCode, JoysticPosition.Max);
                                    mainBoard.timerpdl1 = (int)JoysticPosition.Max;
                                    break;
                                case Keys.ShiftKey:
                                    mainBoard.softswitches.Pb1 = true;
                                    break;
                            }

                        }
                        break;
                }
            }
            else if (e.KeyCode == Keys.ShiftKey)
            {
                switch (e.KeyCode)
                {
                    case Keys.ShiftKey:
                        mainBoard.softswitches.Pb1 = true;
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
                                    AddDownKey(e.KeyCode, JoysticPosition.Min);
                                    mainBoard.timerpdl0 = (int)JoysticPosition.Min;
                                    mainBoard.KeyPressedBuffer = 0x88;
                                    break;
                                case Keys.Right:
                                    AddDownKey(e.KeyCode, JoysticPosition.Max);
                                    mainBoard.timerpdl0 = (int)JoysticPosition.Max;
                                    mainBoard.KeyPressedBuffer = 0x95;
                                    break;
                                case Keys.Up:
                                    AddDownKey(e.KeyCode, JoysticPosition.Min);
                                    mainBoard.timerpdl1 = (int)JoysticPosition.Min;
                                    mainBoard.KeyPressedBuffer = 0x8b;
                                    break;
                                case Keys.Down:
                                    AddDownKey(e.KeyCode, JoysticPosition.Max);
                                    mainBoard.timerpdl1 = (int)JoysticPosition.Max;
                                    mainBoard.KeyPressedBuffer = 0x8a;
                                    break;
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
                        mainBoard.timerpdl0 = (int)KeyLeftDown(e.KeyCode,JoysticPosition.Middle).Value;
                        break;
                    case Keys.Right:
                        mainBoard.timerpdl0 = (int)KeyLeftDown(e.KeyCode,JoysticPosition.Middle).Value;
                        break;
                    case Keys.Up:
                        mainBoard.timerpdl1 = (int)KeyLeftDown(e.KeyCode,JoysticPosition.Middle).Value;
                        break;
                    case Keys.Down:
                        mainBoard.timerpdl1 = (int)KeyLeftDown(e.KeyCode,JoysticPosition.Middle).Value;
                        break;
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
                            Thread.Sleep(200);
                        }
                        else
                            mainBoard.KeyPressedBuffer = (byte)(Encoding.ASCII.GetBytes(new[] { item })[0] | 0b10000000);
                        Thread.Sleep(1);
                    }
                });
                txb.Text = "";
            }

        }

        private void AddDownKey(Keys keys, JoysticPosition joysticPosition)
        {
            if (!downKeys.ContainsKey(keys))
            {
                downKeys.Add(keys,joysticPosition);
            }
        }

        private KeyValuePair<Keys, JoysticPosition> KeyLeftDown(Keys keys, JoysticPosition joysticPosition)
        {
            if (downKeys.ContainsKey(keys))
            {
                downKeys.Remove(keys);
                var leftKey = downKeys.Any() && downKeys.Count == 1 ? downKeys.First() : new KeyValuePair<Keys, JoysticPosition>(keys, joysticPosition);
                if (((keys == Keys.Left || keys == Keys.Right) && (leftKey.Key == Keys.Up || leftKey.Key == Keys.Down))
                || ((keys == Keys.Up || keys == Keys.Down) && (leftKey.Key == Keys.Left || leftKey.Key == Keys.Right)))
                {
                    return new KeyValuePair<Keys,JoysticPosition>(keys, joysticPosition);        
                }
                else
                {
                    return leftKey;
                }
                
            }
            return new KeyValuePair<Keys,JoysticPosition>(keys, joysticPosition);
        }
    }
}