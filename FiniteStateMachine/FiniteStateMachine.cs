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

    private readonly List<IRouteError> _errors = [];
    private readonly Dictionary<(string, string), IList<IRoute>?> _states;

    private List<IRoute> _currentRoutes = [];
    private List<IRoute> _successfulRoutes = [];

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
            _successfulRoutes = [];

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
                newErrors.Add(new RouteError()
                {
                    StartIndex = _startIndex,
                    Position = symbolIndex,
                    EndState = _currentRoutes[i].EndState,
                    StartState = _currentRoutes[i].StartState,
                    Length = _length,
                    Text = _currentRoutes[i].ErrorMessage
                });

                if (errorAction == RouteErrorAction.Skip && isErrorSymbol || errorAction == RouteErrorAction.SkipState)
                {
                    MoveToNextState(_currentRoutes[i]);
                    _length--;
                    PutChar(symbol, symbolIndex);
                }

                if (errorAction == RouteErrorAction.Error)
                {
                    _currentRoutes.RemoveAt(i--);
                }
            }
            else if (_currentRoutes[i].State == RouteState.Completed)
            {
                if (_currentRoutes[i].Priority != _currentRoutes[0].Priority)
                {
                    _successfulRoutes.Add(_currentRoutes[i]);
                    continue;
                }

                MoveToNextState(_currentRoutes[i]);

                break;
            }
        }

        if (_currentRoutes.Count == 0)
        {
            if (_successfulRoutes.Count > 0)
            {
            }


            _errors.AddRange(newErrors);

            ErrorOccurred?.Invoke(this, new ErrorEventArgs() { Errors = _errors });
            _errors.Clear();

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