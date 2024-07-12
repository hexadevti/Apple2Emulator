using Apple2.Mainboard;

namespace Apple2.Mainboard.Abstractions
{

    public interface ICard
    {
        int SlotNumber { get; set; }
        byte[] C000ROM { get; }
        byte[] CC00ROM { get; set; }

        void Write(ushort address, byte b, Apple2Board mainBoard);
        byte Read(ushort address, Apple2Board mainBoard);
    }
}