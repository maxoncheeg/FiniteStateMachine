using FiniteStateMachine.Abstract;
using FiniteStateMachine.Enums;
using FiniteStateMachine.Routes;

namespace FiniteStateMachine.Tests;

public class SetsomanyTests
{
    private IFiniteStateMachine _stateMachine = null!;

    [SetUp]
    public void Setup()
    {
        List<IRoute> states =
        [
            new StringRoute(@"setsomany", "A", "B")
            {
                Priority = 0, ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.RollBack }
            },

            new StringRoute(@"setso", "A", "C")
            {
                Priority = 1, ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.RollBack }
            },
            new StringRoute(@" ", "C", "D")
            {
                ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
            },
            new StringRoute(@"man", "D", "F"),
            new StringRoute(@" ", "F", "G")
            {
                ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
            },
            new StringRoute(@"x", "G", "B"),


            new StringRoute(@"set", "A", "B")
            {
                Priority = 2
            },

            new StringRoute(@".", "B", "E")
            {
                ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
            },
        ];

        _stateMachine = new FiniteStateMachine(states, "A", "E");
    }

    [TestCase<string>("setsomany.")]
    [TestCase<string>("setso.")]
    [TestCase<string>("set.")]
    [TestCase<string>("setsomanx.")]
    [TestCase<string>("setso man x.")]
    [TestCase<string>("setsomanyyeah.")]
    public void CorrectOutputLengthTest(string input)
    {
        _stateMachine.Reset();
        _stateMachine.StateChanged += (sender, args) =>
        {
            if (args.IsFinalState && args.StartIndex == 0 && args.Length == input.Length)
                Assert.Pass();
        };

        // _dimokStateMachine.ErrorOccurred += (sender, args) =>
        // {
        //     foreach (var VARIABLE in COLLECTION)
        //     {
        //         
        //     }
        // };

        for (int i = 0; i < input.Length; i++)
        {
            _stateMachine.PutChar(input[i], i);
        }

        Assert.Fail();
    }
}