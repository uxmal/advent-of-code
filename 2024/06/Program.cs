using System.Diagnostics;

if (args.Length < 1)
    return;
int iFile = 0;
bool interactive = false;
if (args[0] == "-i")
{
    interactive = true;
    ++iFile;
}
var map = ReadLaboratoryMap(args[iFile]);
if (interactive)
{
    Console.Clear();
    map.Render();
    while (Console.ReadKey(true).KeyChar != 'q')
    {
        Thread.Sleep(7);
        if (!map.AdvanceGuard())
            break;
        Console.Clear();
        map.Render();
    }
}
else
{
    while (map.AdvanceGuard())
        ;
}
Console.Write($"Visited positions: {map.Visited.Count}");


static LaboratoryMap ReadLaboratoryMap(string filename)
{
    using StreamReader rdr = File.OpenText(filename);
    int y = 0;
    string? previousLine = null;
    Direction? dir = null;
    Position? pos = null;
    HashSet<Position> obstructions = [];
    int width = 0;
    for (; ; )
    {
        string? line = rdr.ReadLine();
        if (line is null)
            break;
        if (previousLine is not null && line.Length != previousLine.Length)
            throw new ApplicationException("Row length don't match.");
        previousLine = line;
        width = line.Length;
        for (int x = 0; x < line.Length; ++x)
        {
            switch (line[x])
            {
                case '^':
                    pos = new Position(x, y); dir = new Direction(0, -1); break;
                case '>':
                    pos = new Position(x, y); dir = new Direction(1, 0); break;
                case 'v':
                    pos = new Position(x, y); dir = new Direction(0, 1); break;
                case '<':
                    pos = new Position(x, y); dir = new Direction(-1, 0); break;
                case '#':
                    obstructions.Add(new Position(x, y)); break;
                case '.':
                    break;
                default:
                    throw new InvalidDataException($"Unexpected character '{line[x]}' in map.");
            }
        }
        ++y;
    }
    if (dir is null || pos is null)
        throw new InvalidDataException("The player position wasn't found.");
    if (width == 0 || y == 0)
        throw new InvalidDataException("The size of the arena couldn't be determined.");

    return new LaboratoryMap(width, y, obstructions, pos.Value, dir.Value);
}


record struct Position(int X, int Y)
{
    public Position Move(in Direction dir)
    {
        return new Position(this.X + dir.Dx, this.Y + dir.Dy);
    }
}

record struct Direction(int Dx, int Dy)
{
    public Direction RotateRight()
    {
        var dx = -this.Dy;
        var dy = this.Dx;
        return new Direction(dx, dy);
    }
}

class LaboratoryMap
{
    private readonly int arenaWidth;
    private readonly int arenaHeight;
    private readonly HashSet<Position> obstructions;
    private Position guardPos;
    private Direction guardDir;

    public LaboratoryMap(
        int arenaWidth,
        int arenaHeight,
        HashSet<Position> obstructions,
        Position guardPosition,
        Direction guardDirection)
    {
        this.arenaHeight = arenaHeight;
        this.arenaWidth = arenaWidth;
        this.obstructions = obstructions;
        this.guardPos = guardPosition;
        this.guardDir = guardDirection;
        this.Visited = [guardPos];
    }

    public HashSet<Position> Visited { get; }

    public void Render()
    {
        for (int y = 0; y < arenaHeight; ++y)
        {
            for (int x = 0; x < arenaWidth; ++x)
            {
                var p = new Position(x, y);
                if (this.guardPos == p)
                    Console.Write(SelectGuardGlyph());
                else if (obstructions.Contains(p))
                    Console.Write('#');
                else if (Visited.Contains(p))
                    Console.Write("X");
                else
                    Console.Write('.');
            }
            Console.WriteLine();
        }
        Console.WriteLine($"Dir: ({guardDir.Dx},{guardDir.Dy})");
    }

    public bool AdvanceGuard()
    {
        var nextPos = guardPos.Move(guardDir);
        if (!obstructions.Contains(nextPos))
        {
            guardPos = nextPos;
            if (!IsGuardInBounds())
                return false;
            Visited.Add(nextPos);
            return true;
        }
        this.guardDir = guardDir.RotateRight();
        return true;
    }

    private bool IsGuardInBounds()
    {
        return
            (0 <= guardPos.X && guardPos.X < arenaWidth) &&
            (0 <= guardPos.Y && guardPos.Y < arenaHeight);
    }

    private char SelectGuardGlyph()
    {
        var d = this.guardDir;
        Debug.Assert(Math.Abs(d.Dx) + Math.Abs(d.Dy) == 1);
        if (d.Dx == 0)
        {
            if (d.Dy == 1)
                return 'v';
            else
                return '^';
        }
        else if (d.Dx == 1)
        {
            return '>';
        }
        else
        {
            return '<';
        }
    }
}
