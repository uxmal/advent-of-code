using System.Diagnostics;
using System.Text.RegularExpressions;

var reports = ReadReports(args[0]);
var safeReports = reports.Count(r => IsSafeReportD(r));
Console.WriteLine("Safe reports: {0}", safeReports);

static bool IsSafeReportD(int[] report)
{
    var result = IsSafeReportWithDampener(report);
    return result;
}

static bool IsSafeReportWithDampener(int[] report)
{
    for (int i = -1; i < report.Length; ++i)
    {
        if (IsSafeReport(report, i))
            return true;
    }
    return false;
}

static bool IsSafeReport(int[] report, int iIgnore)
{
    Debug.Assert(report.Length > 1, "Must have at least two levels in a report");
    int iStart = iIgnore == 0 ? 1 : 0;
    var prev = report[iStart];
    var prevDelta = 0;
    for (int i = iStart + 1; i < report.Length; ++i)
    {
        if (i == iIgnore)
            continue;
        var level = report[i];
        var sdiff = level - prev;
        var diff = Math.Abs(sdiff);
        if (diff < 1 || diff > 3)
            return false;
        if (prevDelta == 0)
        {
            prevDelta = sdiff;
        }
        else
        {
            if (prevDelta * sdiff < 0)
                return false;
            prevDelta = sdiff;
        }
        prev = level;

    }
    return true;
}

static List<int[]> ReadReports(string filename)
{
    var re = new Regex(@"\s+");
    var result = new List<int[]>();
    using TextReader rdr = File.OpenText(filename);
    var line = rdr.ReadLine();
    while (line is not null)
    {
        var sReport = re.Replace(line, " ").Trim().Split(' ');
        var report = sReport.Select(s => Convert.ToInt32(s)).ToArray();
        result.Add(report);
        line = rdr.ReadLine();
    }
    return result;
}