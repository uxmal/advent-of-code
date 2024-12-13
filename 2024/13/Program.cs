using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

var subProblems = ReadSubproblems(args[0]);
var tokens = SpendTokens(subProblems);
Console.WriteLine($"Total tokens: {tokens}");

double SpendTokens(List<Subproblem> subProblems)
{
    const double APressCost = 3;
    const double BPressCost = 1;
    double totalCost = 0.0;
    foreach (var subProblem in subProblems)
    {
        var v = subProblem.Solve();
        if (subProblem.IsIntegerSolution(v))
        {
            var cost = APressCost*v.X + BPressCost*v.Y;
            Console.WriteLine($"X: {v.X}, Y:{v.Y}, cost:{cost}");
            totalCost += cost;
        }
    }
    return totalCost;
}




static List<Subproblem> ReadSubproblems(string filename)
{
    var result = new List<Subproblem>();
    var reA = new Regex(@"Button A: X\+(\d+), Y\+(\d+)");
    var reB = new Regex(@"Button B: X\+(\d+), Y\+(\d+)");
    var rePrize = new Regex(@"Prize: X=(\d+), Y=(\d+)");
    using var rdr = File.OpenText(filename);
    for (;;)
    {
        var a = rdr.ReadLine();
        if (a is null)
            break;
        var b = rdr.ReadLine();
        if (b is null)
            throw new InvalidDataException("Expected B button.");
        var prize = rdr.ReadLine();
        if (prize is null)
            throw new InvalidDataException("Expected Prize.");
        var mA = reA.Match(a);
        var mB = reB.Match(b);
        var mPrize = rePrize.Match(prize);
        if (!(mA.Success && mB.Success && mPrize.Success))
            throw new InvalidDataException("");
        
        var vecA = new Vector(
            double.Parse(mA.Groups[1].ValueSpan),
            double.Parse(mA.Groups[2].ValueSpan));
        var vecB = new Vector(
            double.Parse(mB.Groups[1].ValueSpan),
            double.Parse(mB.Groups[2].ValueSpan));
        var posPrize = new Position(
            double.Parse(mPrize.Groups[1].ValueSpan),
            double.Parse(mPrize.Groups[2].ValueSpan));
        var s = new Subproblem(vecA, vecB, posPrize);
        result.Add(s);

        if (rdr.ReadLine() is null)
            break;

    }
    return result;
}

record Subproblem(Vector A, Vector B, Position Prize)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Button A: X+{A.X}, Y+{B.Y}");
        sb.AppendLine($"Button B: X+{B.X}, Y+{B.Y}");
        sb.AppendLine($"Prize: X={Prize.X}, Y={Prize.Y}");
        return sb.ToString();
    }

    internal Vector Solve()
    {
        var det = 1.0 / (A.X*B.Y - B.X*A.Y);
        double[,] inv = new double[2,2]
        {
            { B.Y*det, -B.X*det},
            { -A.Y*det, A.X*det }
        };
        var solnX = inv[0,0] * Prize.X + inv[0,1] * Prize.Y;
        var solnY = inv[1,0] * Prize.X + inv[1,1] * Prize.Y;
        return new Vector(Math.Round(solnX,5), Math.Round(solnY, 5));
    }

    public bool IsIntegerSolution(Vector v)
    {
        bool isInt =
            Math.Round(v.X, 0) == v.X &&
            Math.Round(v.Y, 0) == v.Y;
        return isInt;
    }
}



record struct Vector(double X, double Y);

record struct Position(double X, double Y);