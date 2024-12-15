using System.Text;

int iArg = 0;
bool interactive = false;
if (args[iArg] == "-i")
{
    interactive = true;
    ++iArg;
}
var problemState = LoadProblemState(args[iArg]);
foreach (var movement in problemState.Movements)
{
    if (interactive)
    {
        problemState.Render();
        Console.WriteLine(movement);
        Console.ReadLine();
    }
    problemState.Advance(movement);
}
problemState.Render();
int gpsScore = problemState.ComputeGpsScore();
Console.WriteLine("GPS score: {0}", gpsScore);

static ProblemState LoadProblemState(string filename)
{
    int y = 0;
    HashSet<Position> obstacles = [];
    HashSet<Position> boulders = [];
    Position? robot = null;

    using var rdr = File.OpenText(filename);
    int width = 0;
    int height = 0;
    for (; ; )
    {
        string? line = rdr.ReadLine();
        if (line is null || line.Length == 0)
            break;
        int x = 0;
        if (width != 0)
        {
            if (line.Length != width)
                throw new InvalidDataException();
        }
        else
        {
            width = line.Length;
        }
        ++height;

        foreach (char ch in line)
        {
            switch (ch)
            {
                case '#':
                    obstacles.Add(new(x, y));
                    break;
                case 'O':
                    boulders.Add(new(x, y));
                    break;
                case '@':
                    robot = new(x, y);
                    break;
            }
            ++x;
        }
        ++y;
    }
    if (robot is null)
        throw new InvalidDataException();

    List<char> movements = [];
    for (; ; )
    {
        int c = rdr.Read();
        if (c < 0)
            break;
        if ("<>^v".Contains((char)c))
        {
            movements.Add((char)c);
        }
    }
    return new ProblemState(width, height, robot.Value, boulders, obstacles, movements);
}

public class ProblemState
{
    public int Width { get; }
    public int Height { get; }
    public Position Robot { get; set; }
    public HashSet<Position> Boulders { get; set; }
    public HashSet<Position> Obstacles { get; set; }

    public List<char> Movements { get; set; }

    public ProblemState(
        int width,
        int height,
        Position value,
        HashSet<Position> boulders,
        HashSet<Position> obstacles,
        List<char> movements)
    {
        this.Width = width;
        this.Height = height;
        this.Robot = value;
        this.Boulders = boulders;
        this.Obstacles = obstacles;
        this.Movements = movements;
    }

    public void Render()
    {
        StringBuilder sb = new();
        for (int y = 0; y < this.Height; ++y)
        {
            for (int x = 0; x < this.Width; ++x)
            {
                var pos = new Position(x, y);
                if (this.Robot == pos)
                {
                    Console.Write('@');
                }
                else if (this.Boulders.Contains(pos))
                {
                    Console.Write('O');
                }
                else if (this.Obstacles.Contains(pos))
                {
                    Console.Write('#');
                }
                else
                {
                    Console.Write('.');
                }
            }
            Console.WriteLine(sb);
            sb.Clear();
        }
    }

    public void Advance(char m)
    {
        var movement = m switch
        {
            '^' => Vector.N,
            '>' => Vector.E,
            'v' => Vector.S,
            '<' => Vector.W,
            _ => throw new InvalidDataException()
        };

        var newPos = this.Robot + movement;
        if (Obstacles.Contains(newPos))
            return;
        if (Boulders.Contains(newPos))
        {
            var moveableBoulders = FindMoveableBoulders(newPos, movement);
            if (moveableBoulders.Count == 0)
                return;
            MoveBoulders(moveableBoulders, movement);
        }
        this.Robot = newPos;
    }

    private void MoveBoulders(List<Position> moveableBoulders, Vector movement)
    {
        foreach (var boulder in moveableBoulders)
        {
            Boulders.Remove(boulder);
        }
        foreach (var boulder in moveableBoulders)
        {
            var newPos = boulder + movement;
            Boulders.Add(newPos);
        }
    }

    private List<Position> FindMoveableBoulders(Position pos, Vector movement)
    {
        var result = new List<Position>();
        for (; ; pos += movement)
        {
            if (Boulders.Contains(pos))
            {
                result.Add(pos);
                continue;
            }
            if (Obstacles.Contains(pos))
            {
                result.Clear();
                return result;
            }
            // Empty gap!
            return result;
        }
    }

    public int ComputeGpsScore()
    {
        return Boulders.Sum(b => b.Y*100 + b.X);
    }
}

public readonly record struct Position(int X, int Y)
{
    public static Position operator +(Position pos, Vector vec)
    {
        return new(pos.X + vec.X, pos.Y + vec.Y);
    }

}

public record struct Vector(int X, int Y)
{
    public static readonly Vector N = new(0, -1);
    public static readonly Vector E = new(1, 0);
    public static readonly Vector S = new(0, 1);
    public static readonly Vector W = new(-1, 0);
}