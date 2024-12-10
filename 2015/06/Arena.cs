public class Arena
{
    private readonly int[,] lights;

    public int Height { get; }
    public int Width { get; }

    public Arena(int width, int height)
    {
        this.lights = new int[width, height];
        this.Height = height;
        this.Width = width;
    }

    public void TurnOn(Position a, Position b)
    {
        UpdateLights(a, b, b => b + 1);
    }

    public void TurnOff(Position a, Position b)
    {
        UpdateLights(a, b, b => --b >= 0 ? b : 0);
    }

   public void Toggle(Position a, Position b)
    {
        UpdateLights(a, b, b => b + 2);
    }

    private void UpdateLights(Position a, Position b, Func<int, int> fn)
    {
        Normalize(ref a, ref b);
        for (int y = a.Y; y <= b.Y; ++y)
        {
            for (int x = a.X; x <= b.X; ++x)
            {
                lights[y, x] = fn(lights[y, x]);
            }
        }
    }

    private static void Normalize(ref Position a, ref Position b)
    {
        var xMin = Math.Min(a.X, a.X);
        var xMax = Math.Max(a.X, b.X);
        var yMin = Math.Min(a.Y, a.Y);
        var yMax = Math.Max(a.Y, b.Y);
        a = new(xMin, yMin);
        b = new(xMax, yMax);
    }

    public long CountLights()
    {
        long total = 0;
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                total += lights[y, x];
            }
        }
        return total;
    }

    public void Clear()
    {
        TurnOff(new(0, 0), new(Width-1, Height-1));
    }

    public void Render(TextWriter @out)
    {
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                @out.Write(lights[y, x] != 0 ? "O" : ".");
            }
            @out.WriteLine();
        }
    }
}

public record struct Position(int X, int Y);