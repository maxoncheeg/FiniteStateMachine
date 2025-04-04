using System.Text.RegularExpressions;
using FiniteStateMachine.Abstract;

namespace FiniteStateMachine.Routes;

public class RegexSymbolRoute : AbstractRoute
{
    private readonly string _pattern;

    public RegexSymbolRoute(string pattern, string startState, string endState) : base(startState, endState)
    {
        _pattern = pattern;
    }

    public override char PutChar(char symbol)
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
    
    public override string ToString()
    {
        return _pattern;
    }
}