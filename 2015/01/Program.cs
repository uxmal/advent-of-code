var instrs = File.ReadAllLines(args[0]).First();
int floor = 0;
int pos = 0;
foreach (var c in instrs)
{
    switch (c)
    {
        case '(': ++floor; break;
        case ')': --floor; break;
        default: break;
    }
    ++pos;
    if (floor == -1)
        break;
}
Console.WriteLine($"Pos {pos}");
Console.WriteLine($"Floor {floor}");
