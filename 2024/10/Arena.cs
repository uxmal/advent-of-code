public class Arena
{
    private readonly string[] lines;

    public int Height { get; }
    public int Width { get; }

    public Arena(string [] lines)
    {
        this.lines = lines;
        this.Height = lines.Length;
        this.Width = lines.Max(l => l.Length);
        if (lines.Any(l => l.Length != this.Width))
            throw new ArgumentException("Ragged arena.");
    }

    public int this[int x, int y]
    {
        get { 
            char ch = lines[y][x];
            if (char.IsAsciiDigit(ch))
                return ch - '0';
            return -100;
        }
    }

    internal bool IsInRange(Position to)
    {
        return
            0 <= to.X && to.X < this.Width &&
            0 <= to.Y && to.Y < this.Height;
    }
}