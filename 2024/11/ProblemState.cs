public class ProblemState
{
    private int cBlinks;
    private Dictionary<long, Memo> memos;

    public ProblemState(long[] values)
    {
        this.memos = [];
        this.Values = values.Select(v => Memoize(v)).ToList();
    }

    public List<Memo> Values { get; private set; }

    private Memo Memoize(long v)
    {
        if (memos.TryGetValue(v, out var memo))
            return memo;
        memo = new Memo(v);
        memos.Add(v, memo);
        return memo;
    }

    private int CountDecimalDigits(long n)
    {
        return 1 + (int) Math.Floor(Math.Log10(n));
    }

    public void Blink()
    {
        List<Memo> newValues = [];
        foreach (var stone in Values)
        {
            if (stone.Value == 0)
            {
                newValues.Add(Memoize(1));
                continue;
            }
            int digits = CountDecimalDigits(stone.Value);
            if (digits % 2 == 0)
            {
                int halfDigits = digits / 2;
                long scale = (long)Math.Pow(10, halfDigits);
                long stone1 = stone.Value / scale;
                long stone2 = stone.Value % scale;
                newValues.Add(Memoize(stone1));
                newValues.Add(Memoize(stone2));
            }
            else
            {
                newValues.Add(Memoize(stone.Value * 2024));
            }
        }
        this.Values = newValues;
        ++cBlinks;
    }

    public void Render()
    {
        Console.Write($"{cBlinks,2} blink(s), {Values.Count}: [");
        var n = Math.Min(100, Values.Count);
        for (int i = 0; i < n; ++i)
        {
            Console.Write($" {Values[i]}");
        }
        Console.WriteLine(" ]");
    }

    public int CountItems()
    {
        return Values.Count;
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