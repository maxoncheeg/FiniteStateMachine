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
            new StringRoute(@"man", "D", "F")
            {
                ErrorMessage = "missing man",
                ErrorOptions = new RouteErrorOptions
                    { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\s\.]" }
            },
            new StringRoute(@" ", "F", "G")
            {
                ErrorMessage = "missing space",
                ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
            },
            new StringRoute(@"x", "G", "B")
            {
                ErrorMessage = "missing x",
            },

            new StringRoute(@"y", "G", "T")
            {
                ErrorMessage = "missing y",
                ErrorOptions = new RouteErrorOptions
                    { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\s\.]" }
            },

            new StringRoute(@"aloha", "T", "B")
            {
                ErrorMessage = "missing aloha",
                ErrorOptions = new RouteErrorOptions
                    { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\s\.]" }
            },


            new StringRoute(@"set", "A", "B")
            {
                ErrorMessage = "set",
                Priority = 2
            },

            new StringRoute(@"get", "A", "N")
            {
                ErrorMessage = "get",
                Priority = 2
            },

            new StringRoute(@"color", "N", "B")
            {
                ErrorMessage = "color err",
                ErrorOptions = new RouteErrorOptions
                    { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\.\,]" }
            },
            new StringRoute(@"height", "N", "B")
            {
                ErrorMessage = "height err",
                ErrorOptions = new RouteErrorOptions
                    { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\.\,]" }
            },


            new StringRoute(@".", "B", "E")
            {
                ErrorMessage = "missing .",
                ErrorOptions = new RouteErrorOptions
                    { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\s]" }
            },
            new StringRoute(@",", "B", "E")
            {
                ErrorMessage = "missing ,",
                ErrorOptions = new RouteErrorOptions
                    { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\s]" }
            },
        ];

        _stateMachine = new FiniteStateMachine(states, "A", "E")
            {AllowFindFutureWays = true};
}

    [TestCase<string>("setsomany.")]
    [TestCase<string>("set.")]
    [TestCase<string>("setsomanx.")]
    [TestCase<string>("setso man x.")]
    [TestCase<string>("setsomanyyeah.")]
    [TestCase<string>("getcolor.")]
    [TestCase<string>("getheight.")]
    public void CorrectOutputLengthTest(string input)
    {
        _stateMachine.Reset();

        var text = "setsomanx.";

        _stateMachine.StateChanged += (sender, args) =>
        {
            if (args.IsFinalState && args.StartIndex == 0 && args.Length == input.Length)
                Assert.Pass();

            // here you can do something
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

    [TestCase<string, string[]>("setsomanyyeah.", ["yeah"])]
    [TestCase<string, string[]>("setsomanx.", ["missing space", "missing space"])]
    [TestCase<string, string[]>("getcolr.", ["color err"])]
    [TestCase<string, string[]>("gethegdsit.", ["height err"])]
    [TestCase<string, string[]>("getcolheight.", ["col"])]
    [TestCase<string, string[]>("getheicolog.", ["color err"])]
    [TestCase<string, string[]>("set.", [])]
    [TestCase<string, string[]>("setso aloha.", ["missing man", "missing space", "missing y"])]
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
            Console.WriteLine(
                $"(pos {args.Error.Position}): {args.Error.Text}");

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