using System.Data;
using System.Text.RegularExpressions;


var reRobot = new Regex(@"p=(\d+),(\d+) +v=(-?\d+),(-?\d+)");

var width = int.Parse(args[0]);
var height = int.Parse(args[1]);
var robots = LoadRobots(args[2]);
var state = new ProblemState
{
    Width = width,
    Height = height,
    Robots = robots,
};
for (int i = 0; i < 100; ++i)
{
    state.Advance();
}
Render(state);
var safetyFactor = state.SafetyFactor();
Console.WriteLine($"Safety factor: {safetyFactor}.");


List<Robot> LoadRobots(string filename)
{
    return File.ReadAllLines(filename)
        .Select(ParseRobot)
        .ToList();
}

Robot ParseRobot(string sRobot)
{
    var m = reRobot.Match(sRobot);
    if (!m.Success)
        throw new Exception("Nope.");
    var g = m.Groups;
    var x = int.Parse(g[1].ValueSpan);
    var y = int.Parse(g[2].ValueSpan);
    var vx = int.Parse(g[3].ValueSpan);
    var vy = int.Parse(g[4].ValueSpan);
    return new Robot(new(x, y), new(vx, vy));
}

static void RenderCompact(ProblemState state)
{
    var places = state.Robots.Select(r => r.Position).ToHashSet();
    for (int y = 0; y < state.Height; y += 2)
    {
        for (int x = 0; x < state.Width; x += 2)
        {
            var pos = new Position(x, y);
            int n = 0;
            if (places.Contains(new(x, y)))
                ++n;
            if (places.Contains(new(x+1, y)))
                ++n;
            if (places.Contains(new(x, y+1)))
                ++n;
            if (places.Contains(new(x+1, y+1)))
                ++n;
            char ch = n switch
            {
                1 => ',',
                2 => ':',
                3 => '+',
                4 => '*',
                _ => '.'
            };
            Console.Write(ch);
        }
        Console.WriteLine();
    }

}
static void Render(ProblemState state)
{
//    Console.Clear();
    var places = state.Robots.Select(r => r.Position).ToHashSet();
    for (int y = 0; y < state.Height; ++y)
    {
        for (int x = 0; x < state.Width; ++x)
        {
            var pos = new Position(x, y);
            char ch = places.Contains(pos)
                ? '*'
                : '.';
            Console.Write(ch);
        }
        Console.WriteLine();
    }
}

public class ProblemState
{
    public int Width { get; set; }
    public int Height { get; set; }

    public List<Robot> Robots { get; set; } = [];

    internal void Advance()
    {
        for (int i = 0; i < Robots.Count; ++i)
        {
            var r = Robots[i];
            var xNew = ((r.Position.X + r.Direction.X) + Width) % Width;
            var yNew = ((r.Position.Y + r.Direction.Y) + Height) % Height;
            r.Position = new(xNew, yNew);
        }
    }

    public int SafetyFactor()
    {
        var halfWidth = this.Width / 2;
        var halfHeight = this.Height / 2;
        int safetyFactor = 1;
        safetyFactor *= QuadrantScore(0,                 0,                   halfWidth, halfHeight);
        safetyFactor *= QuadrantScore(Width - halfWidth, 0,                   halfWidth, halfHeight);
        safetyFactor *= QuadrantScore(0,                 Height - halfHeight, halfWidth, halfHeight);
        safetyFactor *= QuadrantScore(Width - halfWidth, Height - halfHeight, halfWidth, halfHeight);
        return safetyFactor;
    }

    private int QuadrantScore(int x, int y, int cx, int cy)
    {
        int score = 0;
        foreach (var robot in Robots)
        {
            if (x <= robot.Position.X && robot.Position.X < x + cx &&
                y <= robot.Position.Y && robot.Position.Y < y + cy)
            {
                ++score;
            }
        }
        return score;
    }
}

public class Robot(Position Position, Vector Direction)
{
    public Position Position { get; set; } = Position;
    public Vector Direction { get; set; } = Direction;
}

public readonly record struct Position(int X, int Y)
{
    public static Position operator +(Position pos, Vector dir)
    {
        return new Position(pos.X + dir.X, pos.Y + dir.Y);
    }
}

public readonly record struct Vector(int X, int Y);
