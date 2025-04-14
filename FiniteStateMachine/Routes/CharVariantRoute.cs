using FiniteStateMachine.Abstract;
using FiniteStateMachine.Enums;

namespace FiniteStateMachine.Routes;

public class CharVariantRoute(IList<char> chars, string startState, string endState)
    : AbstractRoute(startState, endState)
{
    public override void PutChar(char symbol)
    {
        base.PutChar(symbol);

        State = chars.Any(@char => @char == symbol) ? RouteState.Completed : RouteState.Error;
    }

    public override double GetPercentage()
    {
        return State == RouteState.Completed ? 1.0 : 0.0;
    }

    public override string ToString()
    {
        string result = string.Empty;

        foreach (var @char in chars)
            result += @char == '\n' ? "\\n" : @char;

        return result;
    }
}