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

    //private readonly List<IRouteError> _errors = [];
    private readonly Dictionary<(string, string), IList<IRoute>?> _states;

    private List<IRoute> _currentRoutes = [];
    private ISuccessfulPosition? _successfulPosition = null;

    public IReadOnlyDictionary<(string, string), IList<IRoute>?> States => _states;

    public event EventHandler<StateEventArgs>? StateChanged;
    public event EventHandler<ErrorEventArgs>? ErrorOccurred;

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
            _successfulPosition = null;

            _isFirstIteration = false;
        }

        if (_inProgress == false)
        {
            _startIndex = symbolIndex;
            _length = 0;
            _currentResult = string.Empty;
        }

        List<IRouteError> newErrors = [];
        _currentResult += symbol;

        _length++;
        for (int i = 0; i < _currentRoutes.Count; i++)
        {
            var isErrorSymbol = _currentRoutes[i].PutChar(symbol);

            if (_currentRoutes[i].State == RouteState.Error)
            {
                var errorAction = _currentRoutes[i].ErrorOptions.Action;
                // todo: учитывать route.ErrorOptions

                if (errorAction == RouteErrorAction.RollBack)
                {
                    if (_currentRoutes.Count > 1)
                    {
                        _currentRoutes.RemoveAt(i--);
                    }
                    else if (_successfulPosition != null)
                    {
                        string unseenText = _currentResult.Substring(_successfulPosition.Index + 1);
                        _currentResult = _currentResult.Substring(0, _successfulPosition.Index + 1);
                        _length = _successfulPosition.Index + 1;

                        MoveToNextState(_successfulPosition.Route);
                        for (int j = 0; j < unseenText.Length; j++)
                        {
                            PutChar(unseenText[j], j);
                        }
                    }
                }

                if (errorAction == RouteErrorAction.Skip && isErrorSymbol || errorAction == RouteErrorAction.SkipState)
                {
                    // newErrors.Add(new RouteError()
                    // {
                    //     StartState = _currentRoutes[i].StartState,
                    //     EndState = _currentRoutes[i].EndState,
                    //     Position = symbolIndex,
                    //     Text = _currentRoutes[i].ErrorMessage,
                    //     Route = _currentRoutes[i].ToString() ?? string.Empty,
                    // });

                    ErrorOccurred?.Invoke(this, new ErrorEventArgs(new RouteError
                    {
                        StartState = _currentRoutes[i].StartState,
                        EndState = _currentRoutes[i].EndState,
                        Position = symbolIndex,
                        Text = _currentRoutes[i].ErrorMessage,
                        Route = _currentRoutes[i].ToString() ?? string.Empty,
                    }));

                    MoveToNextState(_currentRoutes[i]);

                    _length--;
                    _currentResult = _currentResult[..^1];
                    PutChar(symbol, symbolIndex);
                    break;
                }

                if (errorAction == RouteErrorAction.Error)
                {
                    newErrors.Add(new RouteError()
                    {
                        StartState = _currentRoutes[i].StartState,
                        EndState = _currentRoutes[i].EndState,
                        Position = symbolIndex,
                        Text = _currentRoutes[i].ErrorMessage,
                        Route = _currentRoutes[i].ToString() ?? string.Empty,
                    });
                    
                    _currentRoutes.RemoveAt(i--);
                }
            }
            else if (_currentRoutes[i].State == RouteState.Completed)
            {
                if (_currentRoutes[i].Priority != _currentRoutes[0].Priority)
                {
                    if (_successfulPosition == null)
                        _successfulPosition = new SuccessfulPosition(_currentRoutes[i], symbolIndex);
                    else if (_successfulPosition.Route.Priority > _currentRoutes[i].Priority)
                        _successfulPosition = new SuccessfulPosition(_currentRoutes[i], symbolIndex);

                    _currentRoutes.RemoveAt(i--);
                    continue;
                }

                MoveToNextState(_currentRoutes[i]);

                break;
            }
        }

        if (_currentRoutes.Count == 0)
        {
            foreach (var error in newErrors)
            {
                ErrorOccurred?.Invoke(this, new ErrorEventArgs(error));
            }

            //_errors.Clear();

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

        _currentState = _startState;
        _inProgress = false;
        _isFirstIteration = true;
    }
}