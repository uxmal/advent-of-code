using System.Diagnostics;
using System.Text;

var problemState = LoadProblemState(args[0]);
var cheapestScore = FindCheapestScore();

int FindCheapestScore()
{
    Stack<Probe> stack = [];
    stack.Push(new(problemState.Start, problemState.Direction, 0, 0, 0));
    Dictionary<Position, int> scores = [];
    var score = Dfs(stack, scores);
    return score;
}

int Dfs(Stack<Probe> stack, Dictionary<Position, int> scores)
{
    int bestScore = int.MaxValue;
    while (stack.TryPop(out var probe))
    {
        Render(probe, stack, scores, bestScore);
        if (probe.Position == problemState.End)
        {
            bestScore = Math.Min(bestScore, probe.Score);
            continue;
        }
        if (scores.TryGetValue(probe.Position, out var previousScore))
        {
            if (previousScore < probe.Score)
                continue;
        }
        scores[probe.Position] = probe.Score;

        var advance = probe.Position + probe.Direction;
        var l = probe.Direction.RotateLeft();
        var r = probe.Direction.RotateRight();
        var advanceL = probe.Position + l;
        var advanceR = probe.Position + r;
        if (!problemState.Walls.Contains(advanceL))
        {
            stack.Push(new(advanceL, l, probe.Score + 1001, probe.Turns + 1, probe.Advances + 1));
        }
        if (!problemState.Walls.Contains(advanceR))
        {
            stack.Push(new(advanceR, r, probe.Score + 1001, probe.Turns + 1, probe.Advances + 1));
        }
        if (!problemState.Walls.Contains(advance))
        {
            stack.Push(new(advance, probe.Direction, probe.Score + 1, probe.Turns, probe.Advances + 1));
        }
    }
    return bestScore;
}

Console.WriteLine($"Cheapest score: {cheapestScore}");

void Render(Probe tos, Stack<Probe> stack, Dictionary<Position, int> scores, int bestScore)
{
    return ;
    var sb = new StringBuilder();
    var stackPos = stack.Select(p => p.Position).ToHashSet();
    for (int y = 0; y < problemState.Height; ++y)
    {
        sb.Clear();
        for (int x = 0; x < problemState.Width; ++x)
        {
            var pos = new Position(x, y);
            if (pos == tos.Position)
            {
                sb.Append(tos.Direction.Render());
            }
            else if (scores.ContainsKey(pos) && stackPos.Contains(pos))
            {
                sb.Append('*');
            }
            else if (problemState.Walls.Contains(pos))
            {
                sb.Append('#');
            }
            else
            {
                sb.Append('.');
            }
        }
        Console.WriteLine(sb);
    }
    Console.WriteLine($"Turns: {tos.Turns}, advances: {tos.Advances}, best {bestScore}   ");

    Console.ReadLine();
}

static ProblemState LoadProblemState(string filename)
{
    int width = 0;
    int y = 0;
    Position? start = null;
    Position? end = null;
    HashSet<Position> walls = [];
    foreach (var line in File.ReadLines(filename))
    {
        Debug.Assert(width == 0 || width == line.Length);
        width = line.Length;
        int x = 0;
        foreach (var ch in line)
        {
            switch (ch)
            {
                case '#':
                    walls.Add(new(x, y));
                    break;
                case 'E':
                    end = new(x, y);
                    break;
                case 'S':
                    start = new(x, y);
                    break;
            }
            ++x;
        }
        ++y;
    }
    Debug.Assert(start.HasValue);
    Debug.Assert(end.HasValue);
    return new ProblemState(width, y, start.Value, Vector.E, end.Value, walls);
}


public class ProblemState
{
    public ProblemState(int width, int y, Position start, Vector dir, Position end, HashSet<Position> walls)
    {
        Width = width;
        Height = y;
        Start = start;
        Direction = dir;
        End = end;
        Walls = walls;
    }

    public int Width { get; }
    public int Height { get; }
    public Position Start { get; }
    public Vector Direction { get; }
    public Position End { get; }
    public HashSet<Position> Walls { get; }
}

public record struct Position(int X, int Y)
{
    public static Position operator +(Position pos, Vector v)
    {
        return new Position(pos.X + v.X, pos.Y + v.Y);
    }
}

public record struct Vector(int X, int Y)
{
    public static Vector N { get; } = new(0, -1);
    public static Vector E { get; } = new(1, 0);
    public static Vector S { get; } = new(0, 1);
    public static Vector W { get; } = new(-1, 0);

    public Vector RotateRight()
    {
        return new Vector(-this.Y, this.X);
    }

    public Vector RotateLeft()
    {
        return new Vector(this.Y, -this.X);
    }

    public char Render()
    {
        if (X == 0)
        {
            return Y == 1 ? 'v' : '^';
        }
        else
        {
            return X == 1 ? '>' : '<';
        }
    }

}

public record Probe(Position Position, Vector Direction, int Score, int Turns, int Advances);