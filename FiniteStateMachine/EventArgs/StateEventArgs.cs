namespace FiniteStateMachine.EventArgs;

public class StateEventArgs : System.EventArgs
{
    public bool IsFinalState { get; init; }
    
    public int? StartIndex { get; init; }
    public int? Length { get; init; }
    
    public string Result { get; init; } = string.Empty;
    public string PreviousState { get; set; } = string.Empty;
    public string CurrentState { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
}