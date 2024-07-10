using System.Runtime.CompilerServices;

namespace Runtime.Abstractions
{
    public interface IRamCard
    {
        byte[,] MemoryBankSwitchedRAM1 { get; set; }
        byte[,] MemoryBankSwitchedRAM2_1 { get; set; }
        byte[,] MemoryBankSwitchedRAM2_2 { get; set; }

        bool MemoryBankBankSelect1_2 { get; set; }

        bool MemoryBankReadRAM_ROM { get; set; }

        bool MemoryBankWriteRAM_NoWrite { get; set; }

        int SelectedBank { get; set; }

    }
}
