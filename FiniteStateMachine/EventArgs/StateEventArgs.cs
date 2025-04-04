namespace FiniteStateMachine.EventArgs;

public class StateEventArgs : System.EventArgs
{
    public bool HasSearchCompleted { get; set; } = false;
    public int? StartIndex { get; set; } = null;
    public int? Length { get; set; } = null;
    public string PreviousState { get; set; } = string.Empty;
    public string CurrentState { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
}