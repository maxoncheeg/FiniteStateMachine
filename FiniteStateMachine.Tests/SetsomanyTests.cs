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
                ErrorMessage = "missing space",
                ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
            },
            new StringRoute(@"man", "D", "F"),
            new StringRoute(@" ", "F", "G")
            {
                ErrorMessage = "missing space",
                ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
            },
            new StringRoute(@"x", "G", "B"),


            new StringRoute(@"set", "A", "B")
            {
                ErrorMessage = "unsupported key word",
                Priority = 2
            },

            new StringRoute(@".", "B", "E")
            {
                ErrorMessage = "unsupported keys",
                ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
            },
        ];

        _stateMachine = new FiniteStateMachine(states, "A", "E");
    }

    [TestCase<string>("setsomany.")]
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

        _stateMachine.ErrorOccurred += (sender, args) =>
        {
            Console.WriteLine($"(pos {args.Error.Position}): {args.Error.Text}");
        };

        for (int i = 0; i < input.Length; i++)
        {
            _stateMachine.PutChar(input[i], i);
        }

        Assert.Fail();
    }
    
    [TestCase<string, string[]>("setsomanyyeah.", [])]
    [TestCase<string, string[]>("setsomanx.", ["missing space", "missing space"])]
    [TestCase<string, string[]>("set.", [])]
    public void CheckErrorsTest(string input, string[] errors)
    {
        Console.WriteLine($"INPUT: {input}\n");
        var errorList = errors.ToList();
        _stateMachine.Reset();
        _stateMachine.StateChanged += (sender, args) =>
        {
            if (args.IsFinalState && args.StartIndex == 0 && args.Length == input.Length)
                Console.WriteLine($"\nFOUND: {args.Result}");
        };

        _stateMachine.ErrorOccurred += (sender, args) =>
        {
            Console.WriteLine($"(pos {args.Error.Position}): {args.Error.Text}");

            if (args.Error.Text == string.Empty)
            {
            }
            else if (errorList.Contains(args.Error.Text))
                errorList.Remove(args.Error.Text);
            else
            {
                Assert.Fail();
            }
        };

        for (int i = 0; i < input.Length; i++)
        {
            _stateMachine.PutChar(input[i], i);
        }

        if (errorList.Count == 0) Assert.Pass();
        else Assert.Fail();
    }
    
}