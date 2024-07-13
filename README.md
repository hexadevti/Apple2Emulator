# Apple ][+ 

Este é um emulador de Apple II+ juntamente com o processador 6502. Feito por pura diversão, varios desafios e conceitos aprendidos com esse projeto. Baseado no projeto https://github.com/AndiPexton/6502, o qual traz um emulador de Apple I. Atualizei o projeto e corrigi alguns erros de alguns Opcodes.

Esse emulador traz alguns recursos como:

- Video Colorido/Monocromático (Hires)
- Som emulado como Speaker (usando componente NAudio https://github.com/naudio/NAudio)
- Acelerador de Clock (Até aproximadamente 50Mhz, dependendo do Hardware usado)
- Language card (16kb expansion)
- Saturn 128kb RAM card
- 80 Columns Card
- Disk II Card DOS/Prodos compatible

Alguns pontos de melhorias necessários, como: maior estabilidade do som e redução do consumo da máquina devido ao controle de clock. (C# possui algumas limitações de Thread.Sleep, mínimo de 1ms, com pouca precisão).

Sugestões são bem-vindas e agradeço qualquer tipo de contribuição ao projeto.

Referências importantes:

- Videx: https://mirrors.apple2.org.za/Apple%20II%20Documentation%20Project/Interface%20Cards/80%20Column%20Cards/Videx%20Videoterm/ROM%20Images/
- 6502 Opcodes: https://www.masswerk.at/6502/6502_instruction_set.html#BMI
- Virtu: https://github.com/digital-jellyfish/Virtu


Livros essenciais:

- Apple II Reference Manual: file:///C:/Users/luciano/OneDrive/%C3%81rea%20de%20Trabalho/Apple/books/Apple%20II%20Reference%20Manual.pdf
- Guia do programador DOS: https://datassette.s3.us-west-004.backblazeb2.com/livros/guia_do_programador_dos.pdf
- Beneath Apple DOS/Prodos: https://datassette.s3.us-west-004.backblazeb2.com/livros/beneath_apple_dos_prodos_2020.pdf
- Videx: file:///C:/Users/luciano/OneDrive/%C3%81rea%20de%20Trabalho/Apple/books/Videx%20Videoterm%20-%20Installation%20and%20Operation%20Manual.pdf

Próximos recursos:

- Joystick
- 65c02 (Apple //e)
- Hard Drive
- Double-High Resolution

Compatibilidade:

criado em dotnet core 8.0 (net8.0)
compativel com:
- dotnet core 7.0
- dotnet core 6.0