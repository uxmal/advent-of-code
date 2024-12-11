public class Component
{
    public Component(string[] inputs, string output, Func<Component, ProblemState, ushort?> compute)
    {
        this.Inputs = inputs;
        this.Output = output;
        this.doCompute = compute; 
    }

    public string[] Inputs { get; }
    public string Output { get; }
    private readonly Func<Component, ProblemState, ushort?> doCompute;

    public ushort? Compute(ProblemState state)
    {
        return doCompute(this, state);
    }
}
