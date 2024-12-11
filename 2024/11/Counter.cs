public class Counter<T>
{
    private readonly Dictionary<T, long> tallies;
    public Counter(IEnumerable<T> items)
    {
        this.tallies = [];
        foreach (var item in items)
        {
            Add(item);
        }
    }

    public Counter()
    {
        this.tallies = [];
    }

    public void Add(T item)
    {
        Add(item, 1);
    }

    public long Add(T item, long count)
    {
        if (!tallies.TryGetValue(item, out long tally))
        {
            tally = 0;
        }
        tally += count;
        tallies[item] = tally;
        return tally;
    }

    public IEnumerable<KeyValuePair<T, long>> GetTallies()
    {
        return tallies;
    }
}