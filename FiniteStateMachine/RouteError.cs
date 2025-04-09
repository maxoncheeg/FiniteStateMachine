using FiniteStateMachine.Abstract;

namespace FiniteStateMachine;

public class RouteError : IRouteError
{
    public int Position { get; init; }
    public string Result { get; init; } = string.Empty;
    public string StartState { get; init; } = string.Empty;
    public string EndState { get; init; } = string.Empty;
    public string Route { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
}