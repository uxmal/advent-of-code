using Advent2024.Day06.Core;

internal class ConsoleRenderDevice : IRenderDevice
{
    public ConsoleRenderDevice()
    {
    }

    public void BeginFrame()
    {
    }

    public void Clear()
    {
        Console.Clear();
    }

    public char ReadKey()
    {
        return Console.ReadKey(true).KeyChar;
    }

    public void WriteGlyph(char c)
    {
        Console.Write(c);
    }

    public void WriteLine()
    {
        Console.WriteLine();
    }

    public void WriteStatusLine(string s)
    {
        Console.WriteLine(s);
    }
}