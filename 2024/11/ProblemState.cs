using System.Numerics;

public class ProblemState
{
    private int cBlinks;
    private Counter<long> tallies;

    public ProblemState(long[] values)
    {
        this.tallies = new(values);
    }


    private int CountDecimalDigits(long n)
    {
        return 1 + (int) Math.Floor(Math.Log10(n));
    }

    public void Blink()
    {
        Counter<long> newTallies = new();
        foreach (var (stone, count) in tallies.GetTallies())
        {
            if (stone == 0)
            {
                newTallies.Add(1, count);
                continue;
            }
            int digits = CountDecimalDigits(stone);
            if (digits % 2 == 0)
            {
                int halfDigits = digits / 2;
                long scale = (long)Math.Pow(10, halfDigits);
                long stone1 = stone / scale;
                long stone2 = stone % scale;
                newTallies.Add(stone1, count);
                newTallies.Add(stone2, count);
            }
            else
            {
                newTallies.Add(stone * 2024, count);
            }
        }
        this.tallies = newTallies;
        ++cBlinks;
    }

    public void Render()
    {
        // foreach (var (k, v) in tallies.GetTallies().OrderBy(c => c.Key))
        // {
        //     Console.WriteLine("   {0,18} {1,10}", k, v);
        // }
        Console.WriteLine($"{cBlinks,2} blink(s), {CountItems()}");
    }

    public BigInteger CountItems()
    {
        BigInteger sum = 0;
        foreach (var item in tallies.GetTallies())
        {
            sum += item.Value;
        }
        return sum;
    }
}

public class Memo
{
    public Memo(long value, params Memo[] submemos)
    {
        this.Value = value;
        if (submemos.Length == 0)
            this.Count = 1;
        else 
            this.Count = submemos.Sum(m => m.Count);
    }

    public int Count {get;}
    public long Value {get;}

    public override string ToString()
    {
        return Value.ToString();
    }
}