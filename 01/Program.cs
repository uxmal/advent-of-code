using System.Diagnostics;
using System.Text.RegularExpressions;

var (left, right) = ReadInput(args[0]);
int result = ComputeDifferences(left, right);
int similarity = ComputeSimilarityScore(left, right);
Console.WriteLine($"Sum of differences: {result}");
Console.WriteLine($"Similarity score:   {similarity}");

static int ComputeDifferences(List<int> left, List<int> right)
{
    left.Sort();
    right.Sort();
    var sumDistances = 0;
    var el = left.GetEnumerator();
    var er = right.GetEnumerator();
    while (el.MoveNext() && er.MoveNext())
    {
        var l = el.Current;
        var r = er.Current;
        var dist = Math.Abs(l - r);
        sumDistances += dist;
    }
    return sumDistances;
}

static int ComputeSimilarityScore(List<int> left, List<int> right)
{
    var rightTallies =
        (from r in right
         group r by r into g
         select new { g.Key, Count = g.Count() })
        .ToDictionary(k => k.Key, v => v.Count);
    int result = 0;
    foreach (var l in left)
    {
        if (rightTallies.TryGetValue(l, out var tally))
        {
            result += l * tally;
        }
    }
    return result;
}

static (List<int>, List<int>) ReadInput(string filename)
{
    using TextReader rdr = File.OpenText(filename);
    string? line = rdr.ReadLine();
    int pos = 0;
    var left = new List<int>();
    var right = new List<int>();
    var re = new Regex(@"\s+");
    while (line is not null)
    {
        var nums = re.Replace(line.Trim(), " ").Split();
        Debug.Assert(nums.Length == 2);
        var l = Convert.ToInt32(nums[0]);
        var r = Convert.ToInt32(nums[1]);
        left.Add(l);
        right.Add(r);
        ++pos;
        line = rdr.ReadLine();
    }
    return (left, right);
}