using System.Diagnostics;
using Advent2024.Day06.Core;


if (args.Length < 1)
    return;
var vm = ArgsParser.EatArgs(args);

var console = new ConsoleRenderDevice();
if (!vm.ExtraObstacle)
{
    CountVisited(vm.Interactive, vm.State!, console);
}
else
{
    CountLoopingExtraObstacles(vm.Interactive, vm.State!, console);
}


static void CountLoopingExtraObstacles(bool interactive, LaboratoryState startState, IRenderDevice device)
{
    HashSet<Position> loopingPositions = [];
    var slowMover = startState;
    for (; ; )
    {
        var state = TryPlaceObstruction(slowMover);
        if (state is null)
            break;
        var pos = state.ExtraObstruction;
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
        if (slowMover.AdvanceGuard() == GuardState.LeftLaboratory)
            break;
    }
    device.WriteStatusLine($"Looping positions: {loopingPositions.Count}");
}

static LaboratoryState? TryPlaceObstruction(LaboratoryState state)
{
    var probeState = new LaboratoryState(state);
    for (; ; )
    {
        var obstrPosition = probeState.GuardPosition.Move(probeState.GuardDirection);
        if (!probeState.IsObstructed(obstrPosition))
        {
            probeState.PlaceObstruction(obstrPosition);
            return probeState;
        }
        if (probeState.AdvanceGuard() != GuardState.Moving)
            return null;
    }
}

static GuardState MoveGuard(LaboratoryState state, IRenderDevice device)
{
    for (; ; )
    {
        var guardState = state.AdvanceGuard();
        device.Clear();
        state.Render(device);
        if (device.ReadKey() == 'q')
            return GuardState.LeftLaboratory;
        if (guardState != GuardState.Moving)
            return guardState;
    }
}

static void CountVisited(bool interactive, LaboratoryState state, IRenderDevice device)
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
    device.WriteStatusLine($"Visited positions: {state.Visited.Count}");
}


