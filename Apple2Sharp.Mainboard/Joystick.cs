using Apple2Sharp.Mainboard;

namespace Apple2Sharp.Mainboard
{
    public static class Joystick
    {
        static int joystickCycles0 = 0;
        static int joystickCycles1 = 0;
        static int joystickCycles2 = 0;
        static int joystickCycles3 = 0;

        public static void Process(Apple2Board _mainBoard, float speedAdjust = 1)
        {
            if (_mainBoard.joystick)
            {
                if (_mainBoard.softswitches.CgReset0)
                {
                    joystickCycles0++;
                }
                if (_mainBoard.softswitches.CgReset1)
                {
                    joystickCycles1++;
                }
                if (_mainBoard.softswitches.CgReset2)
                {
                    joystickCycles2++;
                }
                if (_mainBoard.softswitches.CgReset3)
                {
                    joystickCycles3++;
                }

                if (joystickCycles0 >= (int)(_mainBoard.timerpdl0 * speedAdjust))
                {
                    joystickCycles0 = 0;
                    _mainBoard.softswitches.Cg0 = false;
                    _mainBoard.softswitches.CgReset0 = false;
                }

                if (joystickCycles1 >= (int)(_mainBoard.timerpdl1 * speedAdjust))
                {
                    joystickCycles1 = 0;
                    _mainBoard.softswitches.Cg1 = false;
                    _mainBoard.softswitches.CgReset1 = false;
                }

                if (joystickCycles2 >= (int)(_mainBoard.timerpdl2 * speedAdjust))
                {
                    joystickCycles2 = 0;
                    _mainBoard.softswitches.Cg2 = false;
                    _mainBoard.softswitches.CgReset2 = false;
                }

                if (joystickCycles3 >= (int)(_mainBoard.timerpdl3 * speedAdjust))
                {
                    joystickCycles3 = 0;
                    _mainBoard.softswitches.Cg3 = false;
                    _mainBoard.softswitches.CgReset3 = false;
                }
            }
        }
    }
}