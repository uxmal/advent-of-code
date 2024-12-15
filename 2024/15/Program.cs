using System.Diagnostics;
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
    HashSet<Position> boxesLeft = [];
    HashSet<Position> boxesRight = [];
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
                    obstacles.Add(new(x*2, y));
                    obstacles.Add(new(x*2+1, y));
                    break;
                case 'O':
                    boxesLeft.Add(new(x*2, y));
                    boxesRight.Add(new(x*2+1, y));
                    break;
                case '@':
                    robot = new(x*2, y);
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
    return new ProblemState(width*2, height, robot.Value, boxesLeft, boxesRight, obstacles, movements);
}

public class ProblemState
{
    public int Width { get; }
    public int Height { get; }
    public Position Robot { get; set; }
    public HashSet<Position> BoxesLeft { get; set; }
    public HashSet<Position> BoxesRight { get; set; }
    public HashSet<Position> Obstacles { get; set; }

    public List<char> Movements { get; set; }

    public ProblemState(
        int width,
        int height,
        Position value,
        HashSet<Position> boxesLeft,
        HashSet<Position> boxesRight,
        HashSet<Position> obstacles,
        List<char> movements)
    {
        this.Width = width;
        this.Height = height;
        this.Robot = value;
        this.BoxesLeft = boxesLeft;
        this.BoxesRight = boxesRight;
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
                else if (this.BoxesLeft.Contains(pos))
                {
                    Console.Write('[');
                }
                else if (this.BoxesRight.Contains(pos))
                {
                    Console.Write(']');
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
        switch (m)
        {
            case '>': AdvanceHorizonal(Vector.E); break;
            case '<': AdvanceHorizonal(Vector.W); break;
            case '^': AdvanceVertical(Vector.N); break;
            case 'v': AdvanceVertical(Vector.S); break;
        };
    }

    private void AdvanceVertical(in Vector movement)
    {
        var newPos = this.Robot + movement;
        Position boxLeft;
        Position boxRight;
        if (Obstacles.Contains(newPos))
            return;
        if (BoxesLeft.Contains(newPos))
        {
            boxLeft = newPos;
            boxRight = new Position(newPos.X+1, newPos.Y);
            Debug.Assert(BoxesRight.Contains(boxRight));
        }
        else if (BoxesRight.Contains(newPos))
        {
            boxRight = newPos;
            boxLeft  = new Position(newPos.X-1, newPos.Y);
            Debug.Assert(BoxesLeft.Contains(boxLeft));
        }
        else 
        {
            this.Robot = newPos;
            return;
        }
            var (leftBoxes, rightBoxes) = FindMoveableVerticalBoulders(boxLeft, boxRight, movement);
            if (leftBoxes.Count == 0)
                return;
            MoveBoulders(leftBoxes, rightBoxes, movement);
            this.Robot = newPos;
    }

    private (HashSet<Position>, HashSet<Position>) FindMoveableVerticalBoulders(Position boxLeft, Position boxRight, Vector movement)
    {
        var leftBoxes = new HashSet<Position> { boxLeft };
        var rightBoxes = new HashSet<Position> { boxRight };

        var front = new HashSet<Position> { boxLeft, boxRight };
        var newFront = new HashSet<Position>(); 

        for (; ;)
        {
            foreach (var box in front)
            {
                if (BoxesLeft.Contains(box))
                {
                    var rbox = new Position(box.X+1, box.Y);
                    Debug.Assert(BoxesRight.Contains(rbox));
                    leftBoxes.Add(box);
                    rightBoxes.Add(rbox);
                    newFront.Add(box+movement);
                    newFront.Add(rbox+movement);
                }
                if (BoxesRight.Contains(box))
                {
                    var lbox = new Position(box.X-1, box.Y);
                    leftBoxes.Add(lbox);
                    rightBoxes.Add(box);
                    newFront.Add(lbox+movement);
                    newFront.Add(box+movement);
                }
                if (Obstacles.Contains(box))
                {
                    leftBoxes.Clear();
                    rightBoxes.Clear();
                    return (leftBoxes, rightBoxes);
                }
            }
            if (newFront.Count == 0)
                return (leftBoxes, rightBoxes);
            (front, newFront) = (newFront, front);
            newFront.Clear();
        }
    }

    private void AdvanceHorizonal(in Vector movement)
    {
        var newPos = this.Robot + movement;
        if (Obstacles.Contains(newPos))
            return;
        if (BoxesLeft.Contains(newPos) || BoxesRight.Contains(newPos))
        {
            var (leftBoxes, rightBoxes) = FindMoveableHorizontalBoulders(newPos, movement);
            if (leftBoxes.Count == 0)
                return;
            MoveBoulders(leftBoxes, rightBoxes, movement);
        }
        this.Robot = newPos;
    }

    private void MoveBoulders(ICollection<Position> leftBoxes, ICollection<Position> rightBoxes, Vector movement)
    {
        foreach (var box in leftBoxes)
        {
            BoxesLeft.Remove(box);
        }
        foreach (var box in rightBoxes)
        {
            BoxesRight.Remove(box);
        }
        foreach (var box in leftBoxes)
        {
            var newPos = box + movement;
            BoxesLeft.Add(newPos);
        }
        foreach (var box in rightBoxes)
        {
            var newPos = box + movement;
            BoxesRight.Add(newPos);
        }
    }

    private (List<Position>,List<Position>) FindMoveableHorizontalBoulders(Position pos, Vector movement)
    {
        var leftBoxes = new List<Position>();
        var rightBoxes = new List<Position>();
        for (; ; pos += movement)
        {
            if (BoxesLeft.Contains(pos))
            {
                leftBoxes.Add(pos);
                continue;
            }
            if (BoxesRight.Contains(pos))
            {
                rightBoxes.Add(pos);
                continue;
            }
            if (Obstacles.Contains(pos))
            {
                leftBoxes.Clear();
                rightBoxes.Clear();
                return (leftBoxes, rightBoxes);
            }
            // Empty gap!
            return (leftBoxes, rightBoxes);
        }
    }

    public int ComputeGpsScore()
    {
        return BoxesLeft.Sum(b => b.Y*100 + b.X);
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