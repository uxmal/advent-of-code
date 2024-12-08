namespace Advent2024.Day08;

public record Arena(int Width, int Height)
{
    public bool IsInBounds(Position p) => IsInBounds(p.X, p.Y);

    public bool IsInBounds(int x, int y)
    {
        return 0 <= x && x < Width &&
               0 <= y && y < Height;
    }
}