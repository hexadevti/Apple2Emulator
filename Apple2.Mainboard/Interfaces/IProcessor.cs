using Apple2.Mainboard.Enums;

namespace Apple2.Mainboard.Interfaces;

public interface IProcessor
{
    void IncrementPC();

    void Run();

    void RunCycle();

    void Reset();

    void WarmStart();

    CpuState cpuState { get; set; }
}
