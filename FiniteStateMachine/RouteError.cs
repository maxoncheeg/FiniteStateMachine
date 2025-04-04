using FiniteStateMachine.Abstract;

namespace FiniteStateMachine;

public class RouteError : IRouteError
{
    public required int Position { get; init; }
    public required int StartIndex { get; init; }
    public required int Length { get; init; }
    public required string StartState { get; init; }
    public required string EndState { get; init; }
    public string Text { get; init; } = string.Empty;
}