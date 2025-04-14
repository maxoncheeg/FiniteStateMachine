using System.Text.RegularExpressions;
using FiniteStateMachine.Abstract;
using FiniteStateMachine.Enums;

namespace FiniteStateMachine.Routes;

public class RegexSymbolRoute(string pattern, string startState, string endState) : AbstractRoute(startState, endState)
{
    public override void PutChar(char symbol)
    {
        base.PutChar(symbol);

        State = Regex.IsMatch(symbol.ToString(), pattern) ? RouteState.Completed : RouteState.Error;
    }

    public override double GetPercentage()
    {
        return State == RouteState.Completed ? 1.0 : 0.0;
    }

    public override string ToString()
    {
        return pattern;
    }
}