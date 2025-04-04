namespace FiniteStateMachine.Abstract;

public interface IRoute
{
    public RouteState State { get; }
    public string StartState { get; }
    public string EndState { get; }
    public string ErrorMessage { get; }
    
    public char PutChar(char symbol);
    public void Reset();
}