using System.Text.RegularExpressions;
using FiniteStateMachine.Abstract;
using FiniteStateMachine.Enums;

namespace FiniteStateMachine.Routes;

public class RegexSymbolRoute : AbstractRoute
{
    private readonly string _pattern;

    public RegexSymbolRoute(string pattern, string startState, string endState) : base(startState, endState)
    {
        _pattern = pattern;
    }

    public override bool PutChar(char symbol)
    {
        var isErrorSymbol = base.PutChar(symbol);

        if (Regex.IsMatch(symbol.ToString(), _pattern))
        {
            State = RouteState.Completed;
        }
        else
            State = RouteState.Error;
        
        if (State != RouteState.Completed && isErrorSymbol)
        {
            State = RouteState.Error;
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        return _pattern;
    }
}