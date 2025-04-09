using FiniteStateMachine.Abstract;

namespace FiniteStateMachine.EventArgs;

public class ErrorEventArgs(IRouteError error) : System.EventArgs
{
    public IRouteError Error { get; set; } = error;
}