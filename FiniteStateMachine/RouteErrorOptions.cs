using FiniteStateMachine.Abstract;
using FiniteStateMachine.Enums;

namespace FiniteStateMachine;

public class RouteErrorOptions : IRouteErrorOptions
{
    public RouteErrorAction Action { get; init; } = RouteErrorAction.Error;
    public string ErrorSymbolRegexPattern { get; init; } = string.Empty;
    public char? FixSymbol { get; init; } = null;
}