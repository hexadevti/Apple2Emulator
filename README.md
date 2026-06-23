# Apple2sharp

A cross-component **Apple ][+ emulator** written in C# / .NET, built around a full **MOS 6502** processor implementation.

> ⚠️ **Windows only.** The user interface is built with Windows Forms and targets `net8.0-windows`.

Made for pure fun and as a learning project — lots of challenges and concepts were explored along the way. It is based on [AndiPexton/6502](https://github.com/AndiPexton/6502) (an Apple I emulator), which was extended into a full Apple ][+ machine with several opcode fixes and many new features.

Suggestions are welcome, and any kind of contribution to the project is appreciated.

## Features

- 🖥️ **Video** — Text (40×24), Low-Res and High-Res graphics, in both color and monochrome
- 🔊 **Sound** — Speaker emulation powered by [NAudio](https://github.com/naudio/NAudio)
- ⚡ **Clock accelerator** — Run up to ~50 MHz (depending on host hardware)
- 💾 **Disk II Card** — Boot and read/write DOS 3.3 and ProDOS disks (`.dsk`, `.po`, `.do` images)
- 🧩 **Expansion cards:**
  - **Language Card** — 16 KB RAM expansion
  - **Saturn 128 KB RAM Card**
  - **80 Columns Card** (Videx Videoterm compatible)
- ⌨️ **Keyboard** — Full keyboard mapping, including arrow keys, Reset and Warm Start

Sample disk images are included in the [`disks/`](Apple2sharp/disks) folder. All required ROMs ship in the [`roms/`](Apple2sharp/roms) folder, so no extra downloads are needed.

## Solution structure

The solution is split into four projects:

| Project | Type | Description |
| --- | --- | --- |
| **Apple2sharp** | WinForms app (`net8.0-windows`) | UI, entry point, wiring, video rendering and input handling |
| **Apple2.CPU** | Class library (`net8.0`) | MOS 6502 processor: registers, flags and the full opcode set |
| **Apple2.Mainboard** | Class library (`net8.0`) | Memory map, soft switches and mainboard logic |
| **Apple2.IO** | Class library (`net8.0`) | Video, speaker, disk drive and expansion cards |

```
Apple2sharp/
├── Apple2sharp.sln
├── Apple2sharp/        # WinForms UI + Program.cs entry point
│   ├── roms/           # Applesoft BASIC, character ROM, Disk II & Videx ROMs
│   └── disks/          # Bootable disk images (DOS 3.3, ProDOS)
├── Apple2.CPU/         # 6502 core (Mos6502/, OpCodeProcessors/)
├── Apple2.Mainboard/   # Memory & soft switches
└── Apple2.IO/          # Video, Speaker, DiskDrive, Cards/
```

## Requirements

- Windows
- [.NET SDK 8.0](https://dotnet.microsoft.com/download) (also compatible with .NET 7.0 and 6.0)

## Build & Run

Clone the repository and run from the solution root:

```bash
# Build
dotnet build Apple2sharp.sln

# Run
dotnet run --project Apple2sharp/Apple2sharp.csproj
```

Alternatively, open `Apple2sharp.sln` in Visual Studio and run the **Apple2sharp** project.

## Known limitations

- Sound stability could be improved.
- Clock control increases CPU usage on the host, because C#'s `Thread.Sleep` has a minimum resolution of ~1 ms with limited precision.

## Roadmap

- Joystick support
- 65C02 CPU (Apple //e)
- Hard drive support
- Double High-Resolution graphics

## References

- **6502 Instruction Set** — https://www.masswerk.at/6502/6502_instruction_set.html
- **Videx Videoterm ROM images** — https://mirrors.apple2.org.za/Apple%20II%20Documentation%20Project/Interface%20Cards/80%20Column%20Cards/Videx%20Videoterm/ROM%20Images/
- **Virtu** (reference emulator) — https://github.com/digital-jellyfish/Virtu

### Recommended reading

- *Apple II Reference Manual*
- *DOS Programmer's Guide*
- *Beneath Apple DOS / ProDOS* — https://datassette.s3.us-west-004.backblazeb2.com/livros/beneath_apple_dos_prodos_2020.pdf
- *Videx Videoterm — Installation and Operation Manual*

## Acknowledgements

- [AndiPexton/6502](https://github.com/AndiPexton/6502) — the original 6502 / Apple I emulator this project is based on
- [NAudio](https://github.com/naudio/NAudio) — audio output
