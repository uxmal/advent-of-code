using System.Diagnostics.CodeAnalysis;

var dimensions = ReadPresentDimensions(args[0]);
long totalPaper = dimensions.Select(d => PaperRequired(d)).Sum();
long totalRibbon = dimensions.Select(d => RibbonRequired(d)).Sum();

Console.WriteLine($"Total paper required: {totalPaper} sq ft");
Console.WriteLine($"Total paper required: {totalRibbon} sq ft");

static long PaperRequired(int[] dims)
{
    var l = dims[0];
    var w = dims[1];
    var h = dims[2];
    var sides = new long[] {
        l*w,
        l*h,
        w*h
    };
    var smallestSide = sides.Min();
    return smallestSide + 2 * sides.Sum();
}

static long RibbonRequired(int[] dims)
{
    var l = dims[0];
    var w = dims[1];
    var h = dims[2];
    var perimeters = new long[] {
        2*(l+w),
        2*(l+h),
        2*(w+h),
    };
    var smallestPerimeter = perimeters.Min();
    var bow = l*w*h;
    return smallestPerimeter + bow;
}

static List<int[]> ReadPresentDimensions(string filename)
{
    var result = new List<int[]>();
    using var rdr = File.OpenText(filename);
    for (; ; )
    {
        string? line = rdr.ReadLine();
        if (line is null)
            return result;
        var dims = line.Split('x').Select(x => int.Parse(x)).ToArray();
        if (dims.Length != 3)
            throw new InvalidDataException();
        result.Add(dims);
    }
}