using System.Diagnostics;

var arena = ReadArena(args[0]);
var regionDict = CreateEmptyDict(arena);
var regions = FindRegions(arena, regionDict);
var distinctRegions = regions.GroupBy(r => r.Representative)
    .Select(e => e.Key)
    .ToList();
CountCorners(arena, regionDict);

foreach (var region in distinctRegions.OrderBy(r => r.ch))
{
//    Console.WriteLine($"{region.ch}: a {region.area,4}; p {region.perimeter,4}; c {region.corners}");
}

var sw = Stopwatch.StartNew();
var totalPerimeterPrice = distinctRegions.Sum(r => r.PerimeterPrice);
var msPart1 = sw.Elapsed.Microseconds;
sw.Restart();
var totalSidePrice = distinctRegions.Sum(r => r.SidePrice);
var msPart2= sw.Elapsed.Microseconds;
Console.WriteLine("Total perimeter price: {0}", totalPerimeterPrice);
Console.WriteLine("Total side price: {0}", totalSidePrice);
Console.WriteLine("Part 1: {0} usec, Part 2: {1} usec", msPart1, msPart2);

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

static int IsCorner(Dictionary<Position, Region> regions, Region c,  int x, int y, int dx, int dy)
{
    var cx = regions[new(x+dx,y)].FindRep();
    var cy = regions[new(x,y+dy)].FindRep();
    var cxy = regions[new(x+dx,y+dy)].FindRep();
    if (cx != c && cy != c)
        return 1;
    return (cx == c && cy == c && cxy != c) ? 1 : 0;
}

static void CountCorners(Arena arena, Dictionary<Position, Region> regions)
{
    for (int y = 0; y < arena.Height; ++y)
    {
        for (int x = 0; x < arena.Width; ++x)
        {
            char ch = arena[x,y];
            var region = regions[new(x,y)].FindRep();
            var ul = IsCorner(regions, region, x, y, -1, -1);
            var ur = IsCorner(regions, region, x, y, 1, -1);
            var ll = IsCorner(regions, region, x, y, -1, 1);
            var lr = IsCorner(regions, region, x, y, 1, 1);
            region.corners += ul + ur + ll + lr;
        }
    }
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
    for (int y = -1; y <= arena.Height; ++y)
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
    public int corners;

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

    public long PerimeterPrice => perimeter * area;
    public long SidePrice => corners * area;
}

record struct Position(int X, int Y);