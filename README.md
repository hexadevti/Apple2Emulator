# Apple2Sharp - dotnet C# Apple ][+ emulator

*For Windows only

This is an Apple II+ emulator with the 6502 processor. Made for pure fun, several challenges and concepts learned with this project. Based on the project https://github.com/AndiPexton/6502, which features an Apple I emulator. I updated the project and fixed some errors in some Opcodes.

This emulator brings some features such as:

- Color/Monochrome Video (Hires)
- Sound emulated as Speaker (using NAudio component https://github.com/naudio/NAudio)
- Clock Accelerator (Up to approximately 50Mhz, depending on the Hardware used)
- Language card (16kb expansion)
- Saturn 128kb RAM card
- 80 Columns Card
- Disk II Card DOS/Prodos compatible .dsk, .po, .do image files*
- Joystick (when activated, use arrows for directions and control/shift for buttons 1/2)


*.dsk files examples at \disks folder

Some points of necessary improvements, such as: greater sound stability and reduction of machine consumption due to clock control. (C# has some Thread.Sleep limitations, minimum of 1ms, with little precision).

Suggestions are welcome and I appreciate any kind of contribution to the project.

Important references:
Referências importantes:

- Videx: https://mirrors.apple2.org.za/Apple%20II%20Documentation%20Project/Interface%20Cards/80%20Column%20Cards/Videx%20Videoterm/ROM%20Images/
- 6502 Opcodes: https://www.masswerk.at/6502/6502_instruction_set.html#BMI
- Virtu: https://github.com/digital-jellyfish/Virtu


Essential books:

- Apple II Reference Manual: file:///C:/Users/luciano/OneDrive/%C3%81rea%20de%20Trabalho/Apple/books/Apple%20II%20Reference%20Manual.pdf
- Guia do programador DOS: https://datassette.s3.us-west-004.backblazeb2.com/livros/guia_do_programador_dos.pdf
- Beneath Apple DOS/Prodos: https://datassette.s3.us-west-004.backblazeb2.com/livros/beneath_apple_dos_prodos_2020.pdf
- Videx: file:///C:/Users/luciano/OneDrive/%C3%81rea%20de%20Trabalho/Apple/books/Videx%20Videoterm%20-%20Installation%20and%20Operation%20Manual.pdf

Upcoming features:

- 65c02 (Apple //e)
- Apple //e board
- Hard Drive
- Double-High Resolution
- 800kb disks
- Mouse


Compatibility:

reated in dotnet core 8.0 (net8.0) compatible with:
- dotnet core 7.0
- dotnet core 6.0