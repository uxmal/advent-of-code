using System.Diagnostics;

args = args.Length == 0
    ? [@"C:\dev\uxmal\advent\2024\07\input"]
    : args;
var equations = LoadEquations(args[0]);
var total = equations.Where(EqEvaluator.CouldBeTrue).Sum(e => e.Result);
Console.WriteLine($"Total: {total}");

static List<Equation> LoadEquations(string filename)
{
    List<Equation> result = [];
    using StreamReader rdr = File.OpenText(filename);
    for (; ; )
    {
        var line = rdr.ReadLine();
        if (line is null)
            return result;
        var eq = line.Split(':');
        if (eq.Length != 2)
            return result;
        var total = long.Parse(eq[0]);
        var terms = eq[1].Split(' ', StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToArray();
        result.Add(new(total, terms));
    }
}

static class EqEvaluator
{

    public static bool CouldBeTrue(Equation e)
    {
        long totalSoFar = e.Terms[0];
        return CouldBeTrueRecursive(e, 1, totalSoFar);
    }

    static readonly Func<long, long, long>[] operators = new Func<long, long, long>[]
    {
        (a, b) => a + b,
        (a, b) => a * b,
        Concat,
    };

    static long Concat(long a, long b)
    {
        Debug.Assert(b != 0);
        long t = b;
        while (t != 0)
        {
            a *= 10;
            t /= 10;
        }
        return a + b;
    }

    static bool CouldBeTrueRecursive(Equation e, int index, long totalSoFar)
    {
        if (index >= e.Terms.Length)
            return e.Result == totalSoFar;
        if (totalSoFar > e.Result)
            return false;
        foreach (var op in operators)
        {
            var newTotal = op(totalSoFar, e.Terms[index]);
            if (CouldBeTrueRecursive(e, index+1, newTotal))
                return true;
        }
        return false;
    }
}


public record class Equation(long Result, long[] Terms);
