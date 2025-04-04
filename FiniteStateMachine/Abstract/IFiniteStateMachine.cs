using FiniteStateMachine.EventArgs;
using ErrorEventArgs = FiniteStateMachine.EventArgs.ErrorEventArgs;

namespace FiniteStateMachine.Abstract;

public interface IFiniteStateMachine
{
    public IReadOnlyDictionary<(string, string), IList<IRoute>?> States { get; }
    public event EventHandler<StateEventArgs> StateChanged;
    public event EventHandler<ErrorEventArgs> ErrorOccurred;
    
    public bool PutChar(char symbol, int index);
    public void Reset();
}