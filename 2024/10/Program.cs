using System.Runtime.CompilerServices;

var arena = LoadArena(args[0]);

var (peaks, graph) = FindAllEdges(arena);
Dictionary<Position, int> trailheadScores = [];
ScoreTrails(graph, arena, peaks);
var sumScores = trailheadScores.Values.Sum();
Console.WriteLine($"Sum of scores: {sumScores}");

static Arena LoadArena(string filename)
{
    return new Arena(File.ReadAllLines(filename));
}

void ScoreTrails(Graph graph, Arena arena, HashSet<Position> peaks)
{
    foreach (var peak in peaks)
    {
        HashSet<Position> visited = [];
        ScoreTrailsDown(peak, graph, arena, visited);
        RenderAndWait(arena, visited);
    }
}

void ScoreTrailsDown(Position pos, Graph graph, Arena arena, HashSet<Position> visited)
{
    if (!visited.Add(pos))
        return;
    var elev = arena[pos.X, pos.Y];
    if (elev == 0)
    {
        if (!trailheadScores.TryGetValue(pos, out var score))
            score = 0;
        trailheadScores[pos] = score + 1;
    }
    foreach (var next in graph.Succ(pos))
    {
        ScoreTrailsDown(next, graph, arena, visited);
    }
}

static void RenderAndWait(Arena arena, HashSet<Position> visited)
{
    return;
    Console.Clear();
    for (int y = 0; y < arena.Height; ++y)
    {
        for (int x = 0; x < arena.Width; ++x)
        {
            var pos = new Position(x, y);
            char ch = '.';
            if (visited.Contains(pos))
                ch = (char)(arena[x, y] + '0');
            Console.Write(ch);
        }
        Console.WriteLine();
    }
    Console.ReadKey(true);
}

static (HashSet<Position>, Graph) FindAllEdges(Arena arena)
{
    var peaks = new HashSet<Position>();
    var graph = new Graph();
    Position from;

    void Probe(int dx, int dy)
    {
        var to = new Position(from.X + dx, from.Y + dy);
        if (!arena.IsInRange(to))
            return;
        var elevFrom = arena[from.X, from.Y];
        var elevTo = arena[to.X, to.Y];
        if (elevFrom - elevTo != 1)
            return;
        graph.AddEdge(from, to);
    }

    for (int y = 0; y < arena.Height; ++y)
    {
        for (int x = 0; x < arena.Width; ++x)
        {
            from = new Position(x, y);
            if (arena[x, y] == 9)
                peaks.Add(from);
            Probe(1, 0);
            Probe(0, 1);
            Probe(0, -1);
            Probe(-1, 0);
        }
    }
    return (peaks, graph);
}