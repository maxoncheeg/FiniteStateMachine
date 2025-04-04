using FiniteStateMachine.Abstract;

namespace FiniteStateMachine.EventArgs;

public class ErrorEventArgs : System.EventArgs
{
    public IList<IRouteError> Errors { get; init; } = [];
}