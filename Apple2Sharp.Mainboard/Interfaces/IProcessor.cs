using Apple2Sharp.Mainboard.Enums;

namespace Apple2Sharp.Mainboard.Interfaces;

public interface IProcessor
{
    void IncrementPC();

    void Run();

    void RunCycle();

    void Reset();

    void WarmStart();

    CpuState cpuState { get; set; }
}
