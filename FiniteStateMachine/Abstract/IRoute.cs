using FiniteStateMachine.Enums;

namespace FiniteStateMachine.Abstract;

public interface IRoute
{
    public bool HasErrorSymbols { get; }
    public RouteState State { get; }
    public string StartState { get; }
    public string EndState { get; }
    public string ErrorMessage { get; }
    public int Priority { get; }
    public IRouteErrorOptions ErrorOptions { get; } 
    
    /// <summary>
    /// Провести символ через путь.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns>Если путь встретит запрещенный ErrorOptions символ, вернется true</returns>
    public void PutChar(char symbol);
    public double GetPercentage();
    public void Reset();
}