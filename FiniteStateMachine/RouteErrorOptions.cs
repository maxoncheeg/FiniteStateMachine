using FiniteStateMachine.Abstract;
using FiniteStateMachine.Enums;

namespace FiniteStateMachine;

public class RouteErrorOptions : IRouteErrorOptions
{
    public RouteErrorAction Action { get; init; } = RouteErrorAction.Error;
    public IList<char> ErrorSymbols { get; init; } = [];
    public char? FixSymbol { get; init; } = null;
}