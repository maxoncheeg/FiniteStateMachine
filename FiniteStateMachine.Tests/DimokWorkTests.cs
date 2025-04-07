using FiniteStateMachine.Abstract;
using FiniteStateMachine.Enums;
using FiniteStateMachine.Routes;

namespace FiniteStateMachine.Tests;

public class DimokWorkTests
{
    private IFiniteStateMachine _dimokStateMachine = null!;

    [SetUp]
    public void Setup()
    {
        List<IRoute> states =
        [
            new StringRoute(@"int", "A", "B"),
            new StringRoute(@"void", "A", "B")
            {
                Priority = 1,
                ErrorOptions = new RouteErrorOptions { ErrorSymbolRegexPattern = @"\s", Action = RouteErrorAction.Skip }
            },
            new StringRoute(@"char", "A", "B") { Priority = 1 },

            new StringRoute(@" ", "B", "C")
            {
                ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
            },

            new RegexSymbolRoute(@"[a-z]", "C", "C"),
            new StringRoute(@"(", "C", "D"),

            new StringRoute(@"int", "D", "E"),
            new StringRoute(@"char", "D", "E") { Priority = 1 },

            new StringRoute(@" ", "E", "F"),

            new RegexSymbolRoute(@"[a-z]", "F", "F"),
            new StringRoute(@")", "F", "G"),
            new StringRoute(@";", "G", "Z"),
        ];

        _dimokStateMachine = new FiniteStateMachine(states, "A", "Z");
    }

    [TestCase<string>("int a(char x);")]
    [TestCase<string>("vod a(int x);")]
    [TestCase<string>("voida(int x);")]
    public void CorrectOutputLengthTest(string input)
    {
        _dimokStateMachine.Reset();
        _dimokStateMachine.StateChanged += (sender, args) =>
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
            _dimokStateMachine.PutChar(input[i], i);
        }

        Assert.Fail();
    }
}