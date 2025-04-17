using FiniteStateMachine.EventArgs;
using ErrorEventArgs = FiniteStateMachine.EventArgs.ErrorEventArgs;

namespace FiniteStateMachine.Abstract;

public interface IFiniteStateMachine
{
    public IReadOnlyDictionary<(string, string), IList<IRoute>?> States { get; }
    public event EventHandler<StateEventArgs> StateChanged;
    public event EventHandler<ErrorEventArgs> ErrorOccurred;

    public int PutChar(char symbol, int index);
    public int PutString(string value);
    public void Reset();
}