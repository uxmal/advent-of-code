using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

Regex rePart = new(@"^(.*) -> (\w+)$");
Regex reBinaryGate = new(@"(\w+) (\w+) (\w+)");
Regex reUnaryGate = new(@"(\w+) (\w+)");
Regex reWire = new(@"(\w+)");
Regex reSignal = new(@"^(\d+)");

var state = new ProblemState();
if (args.Length == 0)
{
    interactive(state);
}
else
{
    batch(args[0],state);
}

void interactive(ProblemState state)
{
    for(;;)
    {
        var line = Console.ReadLine();
        if (line is null || line.Length == 0)
            break;
        if (!ProcessLine(line, state))
            Console.WriteLine("Huh?");
        else
            state.Render(false);
    }
    state.Propagate();
    state.Render(false);
}

void batch(string filename, ProblemState state)
{
    using var rdr = File.OpenText(filename);
    for (;;)
    {
        var line = rdr.ReadLine();
        if (line is null || line.Length == 0)
            break;
        bool yes = ProcessLine(line, state);
        if (!yes)
        {
            Console.WriteLine("*** Error: {0}", line);
            System.Environment.Exit(-1);
        }
    }
    state.Propagate();
    Console.WriteLine("====");
    state.Render(false);
}


bool ProcessLine(string line, ProblemState state)
{
    var m = rePart.Match(line);
    if (!m.Success)
        return false;
    var src = m.Groups[1].Value;
    var dst = m.Groups[2].Value;
    
    m = reBinaryGate.Match(src);
    if (m.Success)
    {
        var op1 = m.Groups[1].Value;
        var fn = m.Groups[2].Value;
        var op2 = m.Groups[3].Value;
        Console.WriteLine($"{line,-30} : {op1} {fn} {op2} -> {dst}");
        switch (fn)
        {
            case "AND": state.CreateBinary(op1, op2, dst, (a, b) => a & b); break;
            case "OR": state.CreateBinary(op1, op2, dst, (a, b) => a | b); break;
            case "LSHIFT": state.CreateShift(op1, op2, dst, (a, b) => a << b); break;
            case "RSHIFT": state.CreateShift(op1, op2, dst, (a, b) => a >>> b); break;
            default: throw new NotSupportedException($"Unsupported gate type: {fn}.");
        }
        return true;
    }
    m = reUnaryGate.Match(line);
    if (m.Success)
    {
        var fn = m.Groups[1].Value;
        var op = m.Groups[2].Value;
        if (fn == "NOT")
        {
            state.CreateUnary(op, dst, a => ~a);
            return true;
        }
        else 
        {
            throw new NotSupportedException($"Unsupported gate type: {fn}.");
        }
    }
    m = reSignal.Match(line);
    if (m.Success)
    {
        state.SetWireSignal(int.Parse(m.Groups[1].Value), dst);
        return true;
    }
    m = reWire.Match(line);
    if (m.Success)
    {
        var op = m.Groups[1].Value;
        state.CreateUnary(op, dst, a => a);
        return true;
    }

    return false;
}
