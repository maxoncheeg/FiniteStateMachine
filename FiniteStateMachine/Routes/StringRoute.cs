using FiniteStateMachine.Abstract;
using FiniteStateMachine.Enums;

namespace FiniteStateMachine.Routes;

public class StringRoute(string word, string startState, string endState) : AbstractRoute(startState, endState)
{
    private int _index;

    public override void PutChar(char symbol)
    {
        base.PutChar(symbol);

        if (_index >= word.Length)
        {
            State = RouteState.Error;
        }

        if (symbol == word[_index] && _index == word.Length - 1)
        {
            State = RouteState.Completed;
        }
        else if (symbol != word[_index])
        {
            State = RouteState.Error;
        }
        else
        {
            _index++;
            State = RouteState.IsProgress;
        }
    }

    public override double GetPercentage()
    {
        return (double)_index / (word.Length - 1);
    }

    public override void Reset()
    {
        base.Reset();

        _index = 0;
    }

    public override string ToString()
    {
        return word;
    }
}