using FiniteStateMachine.Abstract;

namespace FiniteStateMachine.Routes;

public class CharVariantRoute : AbstractRoute
{
    private readonly IList<char> _chars;

    public CharVariantRoute(IList<char> chars, string startState, string endState) : base(startState, endState)
    {
        _chars = chars;
    }

    public override char PutChar(char symbol)
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
    
    public override string ToString()
    {
        string result = string.Empty;
        
        foreach (var @char in _chars)
            result += @char == '\n' ? "\\n" : @char;
        
        return result;
    }
}