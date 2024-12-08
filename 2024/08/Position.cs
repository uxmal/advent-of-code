namespace Advent2024.Day08;

public readonly record struct Position(int X, int Y)
{
    public static Position operator +(Position p, Vector v)
    {
        return new Position(p.X + v.X, p.Y + v.Y);
    }

    public static Vector operator -(Position a, Position b)
    {
        return new Vector(a.X - b.X, a.Y - b.Y);
    }

    public static Position operator -(Position p, Vector v)
    {
        return new Position(p.X - v.X, p.Y - v.Y);
    }

}