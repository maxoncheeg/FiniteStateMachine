using System.Text.RegularExpressions;
using FiniteStateMachine.Abstract;

namespace FiniteStateMachine.Routes;

public class RegexSymbolRoute : IRoute
{
    private readonly string _pattern;
    public RouteState State { get; private set; } = RouteState.NotStarted;
    public string StartState { get; }
    public string EndState { get; }
    public string ErrorMessage { get; }

    public RegexSymbolRoute(string pattern, string startState, string endState, string errorMessage = "")
    {
        StartState = startState;
        EndState = endState;
        _pattern = pattern;
        ErrorMessage = errorMessage;
    }

    public char PutChar(char symbol)
    {
        if(Regex.IsMatch(symbol.ToString(), _pattern))
        {
            State = RouteState.Completed;
        }
        else
        {
            State = RouteState.Error;
        }
        return symbol;
    }

    public void Reset()
    {
        State = RouteState.NotStarted;
    }
    
    public override string ToString()
    {
        return _pattern;
    }
}