using FiniteStateMachine.Enums;

namespace FiniteStateMachine.Abstract;

public interface IRouteErrorOptions
{
    public RouteErrorAction Action { get; }
    public string ErrorSymbolRegexPattern { get; }
    public char? FixSymbol { get; }
}