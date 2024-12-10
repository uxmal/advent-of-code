using System.Runtime.CompilerServices;

var arena = LoadArena(args[0]);

var graph = FindAllEdges(arena);
Dictionary<Position, int> trailheadScores = [];
int sumScores = ScoreTrails(graph, arena);
Console.WriteLine($"Sum of scores: {sumScores}");

static Arena LoadArena(string filename)
{
    return new Arena(File.ReadAllLines(filename));
}

int ScoreTrails(Graph graph, Arena arena)
{
#if PART1
    foreach (var peak in graph.Sinks)
    {
        HashSet<Position> visited = [];
        ScoreTrailsDown(peak, graph, arena, visited);
        RenderAndWait(arena, visited);

    }
    return trailheadScores.Values.Sum();

#else
    int totalScore = 0;
    foreach (var head in graph.Sources)
    {
        HashSet<Position> visited = [];
        int score = CountPaths(head, graph, arena, visited);
        totalScore += score;
        RenderAndWaitRating(arena, visited, score);
    }
    return totalScore;
#endif
}

static int CountPaths(Position pos, Graph graph, Arena arena, HashSet<Position> visited)
{
    if (arena[pos.X, pos.Y] == 9)
    {
        return 1;
    }
    if (!visited.Add(pos))
        return 0;
    int cPaths = 0;
    foreach (var next in graph.Succ(pos))
    {
        cPaths += CountPaths(next, graph, arena, visited);
    }
    visited.Remove(pos);
    return cPaths;
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

static void RenderAndWaitRating(Arena arena, HashSet<Position> visited, int score)
{
    return;
    // Console.Clear();
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
    Console.ReadLine();
    // Console.ReadKey(true);
}

static Graph FindAllEdges(Arena arena)
{
    var graph = new Graph();
    var heads = graph.Sources;
    var peaks = graph.Sinks;
    Position from;

    void Probe(int dx, int dy)
    {
        var to = new Position(from.X + dx, from.Y + dy);
        if (!arena.IsInRange(to))
            return;
        var elevFrom = arena[from.X, from.Y];
        var elevTo = arena[to.X, to.Y];
        if (elevTo - elevFrom == 1)
            graph.AddEdge(from, to);
    }

    for (int y = 0; y < arena.Height; ++y)
    {
        for (int x = 0; x < arena.Width; ++x)
        {
            from = new Position(x, y);
            if (arena[x, y] == 9)
                peaks.Add(from);
            if (arena[x, y] == 0)
                heads.Add(from);
            Probe(1, 0);
            Probe(0, 1);
            Probe(0, -1);
            Probe(-1, 0);
        }
    }
    return graph;
}