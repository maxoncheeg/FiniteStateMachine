namespace FiniteStateMachine.Abstract;

public abstract class AbstractRoute(string startState, string endState) : IRoute
{
    public RouteState State { get; protected set; } = RouteState.NotStarted;
    public string StartState { get; } = startState;
    public string EndState { get; } = endState;
    public string ErrorMessage { get; set; } = string.Empty;

    public int Priority { get; set; } = 0;

    public abstract char PutChar(char symbol);

    public virtual void Reset()
    {
        State = RouteState.NotStarted;
    }
}