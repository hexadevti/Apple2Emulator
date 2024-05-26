namespace Apple2;

public static class SoundGenerator
{
    public static Stream? Generate(Runtime.Memory memory)
    {
        if (memory.softswitches.SoundClick)
        {
            memory.softswitches.SoundClick = false;
            return new MemoryStream(new byte[] {12,32,43,74,23,53,24,54,234,253,153});
        }
        return null;
    }
}
