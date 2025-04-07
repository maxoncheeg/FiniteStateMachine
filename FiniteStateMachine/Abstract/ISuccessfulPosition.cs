namespace FiniteStateMachine.Abstract;

public interface ISuccessfulPosition
{
    public IRoute Route { get; }
    public int Index { get; }
}