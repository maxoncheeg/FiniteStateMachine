using FiniteStateMachine.Abstract;
using FiniteStateMachine.Enums;
using FiniteStateMachine.EventArgs;
using ErrorEventArgs = FiniteStateMachine.EventArgs.ErrorEventArgs;

namespace FiniteStateMachine;

public class FiniteStateMachine : IFiniteStateMachine
{
    private readonly string _startState;
    private readonly string _endState;

    private int _startIndex;
    private int _length;

    private string _currentState;
    private string _currentResult = string.Empty;

    private bool _inProgress;
    private bool _isFirstIteration = true;

    private readonly Dictionary<(string, string), IList<IRoute>?> _states;

    private List<IRoute> _currentRoutes = [];
    private List<IRoute> _futureRoutes = [];
    private Dictionary<IRoute, string> _errorSymbols = [];
    private ISuccessfulPosition? _successfulPosition;

    public IReadOnlyDictionary<(string, string), IList<IRoute>?> States => _states;

    public event EventHandler<StateEventArgs>? StateChanged;
    public event EventHandler<ErrorEventArgs>? ErrorOccurred;
    public bool ResetRoutesIfStartStateHasErrorSymbolsAtStart { get; set; }
    public bool AllowFindFutureWays { get; set; }

    public FiniteStateMachine(List<IRoute> routes, string startState, string endState)
    {
        if (routes.Count == 0)
            throw new NullReferenceException("Finite states cannot be empty");

        _states = new Dictionary<(string, string), IList<IRoute>?>();
        _startState = startState;
        _endState = endState;

        foreach (var route in routes.Where(route => !_states.TryAdd((route.StartState, route.EndState), [route])))
        {
            _states[(route.StartState, route.EndState)] ??= new List<IRoute>();
            _states[(route.StartState, route.EndState)]?.Add(route);
        }

        _currentState = startState;
    }

    public int PutChar(char symbol, int symbolIndex)
    {
        if (_isFirstIteration)
        {
            _currentRoutes = _states.Where(state => state.Key.Item1 == _currentState)
                .SelectMany(state => state.Value ?? []).ToList();
            _currentRoutes.Sort((r1, r2) => r1.Priority < r2.Priority ? -1 : 1);
            _currentRoutes.ForEach(r => r.Reset());
            _successfulPosition = null;

            if (AllowFindFutureWays)
            {
                _futureRoutes = GetNextStates(_currentState);
                _futureRoutes.ForEach(r => r.Reset());
            }

            _isFirstIteration = false;
            _errorSymbols = [];
        }

        if (_inProgress == false)
        {
            _startIndex = symbolIndex;
            _length = 0;
            _currentResult = string.Empty;
        }

        if(symbol == 'd')
            Console.WriteLine(',');

        List<IRouteError> newErrors = [];
        _currentResult += symbol;

        _length++;
        for (int i = 0; i < _currentRoutes.Count; i++)
        {
            var currentRoute = _currentRoutes[i];
            _currentRoutes[i].PutChar(symbol);
            

            if (currentRoute.State == RouteState.Completed)
            {
                if (currentRoute.Priority != _currentRoutes[0].Priority)
                {
                    if (_successfulPosition == null)
                        _successfulPosition = new SuccessfulPosition(currentRoute, symbolIndex);
                    else if (_successfulPosition.Route.Priority > currentRoute.Priority)
                        _successfulPosition = new SuccessfulPosition(currentRoute, symbolIndex);

                    _currentRoutes.RemoveAt(i--);
                    continue;
                }

                MoveToNextState(currentRoute);

                break;
            }

            if (currentRoute.State == RouteState.Error || currentRoute.HasErrorSymbols)
            {
                var errorAction = currentRoute.ErrorOptions.Action;

                if (errorAction == RouteErrorAction.RollBack)
                {
                    if (_currentRoutes.Count > 1)
                    {
                        _currentRoutes.RemoveAt(i--);
                    }
                    else if (_successfulPosition != null)
                    {
                        string unseenText = _currentResult[(_successfulPosition.Index + 1)..];
                        _currentResult = _currentResult[..(_successfulPosition.Index + 1)];
                        _length = _successfulPosition.Index + 1;

                        MoveToNextState(_successfulPosition.Route);
                        for (int j = 0; j < unseenText.Length; j++)
                        {
                            PutChar(unseenText[j], j);
                        }
                    }
                }

                if (errorAction == RouteErrorAction.Skip)
                {
                    _errorSymbols.TryAdd(currentRoute, string.Empty);
                    _errorSymbols[currentRoute] += symbol;
                }

                if (errorAction == RouteErrorAction.Skip && currentRoute.HasErrorSymbols ||
                    errorAction == RouteErrorAction.SkipState)
                {
                    if (ResetRoutesIfStartStateHasErrorSymbolsAtStart && _currentState == _startState &&
                        _currentResult.Length == 1)
                    {
                        ResetRoutes();
                        return 0;
                    }

                    var futureRoute = _futureRoutes.FirstOrDefault(r => r.State == RouteState.Completed);
                    var errorRoutes = futureRoute != null ? FindWayToRoute(currentRoute,futureRoute, []) : null;
                    if (futureRoute != null && errorRoutes != null && errorAction == RouteErrorAction.Skip)
                    {
                        foreach (var route in errorRoutes)
                        {
                            ErrorOccurred?.Invoke(this, new ErrorEventArgs(new RouteError
                            {
                                StartState = route.StartState,
                                EndState = route.EndState,
                                Position = symbolIndex,
                                Text = route.ErrorMessage,
                                Route = route.ToString() ?? string.Empty,
                            }));
                        }

                        currentRoute = futureRoute;
                    }
                    else
                    {

                        var otherRoutes = _currentRoutes
                            .Where(r => r != currentRoute && r.Priority == currentRoute.Priority).ToList();
                        otherRoutes.Sort((r1, r2) => r1.GetPercentage().CompareTo(r2.GetPercentage()));

                        currentRoute =
                            otherRoutes.Count > 0 && otherRoutes[0].GetPercentage() > currentRoute.GetPercentage()
                                ? otherRoutes[0]
                                : currentRoute;
                        
                        ErrorOccurred?.Invoke(this, new ErrorEventArgs(new RouteError
                        {
                            StartState = currentRoute.StartState,
                            EndState = currentRoute.EndState,
                            Position = symbolIndex,
                            Text = currentRoute.ErrorMessage,
                            Route = currentRoute.ToString() ?? string.Empty,
                        }));
                        
                        if (_errorSymbols.ContainsKey(currentRoute))
                            _errorSymbols.Remove(currentRoute);
                    }
                    

                    MoveToNextState(currentRoute);
                    if (currentRoute.EndState == _endState) return 0;

                    _length--;
                    _currentResult = _currentResult[..^1];
                    PutChar(symbol, symbolIndex);
                    break;
                }

                if (errorAction == RouteErrorAction.Error)
                {
                    newErrors.Add(new RouteError()
                    {
                        StartState = currentRoute.StartState,
                        EndState = currentRoute.EndState,
                        Position = symbolIndex,
                        Text = currentRoute.ErrorMessage,
                        Route = currentRoute.ToString() ?? string.Empty,
                    });

                    _currentRoutes.RemoveAt(i--);
                }
            }
        }
        _futureRoutes.ForEach(r => r.PutChar(symbol));

        if (_currentRoutes.Count == 0)
        {
            foreach (var error in newErrors)
            {
                ErrorOccurred?.Invoke(this, new ErrorEventArgs(error));
            }

            var inProgress = _inProgress;
            ResetRoutes();

            if (inProgress)
                PutChar(symbol, symbolIndex);
        }

        _inProgress = true;

        return 0;
    }

    public int PutString(string value)
    {
        throw new NotImplementedException();
    }

    private void MoveToNextState(IRoute route)
    {
        if (_errorSymbols.ContainsKey(route))
        {
            ErrorOccurred?.Invoke(this, new ErrorEventArgs(new RouteError()
            {
                StartState = _currentRoutes[0].StartState,
                EndState = _currentRoutes[0].EndState,
                Position = _currentResult.Length - _errorSymbols[route].Length - 1,
                Type = StateMachineErrorType.ErrorSymbols,
                Route = route.ToString() ?? string.Empty,
                Text = _errorSymbols[route],
                Result = _currentResult
            }));
        }

        _currentState = route.EndState;
        _isFirstIteration = true;

        StateChanged?.Invoke(this,
            new StateEventArgs()
            {
                IsFinalState = _currentState == _endState,

                StartIndex = _startIndex,
                Length = _length,
                Route = route.ToString() ?? String.Empty,

                CurrentState = _currentState, Result = _currentResult,
                PreviousState = route.StartState
            });

        if (_currentState == _endState)
        {
            ResetRoutes();
        }
    }

    public void Reset()
    {
        ErrorOccurred = null;
        StateChanged = null;
        ResetRoutes();
    }

    private void ResetRoutes()
    {
        foreach (var states in _states)
            if (states.Value != null)
                foreach (var route in states.Value)
                    route.Reset();

        _currentResult = string.Empty;
        _currentState = _startState;
        _inProgress = false;
        _isFirstIteration = true;
    }

    private List<IRoute> GetNextStates(string state)
    {
      
        List<string> tempStates = [];
        List<IRoute> result = [];
        
        tempStates = _states.Where(s => s.Key.Item1 == state).SelectMany(s => s.Value).Select(r => r.EndState).Distinct().ToList();

        for(int i = 0; i < tempStates.Count; i++){
        // while (tempStates.Any())
        // {
            var currentState = tempStates[i];
            var temp = _states.Where(s => s.Key.Item1 == currentState).SelectMany(s => s.Value);
            foreach (var route in temp)
            {
                if(!tempStates.Contains(route.EndState))
                    tempStates.Add(route.EndState);
            }
            tempStates.AddRange(temp.Select(r => r.EndState).ToList());
            tempStates = tempStates.Distinct().ToList();
            //tempStates.RemoveAt(0);
            
            result.AddRange(temp);
        }
        
        result = result.Distinct().ToList();
        return result;
    }

    private List<IRoute> FindWayToRoute(IRoute current, IRoute searchRoute, List<IRoute> previousRoutes)
    {
        List<IRoute> result = [];
        
        var nextRoutes = _states.Where(state => state.Key.Item1 == current.EndState)
            .SelectMany(state => state.Value ?? []).ToList();

        if (nextRoutes.Contains(searchRoute))
            return [current];

        previousRoutes.Add(current);
        foreach (var route in nextRoutes)
        {
            if(route.EndState == current.StartState)continue;
            if(previousRoutes.Contains(route))continue;
            
            var ways = FindWayToRoute(route, searchRoute, previousRoutes);
            if(ways?.Count > 0)
            {
                result = [current];
                result.AddRange(ways);
                break;
            }
        }
        
        return result;
    }
}