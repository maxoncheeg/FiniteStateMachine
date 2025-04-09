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
                ErrorMessage = "incorrect func type",
                ErrorOptions = new RouteErrorOptions { ErrorSymbolRegexPattern = @"\s", Action = RouteErrorAction.Skip }
            },
            new StringRoute(@"char", "A", "B") { Priority = 1 },

            new StringRoute(@" ", "B", "C")
            {
                ErrorMessage = "missing space",
                ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
            },

            new RegexSymbolRoute(@"[a-z]", "C", "C"),
            new StringRoute(@"(", "C", "D")
            {
                Priority = 1,
                ErrorMessage = "missing dash",
                ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
            },

            new StringRoute(@"int", "D", "E")
            {
                ErrorMessage = "incorrect arg type",
                ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"\s" }
            },
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
    [TestCase<string>("lop a)char x);")]
    [TestCase<string>("lop a))char x);")]
    [TestCase<string>("lop achar x);")]
    //[TestCase<string>("lop achar ")]
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

    //[TestCase<string, string[]>("ints a(char x);", ["missing space", "missing dash"])]
    [TestCase<string, string[]>("vod a(int x);", ["incorrect func type"])]
    [TestCase<string, string[]>("voida(int x);", ["missing space"])]
    [TestCase<string, string[]>("lop a)char x);", ["incorrect func type", "missing dash", "incorrect arg type"])]
    [TestCase<string, string[]>("lop achar x);", ["incorrect func type", "missing dash", "incorrect arg type"])]
    public void CheckErrorsTest(string input, string[] errors)
    {
        Console.WriteLine($"INPUT: {input}\n");
        var errorList = errors.ToList();
        _dimokStateMachine.Reset();
        _dimokStateMachine.StateChanged += (sender, args) =>
        {
            if (args.IsFinalState && args.StartIndex == 0 && args.Length == input.Length)
                Console.WriteLine($"\nFOUND: {args.Result}");
        };

        _dimokStateMachine.ErrorOccurred += (sender, args) =>
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
            _dimokStateMachine.PutChar(input[i], i);
        }

        if (errorList.Count == 0) Assert.Pass();
        else Assert.Fail();
    }
}