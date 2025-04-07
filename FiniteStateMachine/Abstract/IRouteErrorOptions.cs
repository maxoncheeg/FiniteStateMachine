using FiniteStateMachine.Enums;

namespace FiniteStateMachine.Abstract;

public interface IRouteErrorOptions
{
    public RouteErrorAction Action { get; }
    public IList<char> ErrorSymbols { get; }
    public char? FixSymbol { get; }
}