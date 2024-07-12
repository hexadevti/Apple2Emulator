using Apple2.Mainboard;

namespace Apple2.CPU
{
   internal static class LoadOpCodeProcessors
   {
      public static void Process_LDY(State processorState, Apple2Board mainBoard, ushort address)
      {
         var b = mainBoard.ReadByte(address);
         processorState.Y = b;
         processorState.Z = b == 0;
         processorState.N = RegisterFunctions.IsNegative(b);
      }

      public static void Process_LDX(State processorState, Apple2Board mainBoard, ushort address)
      {
         var b = mainBoard.ReadByte(address);
         processorState.X = b;
         processorState.Z = b == 0;
         processorState.N = RegisterFunctions.IsNegative(b);
      }

      public static void Process_LDA(State processorState, Apple2Board mainBoard, ushort address)
      {
         var b = mainBoard.ReadByte(address);
         processorState.A = b;
         processorState.Z = b == 0;
         processorState.N = RegisterFunctions.IsNegative(b);
      }
   }
}
