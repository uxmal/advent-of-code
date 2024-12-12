using System.Diagnostics;
using System.Net;

var arena = ReadArena(args[0]);
var empty = CreateEmptyDict(arena);
var regions = FindRegions(arena, empty);
var distinctRegions = regions.GroupBy(r => r.Representative)
    .Select(e => e.Key)
    .ToList();
foreach (var region in distinctRegions.OrderBy(r => r.ch))
{
  //  Console.WriteLine($"{region.ch}: area {region.area,8} perimeter {region.perimeter,8}");
}

var totalPrice = distinctRegions.Sum(r => r.Price);
Console.WriteLine("Total price: {0}", totalPrice);

static Arena ReadArena(string filename)
{
    var arena = File.ReadAllLines(filename)
        .Where(a => !a.StartsWith('#'))
        .ToArray();
    Debug.Assert(arena.All(a => a.Length == arena[0].Length));
    return new Arena(arena);
}


static HashSet<Region> FindRegions(Arena arena, Dictionary<Position, Region> regions)
{
    HashSet<Region> result = [];
    for (int y = 0; y < arena.Height; ++y)
    {
        Region? r = null;
        for (int x = 0; x < arena.Width; ++x)
        {
            char ch = arena[x,y];
            var above = regions[new(x,y-1)].FindRep();
            var left = regions[new(x-1, y)].FindRep();
            r = DetermineRegion(ch, r, above, left, regions, result).FindRep();
            ++r.area;
            if (above.ch != r.ch)
                ++r.perimeter;
            if (left.ch != r.ch)
                ++r.perimeter;
            if (arena[x+1,y] != ch)
                ++r.perimeter;
            if (arena[x,y+1] != ch)
                ++r.perimeter;
            regions[new(x,y)] = r;
            result.Add(r);
        }
    }
    return result;
}

static Region DetermineRegion(char ch, Region? current, Region above, Region left, Dictionary<Position, Region> regions, HashSet<Region> result)
{
    if (ch != above.ch)
    {
        if (current is not null && ch == left.ch)
        {
            return current;
        }
        else 
            return new Region(ch);
    }
    if (ch != left.ch || current is null)
    {
        return above;
    }
    if (current != above)
    {
        result.Remove(current);
        return above.UnionInto(current);
    }
    return current;
}

static Dictionary<Position, Region> CreateEmptyDict(Arena arena)
{
    Dictionary<Position, Region> result = [];
    var dummyRegion = new Region('\u8080');
    for (int y = 0; y < arena.Height; ++y)
    {
        result.Add(new(-1, y), dummyRegion);
        result.Add(new(arena.Width, y), dummyRegion);
    }
    for (int x = 0; x < arena.Width; ++x)
    {
        result.Add(new(x, -1), dummyRegion);
        result.Add(new(x, arena.Height), dummyRegion);
    }
    return result;
}


class Region
{
    public Region Representative {get; private set;}

    public readonly char ch;
    public long area;
    public long perimeter;

    public Region(char c)
    {
        this.ch = c;
        this.Representative = this;
    }

    public Region FindRep()
    {
        var rep = this.Representative;
        while (rep.Representative != rep)
        {
            rep = rep.Representative;
        }
        return rep;
    }

    public Region UnionInto(Region newMember)
    {
        var rep1 = newMember.FindRep();
        var rep2 = this.FindRep();
        rep2.perimeter += rep1.perimeter;
        rep2.area += rep1.area;
        newMember.Representative = rep2;
        this.Representative = rep2;
        return rep2;
    }

    public long Price => perimeter * area;
}

record struct Position(int X, int Y);