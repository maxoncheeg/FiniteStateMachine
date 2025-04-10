using System.Text.RegularExpressions;
using FiniteStateMachine.Enums;

namespace FiniteStateMachine.Abstract;

public abstract class AbstractRoute(string startState, string endState) : IRoute
{
    public RouteState State { get; protected set; } = RouteState.NotStarted;
    public string StartState { get; } = startState;
    public string EndState { get; } = endState;
    public string ErrorMessage { get; set; } = string.Empty;
    public int Priority { get; set; }
    public IRouteErrorOptions ErrorOptions { get; set; } = new RouteErrorOptions();

    /// <summary>
    /// Если символ является запрещенным вернет true
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public virtual bool PutChar(char symbol)
    {
        if (!string.IsNullOrEmpty(ErrorOptions.ErrorSymbolRegexPattern) &&
            Regex.IsMatch(symbol.ToString(), ErrorOptions.ErrorSymbolRegexPattern))
        {
            return true;
        }

        return false;
    }

    public virtual void Reset()
    {
        State = RouteState.NotStarted;
    }
}