using System.Numerics;
using System.Text.RegularExpressions;

var problemState = LoadProgram(args[0]);
problemState.Execute();


ProblemState LoadProgram(string filename)
{
    var reRegister = new Regex(@"Register (.): (\d+)");
    var reProgram = new Regex(@"Program: (.*)");
    using var rdr = File.OpenText(filename);
    BigInteger[] regs = [0, 0, 0];
    string? line;
    for (; ; )
    {
        line = rdr.ReadLine();
        if (line is null || line.Length == 0)
            break;
        var m = reRegister.Match(line);
        if (!m.Success)
            throw new InvalidDataException();
        var value = BigInteger.Parse(m.Groups[2].ValueSpan);
        var reg = m.Groups[1].ValueSpan[0] - 'A';
        regs[reg] = value;
    }

    line = rdr.ReadLine();
    if (line is null)
        throw new InvalidDataException();
    var mm = reProgram.Match(line);
    if (!mm.Success)
        throw new InvalidDataException();
    
    var program = mm.Groups[1].Value.Split(',').Select(b => byte.Parse(b)).ToArray();
    return new ProblemState(program, regs);
}

class ProblemState
{
    public BigInteger[] Registers { get; }
    public byte[] Program { get; }
    public int InstructionPointer { get; private set; }

    public ProblemState(byte[] program, BigInteger[] registers)
    {
        this.Registers = registers;
        this.Program = program;
    }

    public void Execute()
    {
        var sep = "";
        while (InstructionPointer < Program.Length)
        {
            var opcode = Program[InstructionPointer++];
            var operand = Program[InstructionPointer++];
            switch (opcode)
            {
                case 0: // adv:
                    int ivalue = (int)Combo(operand);
                    Registers[0] = Registers[0] >> ivalue;
                    break;
                case 1:  // bxl
                    ivalue = Literal(operand);
                    Registers[1] ^= ivalue;
                    break;
                case 2: // bst
                    var value = Combo(operand) & 0x7;
                    Registers[1] = value;
                    break;
                case 3: // jnz
                    if (Registers[0] == 0)
                        break;
                    InstructionPointer = Literal(operand);
                    break;
                case 4: // bxc
                    Registers[1] ^= Registers[2];
                    break;
                case 5: //out
                    value = Combo(operand) & 7;
                    Console.Write(sep);
                    Console.Write(value);
                    sep = ",";
                    break;
                case 6: // bdv:
                    ivalue = (int)Combo(operand);
                    Registers[1] = Registers[0] >> ivalue;
                    break;
                case 7: // adv:
                    ivalue = (int)Combo(operand);
                    Registers[2] = Registers[0] >> ivalue;
                    break;
                default:
                    throw new NotImplementedException($"Opcode {opcode}.");
            }
        }
    }

    private int Literal(byte operand)
    {
        return operand;
    }

    private BigInteger Combo(byte operand)
    {
        return operand switch
        {
            0 or 1 or 2 or 3 => operand,
            4 => Registers[0],
            5 => Registers[1],
            6 => Registers[2],
            _ => throw new NotImplementedException($"Combo operand {operand}.")
        };
    }

}