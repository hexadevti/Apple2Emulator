using Apple2Sharp.CPU65C02;

namespace Apple2Sharp.CPU65C02
{
    internal static class FlagOpCodeProcessors
    {

        public static void Process_CLC(State processorState)
        {
            processorState.C = false;
        }
        public static void Process_SEI(State processorState)
        {
            processorState.I = true;
        }
        public static void Process_CLV(State processorState)
        {
            processorState.V = false;
        }

        public static void Process_CLI(State processorState)
        {
            processorState.I = false;
        }

        public static void Process_SEC(State processorState)
        {
            processorState.C = true;
        }

        public static void Process_SED(State processorState)
        {
            processorState.D = true;
        }

        public static void Process_CLD(State processorState)
        {
            processorState.D = false;
        }
    }
}