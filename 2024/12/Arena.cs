class Arena
{
    private readonly string[] lines;

    public int Width { get; }
    public int Height { get; }

    public Arena(string[] lines)
    {
        this.lines = lines;
        this.Width = lines[0].Length;
        this.Height = lines.Length;
    }

    public char this[int x, int y]
    {
        get
        {
            if (0 <= x && x < Width &&
                0 <= y && y < Height)
            {
                return lines[y][x];
            }
            else
                return '\u8080';
        }
    }
}