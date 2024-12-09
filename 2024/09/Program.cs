using System.Diagnostics;
using System.Net.Sockets;

if (args.Length == 0)
    args = new[] { @"c:\dev\uxmal\advent\2024\09\testinput" };
// var diskimage = ReadDiskimage(args[0]);
var diskimage = ReadContiguousFiles(args[0]);

var sw = Stopwatch.StartNew();
RenderContiguous(diskimage);
CompactContiguous(diskimage);

long wut = Checksum(new int[] {0,0,9,9,2,1,1,1,7,7,7,-1,4,4,-1,3,3,3,-1,-1,-1,-1,5,5,5,5,-1,6,6,6,6,-1,-1,-1,-1,-1,8,8,8,8,-1,-1});

long checksum = ChecksumContiguous(diskimage);



Console.WriteLine($"Checksum: {checksum} ({sw.ElapsedMilliseconds}ms)");
Console.WriteLine($"Wut: {wut} ({sw.ElapsedMilliseconds}ms)");

void CompactContiguous(List<ContiguousFile> diskimage)
{
    int iLeft = 0;
    int iRight = diskimage.Count - 1;
    for (; ; )
    {
        ContiguousFile? file = null;
        while (iRight >= 0)
        {
            file = diskimage[iRight];
            if (file.DiskId != -1 && !file.Moved)
                break;
            --iRight;
        }
        if (file is null)
            return;
        ContiguousFile? gap = null;
        iLeft = 0;
        while (iLeft < diskimage.Count)
        {
            gap = diskimage[iLeft];
            if (gap.DiskId == -1 && gap.Length >= file.Length)
                break;
            ++iLeft;
        }
        if (gap is null || iLeft >= diskimage.Count || iLeft >= iRight)
        {
            // Can't find a contiguous block for the current file,
            // start over.
            --iRight;
            continue;
        }
        if (file.DiskId == 2)
            _ = iRight;
        if (iRight < 0)
            return;

        if (iRight > 0 && diskimage[iRight].DiskId == -1)
        {
            // Coalesce with previous gap.
            diskimage[iRight - 1].Length += file.Length;
            diskimage.RemoveAt(iRight);
            --iRight;
        }
        else
        {
            diskimage[iRight] = new(-1, file.Position, file.Length);
        }

        file.Position = gap.Position;
        if (file.Length == gap.Length)
        {
            diskimage[iLeft] = file;
        }
        else
        {
            diskimage.Insert(iLeft, file);
            gap.Length -= file.Length;
            gap.Position += file.Length;
        }
        file.Moved = true;
        RenderContiguous(diskimage);
    }
    throw new NotImplementedException();
}

long ChecksumContiguous(List<ContiguousFile> diskimage)
{
    long result = 0;
    foreach (var file in diskimage)
    {
        if (file.DiskId == -1)
            continue;
        for (int iPos = file.Position; iPos < file.Position + file.Length; ++iPos)
        {
            result += (iPos * file.DiskId);
        }
        // Console.WriteLine($"{new string(' ', file.Position)}{result} ({file.DiskId},{file.Length})");
    }
    return result;
}

static void RenderContiguous(List<ContiguousFile> diskimage)
{
    return;
    foreach (var file in diskimage)
    {
        char ch;
        if (file.DiskId < 0)
            ch = '.';
        else
            ch = (char)(file.DiskId % 10 + '0');
        for (int i = 0; i < file.Length; ++i)
        {
            Console.Write(ch);
        }
    }
    Console.WriteLine();
}

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
    int iRight = diskimage.Length - 1;
    while (iLeft < iRight)
    {
        while (diskimage[iLeft] >= 0)
        {
            ++iLeft;
        }
        while (diskimage[iRight] < 0)
        {
            --iRight;
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

static List<ContiguousFile> ReadContiguousFiles(string filename)
{
    using var rdr = File.OpenText(filename);
    var result = new List<ContiguousFile>();
    bool isGap = false;
    int iPos = 0;
    int fileId = 0;
    for (; ; )
    {
        int c = rdr.Read();
        if (c < 0)
            break;
        char ch = (char)c;
        if (!char.IsAsciiDigit(ch))
            throw new InvalidDataException($"Unexpected character '{ch}' (U+{c:X4})");
        int count = ch - '0';
        if (isGap)
        {
            if (count != 0)
                result.Add(new ContiguousFile(-1, iPos, count));
            isGap = !isGap;
        }
        else
        {
            Debug.Assert(count > 0);
            result.Add(new(fileId, iPos, count));
            ++fileId;
            isGap = !isGap;
        }
        iPos += count;
    }
    return result;
}

static int[] ReadDiskimage(string filename)
{
    using var rdr = File.OpenText(filename);
    var result = new List<int>();
    bool isGap = false;
    int fileId = 0;
    for (; ; )
    {
        int c = rdr.Read();
        if (c < 0)
            break;
        char ch = (char)c;
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

public class ContiguousFile
{
    public ContiguousFile(int DiskId, int Position, int Length)
    {
        this.DiskId = DiskId;
        this.Position = Position;
        this.Length = Length;
    }
    public int DiskId { get; }
    public int Position { get; set; }
    public int Length { get; set; }
    public bool Moved { get; set; }
}