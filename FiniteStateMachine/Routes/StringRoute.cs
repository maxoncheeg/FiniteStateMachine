﻿using FiniteStateMachine.Abstract;
using FiniteStateMachine.Enums;

namespace FiniteStateMachine.Routes;

public class StringRoute : AbstractRoute
{
    private readonly string _word;
    private int _index = 0;

    public StringRoute(string word, string startState, string endState) : base(startState, endState)
    {
        _word = word;
    }

    public override bool PutChar(char symbol)
    {
        var isErrorSymbol = base.PutChar(symbol);
        
        if (_index >= _word.Length)
        {
            State =  RouteState.Error;
        }

        if (symbol == _word[_index] && _index == _word.Length - 1)
        {
            State =  RouteState.Completed;
        }
        else if (symbol != _word[_index])
        {
            State =  RouteState.Error;
        }
        else
        {
            _index++;
            State =  RouteState.IsProgress;
        }
        
        if (State != RouteState.Completed && isErrorSymbol)
        {
            State = RouteState.Error;
            return true;
        }

        return false;
    }

    public override void Reset()
    {
        base.Reset();
        
        _index = 0;
    }
    
    public override string ToString()
    {
        return _word;
    }
}