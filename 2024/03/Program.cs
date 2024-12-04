using System.Diagnostics;
using System.Text.RegularExpressions;

var muls = ParseMuls(args[0]);
var result = EvaluateMuls(muls);
Console.WriteLine("Sum of products: {0}", result);


static long EvaluateMuls(IEnumerable<(int, int)> pairs)
{
    return pairs.Sum(p => p.Item1 * p.Item2);
}

static IEnumerable<(int, int)> ParseMuls(string filename)
{
    var reMul = new Regex(@"mul\((\d+),(\d+)\)|do(n't)?");
    var text = File.ReadAllText(filename);
    var doMuls = true;
    foreach (Match m in reMul.Matches(text))
    {
        var v = m.Value;
        if (v == "don't")
        {
            doMuls = false;
        }
        else if (v == "do")
        {
            doMuls = true;
        }
        else
        {
            Debug.Assert(m.ValueSpan.StartsWith("mul"));
            if (doMuls)
            {
                var n1 = int.Parse(m.Groups[1].ValueSpan);
                var n2 = int.Parse(m.Groups[2].ValueSpan);
                yield return (n1, n2);
            }
        }
    }
}
