using FiniteStateMachine.Abstract;

namespace FiniteStateMachine.Routes;

public class StringRoute : IRoute
{
    private readonly string _word;
    private int _index = 0;

    public RouteState State { get; private set; } = RouteState.NotStarted;
    public string StartState { get; }
    public string EndState { get; }
    public string ErrorMessage { get; }

    public StringRoute(string word, string startState, string endState, string errorMessage = "")
    {
        StartState = startState;
        EndState = endState;
        _word = word;
        ErrorMessage = errorMessage;
    }

    public char PutChar(char symbol)
    {
        if (_index >= _word.Length)
        {
            State = RouteState.Error;
            return symbol;
        }

        if (symbol == _word[_index] && _index == _word.Length - 1)
        {
            State = RouteState.Completed;
        }
        else if (symbol != _word[_index])
        {
            State = RouteState.Error;
        }
        else
        {
            _index++;
            State = RouteState.IsProgress;
        }

        return symbol;
    }

    public void Reset()
    {
        _index = 0;
        State = RouteState.NotStarted;
    }
    
    public override string ToString()
    {
        return _word;
    }
}