namespace Advent2024.Day06.Core;

using System.Diagnostics;

public readonly record struct Position(int X, int Y)
{
    public readonly Position Move(in Direction dir)
    {
        return new Position(this.X + dir.Dx, this.Y + dir.Dy);
    }
}

public readonly record struct Direction(int Dx, int Dy)
{
    public Direction RotateRight()
    {
        var dx = -this.Dy;
        var dy = this.Dx;
        return new Direction(dx, dy);
    }
}

public class LaboratoryState
{
    private LaboratoryMap map;

    public LaboratoryState(LaboratoryMap map, Position guardPosition, Direction guardDirection)
    {
        this.map = map;
        this.GuardPosition = guardPosition;
        this.GuardDirection = guardDirection;
        this.ExtraObstruction = new Position(-1, -1);
        this.Visited = new() { { GuardPosition, guardDirection } };
    }

    public LaboratoryState(LaboratoryState that)
    {
        this.map = that.map;
        this.GuardPosition = that.GuardPosition;
        this.GuardDirection = that.GuardDirection;
        this.ExtraObstruction = that.ExtraObstruction;
        this.Visited = new Dictionary<Position, Direction>(that.Visited);
    }


    public Dictionary<Position, Direction> Visited { get; }
    public Position ExtraObstruction { get; set; }
    public Position GuardPosition { get; private set; }
    public Direction GuardDirection { get; private set; }

    public GuardState AdvanceGuard()
    {
        var nextPos = GuardPosition.Move(GuardDirection);
        if (!map.IsObstructed(nextPos) && nextPos != ExtraObstruction)
        {
            GuardPosition = nextPos;
            if (!map.IsInBounds(GuardPosition))
                return GuardState.LeftLaboratory;
            if (Visited.TryGetValue(GuardPosition, out var oldDir))
            {
                if (oldDir == GuardDirection)
                    return GuardState.StuckInLoop;
            }
            Visited[nextPos] = GuardDirection;
            return GuardState.Moving;
        }
        this.GuardDirection = GuardDirection.RotateRight();
        return GuardState.Moving;
    }

    public void Render(IRenderDevice device)
    {
        device.BeginFrame();
        for (int y = 0; y < map.ArenaHeight; ++y)
        {
            for (int x = 0; x < map.ArenaWidth; ++x)
            {
                var p = new Position(x, y);
                if (this.GuardPosition == p)
                    device.WriteGlyph(SelectGuardGlyph());
                else if (p == ExtraObstruction)
                    device.WriteGlyph('O');
                else if (map.IsObstructed(p))
                    device.WriteGlyph('#');
                else if (Visited.ContainsKey(p))
                    device.WriteGlyph('X');
                else
                    device.WriteGlyph('.');
            }
            device.WriteLine();
        }
        device.WriteStatusLine($"Dir: ({GuardDirection.Dx},{GuardDirection.Dy})");
    }


    private char SelectGuardGlyph()
    {
        var d = this.GuardDirection;
        System.Diagnostics.Debug.Assert(Math.Abs(d.Dx) + Math.Abs(d.Dy) == 1);
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

    public bool IsObstructed(Position position)
    {
        return map.IsObstructed(position);
    }

    public bool PlaceObstruction(Position position)
    {
        if (map.IsObstructed(position) || !map.IsInBounds(position))
            return false;
        this.ExtraObstruction = position;
        return true;
    }


    public static LaboratoryState ReadLaboratoryMap(string filename)
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

    var map = new LaboratoryMap(width, y, obstructions);
    var state = new LaboratoryState(map, pos.Value, dir.Value);
    return state;
}

}


public class LaboratoryMap
{
    private readonly HashSet<Position> obstructions;

    public LaboratoryMap(
        int arenaWidth,
        int arenaHeight,
        HashSet<Position> obstructions)
    {
        this.ArenaHeight = arenaHeight;
        this.ArenaWidth = arenaWidth;
        this.obstructions = obstructions;
    }

    public int ArenaWidth { get; }

    public int ArenaHeight { get; }

    public bool IsInBounds(Position pos)
    {
        return
            (0 <= pos.X && pos.X < ArenaWidth) &&
            (0 <= pos.Y && pos.Y < ArenaHeight);
    }


    public bool IsObstructed(Position p)
    {
        return obstructions.Contains(p);
    }





}

public enum GuardState
{
    Moving,
    LeftLaboratory,
    StuckInLoop
}