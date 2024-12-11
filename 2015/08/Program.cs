if (args.Length == 0)
{
    interactive();
}
else 
{
    batch(args[0]);
}


static void interactive()
{
    for (;;)
    {
        Console.Out.Write("$ ");
        string? line = Console.ReadLine();
        if (line is null || line.Length == 0)
            break;
        var diff = ComputeSizeDifference(line);
        var ediff = ComputeEncodedSizeDifference(line);
        Console.WriteLine($"Difference:         {diff}");
        Console.WriteLine($"Encoded difference: {ediff}");
    }
}

static void batch(string filename)
{
    var sumDiff = File.ReadLines(filename).Sum(line => ComputeSizeDifference(line));
    Console.WriteLine($"Part1: Sum of differences: {sumDiff}");
    var sumDiffEncoded = File.ReadLines(filename).Sum(line => ComputeEncodedSizeDifference(line));
    Console.WriteLine($"Part2: Sum of encoded differences: {sumDiffEncoded}");
}

static int ComputeEncodedSizeDifference(string line)
{
    int chars = 2;  // start and end quote.
    for (int i = 0; i < line.Length; ++i)
    {
        char ch = line[i];
        if (ch == '"' || ch == '\\')
            ++chars;
        ++chars;
    }
    return chars - line.Length;
}

static int ComputeSizeDifference(string line)
{
    if (line.Length < 2)
        throw new InvalidDataException("Invalid string constant.");
    if (line[0] != '"')
        throw new InvalidDataException("Line should start with '\"'.");
    if (line[^1] != '"')
        throw new InvalidDataException("Line should end with '\"'.");
    int chars = 0;

    for (int i = 1; i < line.Length-1; ++i)
    {
        char ch = line[i];
        if (ch != '\\')
        {
            ++chars;
            continue;
        }
        ++i;
        if (i >= line.Length-1)
            throw new InvalidDataException("Escaped terminating quote.");
        ch = line[i];
        if (ch == '\\' || ch == '"')
        {
            ++chars;
            continue;
        }
        if (ch == 'x')
        {
            ++chars;
            i += 2;
        }
    }
    return line.Length - chars;
}
