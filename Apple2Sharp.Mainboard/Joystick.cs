using Apple2Sharp.Mainboard;

namespace Apple2Sharp.Mainboard
{
    public static class Joystick
    {
        static int joystickCycles0 = 0;
        static int joystickCycles1 = 0;
        static int joystickCycles2 = 0;
        static int joystickCycles3 = 0;

        public static void Process(Apple2Board _mainBoard)
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

            if (joystickCycles0 >= _mainBoard.timerpdl0) // 1Mhz 0 -> 10, 127 (center) -> 1790, 255 -> 3580
            {
                joystickCycles0 = 0;
                _mainBoard.softswitches.Cg0 = false;
                _mainBoard.softswitches.CgReset0 = false;
            }

            if (joystickCycles1 >= _mainBoard.timerpdl1) // for 1Mhz 0 -> 10, 127 (center) -> 1790, 255 -> 3580
            {
                joystickCycles1 = 0;
                _mainBoard.softswitches.Cg1 = false;
                _mainBoard.softswitches.CgReset1 = false;
            }

            if (joystickCycles2 >= _mainBoard.timerpdl2) // for 1Mhz 0 -> 10, 127 (center) -> 1790, 255 -> 3580
            {
                joystickCycles2 = 0;
                _mainBoard.softswitches.Cg2 = false;
                _mainBoard.softswitches.CgReset2 = false;
            }

            if (joystickCycles3 >= _mainBoard.timerpdl3) // for 1Mhz 0 -> 10, 127 (center) -> 1790, 255 -> 3580
            {
                joystickCycles3 = 0;
                _mainBoard.softswitches.Cg3 = false;
                _mainBoard.softswitches.CgReset3 = false;
            }
        }
    }
}