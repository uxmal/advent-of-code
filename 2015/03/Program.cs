using System.Numerics;
using System.Security;

var directions = ReadDirections(args[0]);
var visited = Walk(directions);
Console.WriteLine($"Houses visited: {visited.Count}");

HashSet<Position>  Walk(string directions)
{
    var walkers = new[]
    {
        new Position(0, 0),
        new Position(0, 0),
    };
    HashSet<Position> result = [ new(0, 0) ];
    int iWalker = 0;
    foreach (char ch in directions)
    {
        var pos = walkers[iWalker];
        Vector vec = ch switch
        {
            '>' => new(1, 0) ,
            'v' => new(0, 1) ,
            '<' => new(-1, 0) ,
            '^' => new(0, -1) ,
            _ => throw new InvalidDataException($"Invalid direction {ch}.")
        };
        pos += vec;
        result.Add(pos);
        walkers[iWalker] = pos;
        iWalker = 1 - iWalker;
    }
    return result;
}

static string ReadDirections(string filename)
{
    var lines = File.ReadAllLines(filename);
    return lines[0];
}

public record struct Position(int X, int Y)
{
    public static Position operator +(Position pos, Vector vec)
    {
        return new(pos.X+vec.X, pos.Y+vec.Y);
    }

        public static Position operator -(Position pos, Vector vec)
    {
        return new(pos.X-vec.X, pos.Y-vec.Y);
    }
}

public record struct Vector(int X, int Y);