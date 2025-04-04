namespace FiniteStateMachine.Abstract;

public interface IRouteError
{
    public int Position { get; init; }
    public int StartIndex { get; init; }
    public int Length { get; init; }
    public string StartState { get; init; }
    public string EndState { get; init; }
    public string Text { get; }
}