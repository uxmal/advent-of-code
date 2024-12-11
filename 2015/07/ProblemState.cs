
public class ProblemState
{
    public Dictionary<string, ushort> WireValues { get; } = [];
    public Dictionary<string, List<Component>> ComponentInputs { get; } = [];

    public void SetWireSignal(int value, string dst)
    {
        WireValues[dst] = (ushort)value;
    }

    public void CreateBinary(string op1, string op2, string dst, Func<ushort, ushort, int> compute)
    {
        var component = new Component([op1, op2], dst, (c, state) =>
        {
            var value0 = GetInput(c.Inputs[0], state);
            var value1 = GetInput(c.Inputs[1], state);
            if (value0 is null || value1 is null)
                return null;
            var newValue = (ushort)compute(value0.Value, value1.Value);
            return newValue;
        });
        AttachComponentToInput(component, op1);
        AttachComponentToInput(component, op2);
    }

    public void CreateShift(string op1, string op2, string dst, Func<ushort, int, int> compute)
    {
        int shift = int.Parse(op2);
        var component = new Component([op1], dst, (c, state) =>
        {
            if (!state.WireValues.TryGetValue(c.Inputs[0], out var value0))
                return null;
            var newValue = (ushort)compute(value0, shift);
            return newValue;
        });
        AttachComponentToInput(component, op1);
    }

    public void CreateUnary(string op, string dst, Func<ushort, int> compute)
    {
        var component = new Component([op], dst, (c, state) =>
        {
            if (!state.WireValues.TryGetValue(c.Inputs[0], out var value0))
                return null;
            var newValue = (ushort)compute(value0);
            return newValue;
        });
        AttachComponentToInput(component, op);
    }

    public void Propagate()
    {
        Queue<string> wires = [];
        foreach (var wire in this.WireValues.Keys)
        {
            wires.Enqueue(wire);
        }
        while (wires.TryDequeue(out string? wire))
        {
            if (!this.ComponentInputs.TryGetValue(wire, out var components))
                continue;
            foreach (var c in components)
            {
                var newValue = c.Compute(this);
                if (newValue is not null &&
                    (!WireValues.TryGetValue(c.Output, out ushort oldValue) ||
                     newValue != oldValue))
                {
                    WireValues[c.Output] = newValue.Value;
                    wires.Enqueue(c.Output);
                    Console.WriteLine($"Updating {c.Output} from [{string.Join(",", c.Inputs)}] with {newValue.Value}");
                }
            }
        }
    }

    public void Render(bool interactive)
    {
        if (interactive)
            Console.Clear();
        foreach (var (w, value) in this.WireValues.OrderBy(e => e.Key))
        {
            Console.WriteLine($"{w}: {value}");
        }
        if (interactive)
        {
            Console.ReadKey(true);
        }
    }

    private ushort? GetInput(string operand, ProblemState state)
    {
        if (int.TryParse(operand, out int n))
            return (ushort) n;
        if (state.WireValues.TryGetValue(operand, out var value))
            return value;
        return null;
    }

    private void AttachComponentToInput(Component c, string inputWire)
    {
        if (!ComponentInputs.TryGetValue(inputWire, out var components))
        {
            components = [];
            ComponentInputs.Add(inputWire, components);
        }
        components.Add(c);
    }
}