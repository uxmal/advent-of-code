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
        string? line = Console.ReadLine();
        if (line is null || line.Length == 0)
            break;
        var state = ParseLine(line);
        for (;;)
        {
            state.Render();
            line = Console.ReadLine();
            if (line is null || line.Length > 0 && line[0] == 'q')
                break;
            state.Blink();
        }
    }
}


static void batch(string filename)
{
    string? line = File.ReadLines(filename).First();
    var state = ParseLine(line);
    for (int i = 0; i < 25; ++i)
    {
        state.Blink();
    }
    Console.WriteLine($"{state.CountItems()} stones");
}

static ProblemState ParseLine(string line)
{
    var stones = line.Split(' ')
        .Select(s => Convert.ToInt64(s.Trim()))
        .ToArray();
    return new ProblemState(stones);
}