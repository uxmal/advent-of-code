public class Graph<N, E>
    where N : notnull
{
    private readonly Dictionary<N, List<Edge>> succs =[];
    private readonly Dictionary<N, List<Edge>> preds = [];
    
    public void AddEdge(N from, N to, E edgeData)
    {
        var edge = new Edge(from, to, edgeData);
        if (!succs.TryGetValue(from, out var s))
        {
            s = [];
            succs.Add(from, s);
        }
        s.Add(edge);
        if (!preds.TryGetValue(to, out var p))
        {
            p = [];
            preds.Add(to, p);
        }
        p.Add(edge);
    }

    private record Edge(N From, N To, E Data);
}