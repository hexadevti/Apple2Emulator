using System.Runtime.CompilerServices;

namespace Runtime.Abstractions
{

    public interface ICard
    {
        int SlotNumber { get; set; }
        void Write(ushort address, byte b, MainBoard mainBoard);
        byte Read(ushort address, MainBoard mainBoard, State state);

        byte[] C000ROM { get; }

        byte[] CC00ROM { get; set; }
    }
}