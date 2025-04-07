﻿using FiniteStateMachine.Abstract;
using FiniteStateMachine.Enums;

namespace FiniteStateMachine.Routes;

public class CharVariantRoute : AbstractRoute
{
    private readonly IList<char> _chars;

    public CharVariantRoute(IList<char> chars, string startState, string endState) : base(startState, endState)
    {
        _chars = chars;
    }

    public override bool PutChar(char symbol)
    {
        if(base.PutChar(symbol))
            return true;
        
        if (_chars.Any(@char => @char == symbol))
        {
            State = RouteState.Completed;
        }
        else
            State = RouteState.Error;

        return false;
    }

    public override string ToString()
    {
        string result = string.Empty;

        foreach (var @char in _chars)
            result += @char == '\n' ? "\\n" : @char;

        return result;
    }
}