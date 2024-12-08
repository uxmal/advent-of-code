namespace Advent2024.Day08;

public record ProblemState(
    Arena Arena,
    Dictionary<char, List<Position>> Antennae,
    Dictionary<Position, char> AntennaPositions)
{
    public HashSet<Position> Antinodes { get; } = [];

    public void Render()
    {
        Console.Clear();
        for (int y = 0; y < Arena.Height; ++y)
        {
            for (int x = 0; x < Arena.Width; ++x)
            {
                var pos = new Position(x, y);
                char ch;
                if (this.Antinodes.Contains(pos))
                {
                    ch = '#';
                }
                else if (!AntennaPositions.TryGetValue(pos, out ch))
                {
                    ch = '.';
                }
                Console.Write(ch);
            }
            Console.WriteLine();
        }
    }
}
