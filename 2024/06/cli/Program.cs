using System.Diagnostics;
using Advent2024.Day06.Core;


if (args.Length < 1)
    return;
var vm = ArgsParser.EatArgs(args);

var console = new ConsoleRenderDevice();
var visited = CountVisited(vm.Interactive, new(vm.State!), console);
CountLoopingExtraObstacles(visited, vm.Interactive, vm.State!, console);

static void CountLoopingExtraObstacles(HashSet<Position> path, bool interactive, LaboratoryState startState, IRenderDevice device)
{
    HashSet<Position> loopingPositions = [];
    foreach (var pos in path.Where(p => p != startState.GuardPosition))
    {
        var state = new LaboratoryState(startState);
        state.ExtraObstruction = pos;
        var result = MoveGuard(state, device);
        if (result == GuardState.StuckInLoop)
        {
            loopingPositions.Add(pos);
            if (interactive)
            {
                device.Clear();
                state.Render(device);
                device.WriteStatusLine($"Looping positions: {loopingPositions.Count}");
                if (device.ReadKey() == 'q')
                    break;
            }
        }
    }
    device.WriteStatusLine($"Looping positions: {loopingPositions.Count}");
}

static GuardState MoveGuard(LaboratoryState state, IRenderDevice device)
{
    for (; ; )
    {
        var guardState = state.AdvanceGuard();
        if (guardState != GuardState.Moving)
            return guardState;
    }
}

static HashSet<Position> CountVisited(bool interactive, LaboratoryState state, IRenderDevice device)
{
    if (interactive)
    {
        device.Clear();
        state.Render(device);
        while (device.ReadKey() != 'q')
        {
            Thread.Sleep(7);
            if (state.AdvanceGuard() != GuardState.Moving)
                break;
            device.Clear();
            state.Render(device);
        }
    }
    else
    {
        while (state.AdvanceGuard() == GuardState.Moving)
            ;
    }
    var places = state.Visited.Select(x => x.Item1).ToHashSet();
    device.WriteStatusLine($"Visited positions: {places.Count}");
    return places;
}


