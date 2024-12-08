using System.Diagnostics;
using Advent2024.Day08;

var state = ReadInput(args[0]);
var sw = Stopwatch.StartNew();
PlaceAntinodes(state);
Console.WriteLine($"Unique antinodes {state.Antinodes.Count} (in {sw.ElapsedMilliseconds}ms)");

static void PlaceAntinodes(ProblemState state)
{
    foreach (var (freq, positions) in state.Antennae)
    {
        for (int i = 0; i < positions.Count; ++i)
        {
            for (int j = i+1; j < positions.Count; ++j)
            {
                var vec = positions[j] - positions[i];
                var antinode1 = positions[j] + vec;
                var antinode2 = positions[i] - vec;
                if (state.Arena.IsInBounds(antinode1))
                {
                    state.Antinodes.Add(antinode1);
                    // state.Render();
                    // Console.ReadKey(true);
                }
                if (state.Arena.IsInBounds(antinode2))
                {
                    state.Antinodes.Add(antinode2);
                    // state.Render();
                    // Console.ReadKey(true);
                }
            }
        }
    }
}

static ProblemState ReadInput(string filename)
{
    int height = 0;
    int width = 0;
    Dictionary<char, List<Position>> antennae = [];
    Dictionary<Position, char> antennaPositions = [];
    using StreamReader rdr = File.OpenText(filename);
    for (;;)
    {
        string? line = rdr.ReadLine();
        if (line is null)
            break;
        if (width == 0)
            width = line.Length;
        else if (line.Length != width)
            throw new InvalidOperationException("Line lengths are inconsistent.");
        for (int x = 0; x < line.Length; ++x)
        {
            char c = line[x];
            if (!char.IsAsciiLetterOrDigit(c))
                continue;
            if (!antennae.TryGetValue(c, out var positions))
            {
                positions = [];
                antennae.Add(c, positions);
            }
            var pos = new Position(x, height);
            positions.Add(pos);
            antennaPositions.Add(pos, c);
        }
        ++height;
    }
    var arena = new Arena(width, height);
    return new ProblemState(arena, antennae, antennaPositions);
}
