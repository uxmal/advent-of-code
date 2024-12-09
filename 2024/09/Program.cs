using System.Diagnostics;

var diskimage = ReadDiskimage(args[0]);

var sw = Stopwatch.StartNew();
// Render(diskimage);
Compact(diskimage);
long checksum = Checksum(diskimage);
Console.WriteLine($"Checksum: {checksum} ({sw.ElapsedMilliseconds}ms)");

static void Render(int[] diskimage)
{
    foreach (var id in diskimage)
    {
        if (id < 0)
            Console.Write('.');
        else if (id <= 9)
            Console.Write((char)(id + '0'));
        else 
            throw new InvalidOperationException($"Unexpected disk ID {id}");
    }
    Console.WriteLine();
}

static void Compact(int[] diskimage)
{
    int iLeft = 0;
    int iRight = diskimage.Length-1;
    while (iLeft < iRight)
    {
        while (diskimage[iLeft] >= 0)
        {
            ++iLeft;
            continue;
        }
        while (diskimage[iRight] < 0)
        {
            --iRight;
            continue;
        }
        diskimage[iLeft] = diskimage[iRight];
        diskimage[iRight] = -1;
        ++iLeft;
        --iRight;
        // Render(diskimage);
    }
}

static long Checksum(int[] diskimage)
{
    long result = 0;
    for (int i = 0; i < diskimage.Length; ++i)
    {
        int fileId = diskimage[i];
        if (fileId < 0)
            continue;
        result += i * fileId;
    }
    return result;
}

static int[] ReadDiskimage(string filename)
{
    using var rdr = File.OpenText(filename);
    var result = new List<int>();
    bool isGap = false;
    int fileId = 0;
    for (;;)
    {
        int c = rdr.Read();
        if (c < 0)
            break;
        char ch = (char) c;
        if (!char.IsAsciiDigit(ch))
            throw new InvalidDataException($"Unexpected character '{ch}' (U+{c:X4})");
        int count = ch - '0';
        if (isGap)
        {
            AppendMultiple(-1, count, result);
            isGap = !isGap;
        }
        else 
        {
            Debug.Assert(count > 0);
            AppendMultiple(fileId, count, result);
            ++fileId;
            isGap = !isGap;
        }
    }
    return result.ToArray();
} 

static void AppendMultiple(int diskId, int count, List<int> result)
{
    for (int i = 0; i < count; ++i)
    {
        result.Add(diskId);
    }
}