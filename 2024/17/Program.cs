using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using ValueType = System.Int64;

var problemState = LoadProgram(args[0]);
// problemState.Disasm();
// Console.WriteLine("---");
// problemState.Lift();
// Console.WriteLine("---");

// for (ValueType aa = 1;; aa += 8*8*8*8)
// {
//         problemState.Registers[0] = aa;
//         problemState.Registers[1] = 0;
//         problemState.Registers[2] = 0;
//         var result = problemState.Execute();
//         PrintResult(result);
// }

var digits = new byte[problemState.Program.Length];
ValueType a = Dfs(digits);
Console.WriteLine($"A: {a}");


ValueType Dfs(byte[] digits)
{
    Stack<(int, int)> stack = [];
    stack.Push((1, 0));
    while (stack.TryPop(out var tos))
    {
        var (digit, idigit) = tos;
        digits[idigit] = (byte)digit;
        var a = MakeNumber(digits);
        problemState.Registers[0] = a;
        problemState.Registers[1] = 0;
        problemState.Registers[2] = 0;
        var result = problemState.Execute();

        // Console.WriteLine($"Testing: {a} - 0o{Convert.ToString(a, 8)}");
        // Console.Write("  ");
        // PrintResult(result);

        if (result.Count == problemState.Program.Length &&
            result[digits.Length - 1 - idigit] == problemState.Program[digits.Length - 1 - idigit])
        {
            if (AreEqual(result, problemState.Program))
                return a;
            if (idigit < digits.Length - 1)
            {
                stack.Push((digit+1, idigit));
                stack.Push((0, idigit + 1));
            }
        }
        else
        {
            ++digit;
            if (digit < 8)
                stack.Push((digit, idigit));
            else 
                 _ = digit;
        }
    }
    return -1;
}

static ValueType MakeNumber(byte[] digits)
{
    ValueType number = 0;
    foreach (var digit in digits)
    {
        number = (number << 3) + digit;
    }
    return number;
}

int TestProgram(ValueType a)
{
    problemState.Registers[0] = a;
    problemState.Registers[1] = 0;
    problemState.Registers[2] = 0;

    var result = problemState.Execute();
    if (result.Count != problemState.Program.Length)
    {
        PrintResult(result);
        return 1;
    }
    if (AreEqual(result, problemState.Program))
    {
        return 0;
    }
    PrintResult(result);
    return 2;
}

static void PrintResult(List<byte> result)
{
    Console.WriteLine(string.Join(",", result.Select(b => b.ToString())));
}

static bool AreEqual(List<byte> result, byte[] program)
{
    if (result.Count != program.Length)
        return false;
    for (int i = 0; i < result.Count; ++i)
    {
        if (result[i] != program[i])
            return false;
    }
    return true;
}

ProblemState LoadProgram(string filename)
{
    var reRegister = new Regex(@"Register (.): (\d+)");
    var reProgram = new Regex(@"Program: (.*)");
    using var rdr = File.OpenText(filename);
    ValueType[] regs = [0, 0, 0];
    string? line;
    for (; ; )
    {
        line = rdr.ReadLine();
        if (line is null || line.Length == 0)
            break;
        var m = reRegister.Match(line);
        if (!m.Success)
            throw new InvalidDataException();
        var value = ValueType.Parse(m.Groups[2].ValueSpan);
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
    public ValueType[] Registers { get; }
    public byte[] Program { get; }
    public int InstructionPointer { get; private set; }

    public ProblemState(byte[] program, ValueType[] registers)
    {
        this.Registers = registers;
        this.Program = program;
    }

    public void Lift()
    {
        for (int i = 0; i < Program.Length;)
        {
            var opcode = Program[i++];
            var operand = Program[i++];
            switch (opcode)
            {
                case 0: // adv:
                    Console.WriteLine($"a = a >> {DisasmCombo(operand)}");
                    break;
                case 1:  // bxl
                    Console.WriteLine($"b = b ^ #{operand}");
                    break;
                case 2: // bst
                    Console.WriteLine($"b = {DisasmCombo(operand)}");
                    break;
                case 3: // jnz
                    Console.WriteLine($"if (a != 0) goto {operand}");
                    break;
                case 4: // bxc
                    Console.WriteLine("b = b ^ c");
                    break;
                case 5: //out
                    Console.WriteLine($"out({DisasmCombo(operand)})");
                    break;
                case 6: // bdv:
                    Console.WriteLine($"b = a >> {DisasmCombo(operand)}");
                    break;
                case 7: // cdv:
                    Console.WriteLine($"c = a >> {DisasmCombo(operand)}");
                    break;
                default:
                    throw new NotImplementedException($"Opcode {opcode}.");
            }
        }
    }
    public void Disasm()
    {
        for (int i = 0; i < Program.Length;)
        {
            var opcode = Program[i++];
            var operand = Program[i++];
            switch (opcode)
            {
                case 0: // adv:
                    Console.WriteLine($"adv {DisasmCombo(operand)}");
                    break;
                case 1:  // bxl
                    Console.WriteLine($"bxl #{operand}");
                    break;
                case 2: // bst
                    Console.WriteLine($"bst {DisasmCombo(operand)}");
                    break;
                case 3: // jnz
                    Console.WriteLine($"jnz #{operand}");
                    break;
                case 4: // bxc
                    Console.WriteLine("bxc");
                    break;
                case 5: //out
                    Console.WriteLine($"out {DisasmCombo(operand)}");
                    break;
                case 6: // bdv:
                    Console.WriteLine($"bdv {DisasmCombo(operand)}");
                    break;
                case 7: // cdv:
                    Console.WriteLine($"cdv {DisasmCombo(operand)}");
                    break;
                default:
                    throw new NotImplementedException($"Opcode {opcode}.");
            }
        }
    }


    public List<byte> Execute()
    {
        List<byte> result = [];
        InstructionPointer = 0;
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
                    result.Add((byte)value);
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
        return result;
    }

    private int Literal(byte operand)
    {
        return operand;
    }

    private string DisasmCombo(byte operand)
    {
        return operand switch
        {
            0 or 1 or 2 or 3 => $"#{operand}",
            4 => "a",
            5 => "b",
            6 => "c",
            _ => throw new NotImplementedException($"Combo operand {operand}.")
        };

    }
    private ValueType Combo(byte operand)
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