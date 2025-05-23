﻿using FiniteStateMachine.Enums;

namespace FiniteStateMachine.Abstract;

public interface IRouteError
{
    public int Position { get; init; }
    public string Result { get; init; }
    public string StartState { get; init; }
    public string EndState { get; init; }
    public string Route { get; init; }
    public StateMachineErrorType Type { get; init; }
    public string Text { get; init; }
}