public class Graph
{
    private readonly Dictionary<Position, List<Position>> successors = [];
    private readonly Dictionary<Position, List<Position>> predecessors = [];

    public Graph()
    {
    }

    public HashSet<Position> Sources {get;} = [];
    public HashSet<Position> Sinks {get;} = [];
    
    public void AddEdge(Position from, Position to)
    {
        if (!successors.TryGetValue(from, out var succs))
        {
            succs = [];
            successors.Add(from, succs);
        }
        succs.Add(to);
        if (!predecessors.TryGetValue(to, out var preds))
        {
            preds = [];
            predecessors.Add(to, preds);
        }
        preds.Add(from);
    }

    public IEnumerable<Position> Succ(Position pos)
    {
        if (!successors.TryGetValue(pos, out var ss))
            return Array.Empty<Position>();
        return ss;
    }

    public IEnumerable<Position> Pred(Position pos)
    {
        if (!predecessors.TryGetValue(pos, out var pp))
            return Array.Empty<Position>();
        return pp;
    }
}