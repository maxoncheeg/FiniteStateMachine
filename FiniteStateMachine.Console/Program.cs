// See https://aka.ms/new-console-template for more information

using FiniteStateMachine;
using FiniteStateMachine.Abstract;
using FiniteStateMachine.Enums;
using FiniteStateMachine.Routes;

List<IRoute> states =
[
    new StringRoute(@"ask", "A", "AS")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\s\[]" }
    },
    new StringRoute(@" ", "AS", "T")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
    },
    new StringRoute(@"turtles", "T", "I")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\s\[]" }
    },
    new StringRoute(@"[", "I", "C")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
    },
    
    new StringRoute(@"set", "C", "CS")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
    },
    new StringRoute(@" ", "CS", "P")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
    },
    
    new StringRoute(@"color", "P", "PS")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
    },
    new StringRoute(@" ", "PS", "R")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
    },
    new StringRoute(@"red", "R", "E")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
    },
    new StringRoute(@"green", "R", "E")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
    },
    new StringRoute(@"blue", "R", "E")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
    },
    
    new StringRoute(@"heading", "P", "VS")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
    },
    
    
    new StringRoute(@"forward", "C", "VS")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
    },
    new StringRoute(@"back", "C", "VS")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
    },
    new StringRoute(@"left", "C", "VS")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
    },
    new StringRoute(@"right", "C", "VS")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
    },
    
    new StringRoute(@" ", "VS", "V")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
    },
    
    new StringRoute(@"x", "V", "E")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
    },
    new StringRoute(@"y", "V", "E")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
    },
    new StringRoute(@"z", "V", "E")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
    },
    
    new StringRoute(@"]", "E", "F")
    {
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
    },
    new StringRoute(@",", "E", "C")
    {
        //ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
    },
];



IFiniteStateMachine urlStateMachine = new FiniteStateMachine.FiniteStateMachine(states, "A", "F");

var text = "ask turtles [set heading x, back x, set color blue]";

Console.Write("A -> ");
urlStateMachine.StateChanged += (sender, args) =>
{
    if (args.IsFinalState)
    {
        Console.Write(args.CurrentState);
        Console.WriteLine("\n" + args.Result);
        return;
    }
    Console.Write(args.CurrentState + $"({args.Route.ToString()})" + " -> ");
};

for (int i = 0; i < text.Length; i++)
{
    urlStateMachine.PutChar(text[i], i);
}