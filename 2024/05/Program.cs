using System.Diagnostics;
using System.Text.RegularExpressions;

var (rules, updates) = LoadData(args[0]);
var incorrectUpdates = updates.Where(u => !IsCorrect(rules, u));
var correctedUpdates = incorrectUpdates.Select(u => CorrectUpdate(rules, u));
var result = correctedUpdates.Sum(u => u.MiddleNumber);
Console.WriteLine("Result: {0}", result);


static bool IsCorrect(List<(int, int)> rules, Update u)
{
    foreach (var (before, after) in rules)
    {
        if (u.CollationOrder.TryGetValue(before, out int beforePos) &&
            u.CollationOrder.TryGetValue(after, out int afterPos))
        {
            if (beforePos >= afterPos)
                return false;
        }
    }
    return true;
}

static Update CorrectUpdate(List<(int, int)> rules, Update u)
{
    if (IsCorrect(rules, u))
        return u;
    bool changed = true;
    while (changed)
    {
        changed = false;
        foreach (var (before, after) in rules)
        {
            if (u.CollationOrder.TryGetValue(before, out int beforePos) &&
                u.CollationOrder.TryGetValue(after, out int afterPos))
            {
                // If two are found to be in the wrong order, swap them.
                if (beforePos >= afterPos)
                {
                    (u.PageNumbers[afterPos], u.PageNumbers[beforePos]) =
                        (u.PageNumbers[beforePos], u.PageNumbers[afterPos]);
                    u.CollationOrder[before] = afterPos;
                    u.CollationOrder[after] = beforePos;
                    changed = true;
                }
            }
        }
    }
    return u;
}

static (List<(int, int)> rules, List<Update> updates) LoadData(string filename)
{
    using StreamReader rdr = File.OpenText(filename);
    var rules = ReadRules(rdr);
    var updates = ReadUpdates(rdr);
    return (rules, updates);
}

static List<(int, int)> ReadRules(StreamReader rdr)
{
    var re = new Regex(@"(\d+)\|(\d+)");
    var result = new List<(int, int)>();
    for (; ; )
    {
        var line = rdr.ReadLine();
        if (line is null)
            return result;
        var match = re.Match(line);
        if (!match.Success)
            return result;

        var from = int.Parse(match.Groups[1].ValueSpan);
        var to = int.Parse(match.Groups[2].ValueSpan);
        result.Add((from, to));
    }
}


static List<Update> ReadUpdates(StreamReader rdr)
{
    var result = new List<Update>();
    for (; ; )
    {
        var line = rdr.ReadLine();
        if (line is null)
            return result;
        var sPages = line.Split(',');
        var update = new Update(sPages.Select(s => int.Parse(s)));
        result.Add(update);
    }
}

public class Update
{
    public Update(IEnumerable<int> pageNos)
    {
        this.CollationOrder = [];
        var pages = new List<int>();
        foreach (var pageNo in pageNos)
        {
            pages.Add(pageNo);
            CollationOrder.Add(pageNo, CollationOrder.Count);
        }
        this.PageNumbers = pages.ToArray();
        Debug.Assert(PageNumbers.Length > 0, "Expected positive number of pages.");
        Debug.Assert((PageNumbers.Length & 1) == 1, "Expected odd number of pages");
    }

    public int[] PageNumbers { get; }
    public Dictionary<int, int> CollationOrder { get; }

    public int MiddleNumber
    {
        get
        {
            return PageNumbers[PageNumbers.Length / 2];
        }
    }
}