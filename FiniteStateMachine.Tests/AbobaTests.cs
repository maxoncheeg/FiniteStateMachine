using FiniteStateMachine.Abstract;
using FiniteStateMachine.Enums;
using FiniteStateMachine.Routes;

namespace FiniteStateMachine.Tests;

public class AbobaTests
{
    private IFiniteStateMachine _stateMachine = null!;

    [SetUp]
    public void Setup()
    {
        List<IRoute> routes =
        [
            new StringRoute("aboba", "A", "B")
            {
                ErrorMessage = "Ожидается ключевое слово: aboba",
                ErrorOptions = new RouteErrorOptions()
                {
                    Action = RouteErrorAction.Skip,
                    ErrorSymbolRegexPattern = @"[\W]"
                }
            },
            new StringRoute(".", "B", "C")
            {
                ErrorMessage = "Ожидается '.'",
                ErrorOptions = new RouteErrorOptions()
                {
                    Action = RouteErrorAction.Skip,
                    ErrorSymbolRegexPattern = @"[\W]"
                }
            }
        ];

        _stateMachine = new FiniteStateMachine(routes, "A", "C");
    }

    [TestCase<string>("abobalol.")]
    [TestCase<string>(",")]
    [TestCase<string>("aboba,")]
    [TestCase<string>("lol !")]
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
}