using System.Text.RegularExpressions;

Regex reCmd = new(@"(.*?) (\d+),(\d+) through (\d+),(\d+)");

if (args.Length == 0)
{
    interactive();
}
else
{
    batch(args[0]);
}

void interactive()
{
    var arena = new Arena(10, 10);
    for (;;)
    {
        arena.Render(Console.Out);
        string? line = Console.ReadLine();
        if (line is null || line.Length == 0)
            break;
        interpretCommand(line, arena);
    }
}

void batch(string filename)
{
    var commands = File.ReadAllLines(filename);
    var arena = new Arena(1000, 1000);
    foreach (var cmd in commands)
    {
        interpretCommand(cmd, arena);
    }
    Console.WriteLine($"Total lights: {arena.CountLights()}");
}


bool interpretCommand(string cmd, Arena arena)
{
    if (cmd.StartsWith("c"))
    {
        arena.Clear();
        return true;
    }
    var match = reCmd.Match(cmd);
    if (!match.Success)
        return false;
    var g = match.Groups;
    var pos1 = new Position(int.Parse(g[2].ValueSpan), int.Parse(g[3].ValueSpan));
    var pos2 = new Position(int.Parse(g[4].ValueSpan), int.Parse(g[5].ValueSpan));
    switch (g[1].Value)
    {
        case "turn on":
        arena.TurnOn(pos1, pos2);
        break;
        case "turn off":
        arena.TurnOff(pos1, pos2);
        break;
        case "toggle":
        arena.Toggle(pos1, pos2);
        break;
        default:
        return false;
    }
    return true;
}