// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Text.RegularExpressions;

var width = Convert.ToInt32(args[0]);
var height = Convert.ToInt32(args[1]);
var iMin = Convert.ToInt32(args[2]);
var data = LoadData(width, height, args[3]);


int iMax = data.Count; 
while (iMin <= iMax)
{
    var iMid = iMin + (iMax - iMin) / 2;
    if (RunProblem(width, height, data, iMid) is null)
    {
        iMax = iMid - 1;
    }
    else 
    {
        iMin = iMid + 1;
    }
}
var p = data[iMax];
Console.WriteLine($"{p.X},{p.Y}");


List<Position>? RunProblem(int width, int height, List<Position> data, int cutoff)
{
    var arena = new Arena(width, height, data.Take(cutoff).ToHashSet());
    var start = new Position(0, 0);
    var end = new Position(width - 1, height - 1);
    var path= BFS(arena, start, end);
    return path;
}

void Render(Arena arena, Position current, List<Position> path)
{
    var onpath = path.ToHashSet();
    var sb = new StringBuilder();
    for (int y = 0; y < arena.Height; ++y)
    {
        sb.Clear();
        for (int x = 0; x < arena.Width; ++x)
        {
            char ch = '.';
            Position p = new(x, y);
            if (p == current)
                ch = '*';
            else if (onpath.Contains(p))
            {
                ch = 'O';
            }
            else if (arena.Obstacles.Contains(p))
                ch = '#';
            sb.Append(ch);
        }
        Console.WriteLine(sb);
    }
}

void RenderBfs(Arena arena, Position current, Dictionary<Position, Position> camefrom)
{
    var sb = new StringBuilder();
    for (int y = 0; y < arena.Height; ++y)
    {
        sb.Clear();
        for (int x = 0; x < arena.Width; ++x)
        {
            char ch = '.';
            Position p = new(x, y);
            if (p == current)
                ch = '*';
            else if (camefrom.Keys.Contains(p))
            {
                ch = 'O';
            }
            else if (arena.Obstacles.Contains(p))
                ch = '#';
            sb.Append(ch);
        }
        Console.WriteLine(sb);
    }
}

List<Position> LoadData(int width, int height, string filename)
{
    var reCoords = new Regex(@"(\d+),(\d+)");
    var data = new List<Position>();
    foreach (var line in File.ReadLines(filename))
    {
        var m = reCoords.Match(line);
        if (!m.Success)
            throw new InvalidDataException();
        var x = int.Parse(m.Groups[1].ValueSpan);
        var y = int.Parse(m.Groups[2].ValueSpan);
        data.Add(new(x, y));
    }
    return data;
}

List<Position>? BFS(Arena arena, Position start, Position goal)
{
    Queue<Position> probes = new Queue<Position>();
    probes.Enqueue(start);
    Dictionary<Position, Position> cameFrom = [];
    Dictionary<Position, int> costs = new() { { start, 0 } };
    while (probes.TryDequeue(out var current))
    {
        if (current == goal)
            return BuildPath(current, cameFrom);
        var cost = costs[current] + 1;
        foreach (var n in arena.Neighbors(current))
        {
            if (costs.TryGetValue(n, out var oldCost) && oldCost <= cost)
                continue;
            costs[n] = cost;
            cameFrom[n] = current;
            probes.Enqueue(n);

            // Console.Clear();
            // RenderBfs(n, cameFrom);
            // Console.ReadLine();
        }
    }
    return null;
}

List<Position> BuildPath(Position current, Dictionary<Position, Position> cameFrom)
{
    List<Position> totalPath = [];
    while (cameFrom.TryGetValue(current, out var f))
    {
        totalPath.Add(current);
        current = f;
    }
    return totalPath;
}

public record Arena(int Width, int Height, HashSet<Position> Obstacles)
{
    public bool IsBlocked(Position pos) => Obstacles.Contains(pos);

    public IEnumerable<Position> Neighbors(Position current)
    {
        for (int dy = -1; dy < 2; ++dy)
        {
            for (int dx = -1; dx < 2; ++dx)
            {
                if (Math.Abs(dx) + Math.Abs(dy) != 1)
                    continue;
                var p = new Position(current.X + dx, current.Y + dy);
                if (!IsInside(p))
                    continue;
                if (IsBlocked(p))
                    continue;
                yield return p;
            }
        }
    }

    public bool IsInside(Position p)
    {
        return 0 <= p.X && p.X < Width &&
            0 <= p.Y && p.Y < Height;
    }
}

public readonly record struct Position(int X, int Y)
{
    internal IEnumerable<object> Neighbors()
    {
        throw new NotImplementedException();
    }
}
