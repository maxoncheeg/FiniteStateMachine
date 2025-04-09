using FiniteStateMachine.Abstract;
using FiniteStateMachine.Routes;

namespace FiniteStateMachine.Tests;

public class UrlFiniteStateMachineTests
{
    private IFiniteStateMachine _urlStateMachine = null!;

    [SetUp]
    public void Setup()
    {
        List<IRoute> states =
        [
            new StringRoute(@"https://", "A", "C") { Priority = 0 },
            new StringRoute(@"http://", "A", "C") { Priority = 0 },
            //new StringRoute(@"www.", "A", "C"),
            new RegexSymbolRoute(@"[a-z\d]", "A", "V") { Priority = 2 },

            new RegexSymbolRoute(@"[a-z\d]", "C", "V"), // subdomen.
            new RegexSymbolRoute(@"[a-z\d]", "V", "V"),

            new StringRoute(".", "V", "D"),
            new RegexSymbolRoute(@"[a-z]", "D", "V"), // domen

            new StringRoute(@"/", "V", "E"),
            new CharVariantRoute(['\n', ' '], "V", "G"),

            new RegexSymbolRoute(@"[a-zA-Z\#\.\%\=\?\d\&]", "E", "F"),
            new RegexSymbolRoute(@"[a-zA-Z\#\.\%\=\?\d\&]", "F", "F"),
            new StringRoute(@"/", "F", "E"),
            new CharVariantRoute(['\n', ' '], "F", "G"),
            new CharVariantRoute(['\n', ' '], "E", "G"),
        ];

        _urlStateMachine = new FiniteStateMachine(states, "A", "G");
    }

    [TestCase<string>("https://a/ ")]
    [TestCase<string>("https://a// ")]
    [TestCase<string>("123/ru.ru ")]
    public void IncorrectTestCase(string url)
    {
        _urlStateMachine.Reset();
        _urlStateMachine.StateChanged += (sender, args) =>
        {
            if (args.IsFinalState && args.StartIndex == 0 && args.Length == url.Length)
                Assert.Fail();
        };

        for (int i = 0; i < url.Length; i++)
        {
            _urlStateMachine.PutChar(url[i], i);
        }

        Assert.Pass();
    }

    [TestCase<string>("https://a.ru ")]
    [TestCase<string>("http://a.ru ")]
    [TestCase<string>("www.a.ru ")]
    [TestCase<string>("a.ru ")]
    [TestCase<string>("123.ru ")]
    [TestCase<string>("https://www.wildberries.ru/catalog/139365379/detail.aspx?size=236601874 ")]
    public void CorrectTestCase(string url)
    {
        _urlStateMachine.Reset();
        _urlStateMachine.StateChanged += (sender, args) =>
        {
            if (args.IsFinalState && args.StartIndex == 0 && args.Length == url.Length)
                Assert.Pass();
        };

        for (int i = 0; i < url.Length; i++)
        {
            _urlStateMachine.PutChar(url[i], i);
        }

        Assert.Fail();
    }
}