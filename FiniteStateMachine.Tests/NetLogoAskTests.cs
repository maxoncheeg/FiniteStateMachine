using FiniteStateMachine.Abstract;
using FiniteStateMachine.Routes;

namespace FiniteStateMachine.Tests;

public class NetLogoAskTests
{
    private IFiniteStateMachine _askStateMachine = null!;
    
     public void Setup()
    {
        List<IRoute> states =
        [
            new StringRoute(@"ask", "A", "B"),
            new StringRoute(@" ", "B", "C"),
            new StringRoute(@"turtles", "C", "D"),
            new StringRoute(@"[", "C", "D"),
            
            new StringRoute(@"set", "D", "E"),
            new StringRoute(@"forward", "D", "S"),
            new StringRoute(@"back", "D", "S"),
            new StringRoute(@"left", "D", "S"),
            new StringRoute(@"right", "D", "S"),
            
            
            new StringRoute(@"right", "D", "S"),
            
            
            
            new StringRoute(@"]", "D", "E"),
            
            
            

        ];

        _askStateMachine = new FiniteStateMachine(states, "A", "G");
    }

    [TestCase<string>("https://a/ ")]
    [TestCase<string>("https://a// ")]
    [TestCase<string>("123/ru.ru ")]
    public void IncorrectTestCase(string url)
    {

    }

    [TestCase<string>("https://a.ru ")]
    [TestCase<string>("http://a.ru ")]
    [TestCase<string>("www.a.ru ")]
    [TestCase<string>("www://.a.ru ")]
    [TestCase<string>("a.ru ")]
    [TestCase<string>("123.ru ")]
    [TestCase<string>("https://www.wildberries.ru/catalog/139365379/detail.aspx?size=236601874 ")]
    public void CorrectTestCase(string url)
    {

    }
}