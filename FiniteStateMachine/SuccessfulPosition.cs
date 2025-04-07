using FiniteStateMachine.Abstract;

namespace FiniteStateMachine;

public class SuccessfulPosition(IRoute route, int index) : ISuccessfulPosition
{
    public IRoute Route { get; } = route;
    public int Index { get; } = index;
}