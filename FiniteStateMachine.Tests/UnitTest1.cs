using FiniteStateMachine.Abstract;
using FiniteStateMachine.Routes;

namespace FiniteStateMachine.Tests;

public class Tests
{
    private IFiniteStateMachine _urlStateMachine = null!;
    [SetUp]
    public void Setup()
    {
        List<IRoute> states =
        [
            new StringRoute(@"https://", "A", "C"),
            new StringRoute(@"http://", "A", "C"),
            new StringRoute(@"www.", "A", "C"),
            
            new RegexSymbolRoute(@"[a-z]", "C", "V"), // subdomen.
            new RegexSymbolRoute(@"[a-z]", "V", "V"),
            
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
    public void IncorrectTestCase(string url)
    {
        _urlStateMachine.Reset();
        _urlStateMachine.StateChanged += (sender, args) =>
        {
            if (args.HasSearchCompleted && args.StartIndex == 0 && args.Length == url.Length)
                Assert.Fail();
        };

        for (int i = 0; i < url.Length; i++)
        {
            _urlStateMachine.PutChar(url[i], i);
        }
        
        Assert.Pass();
    }
}