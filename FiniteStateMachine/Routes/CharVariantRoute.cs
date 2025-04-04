using FiniteStateMachine.Abstract;

namespace FiniteStateMachine.Routes;

public class CharVariantRoute : IRoute
{
    private readonly IList<char> _chars;
    public RouteState State { get; private set; } = RouteState.NotStarted;
    public string StartState { get; }
    public string EndState { get; }
    public string ErrorMessage { get; }

    public CharVariantRoute(IList<char> chars, string startState, string endState, string errorMessage = "")
    {
        StartState = startState;
        EndState = endState;
        _chars = chars;
        ErrorMessage = errorMessage;
    }

    public char PutChar(char symbol)
    {
        foreach (var @char in _chars)
        {
            if (@char == symbol)
            {
                State = RouteState.Completed;
                return @char;
            }
        }

        State = RouteState.Error;

        return symbol;
    }

    public void Reset()
    {
        State = RouteState.NotStarted;
    }

    public override string ToString()
    {
        string result = string.Empty;
        
        foreach (var @char in _chars)
            result += @char;
        
        return result;
    }
}