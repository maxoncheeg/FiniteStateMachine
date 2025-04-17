// See https://aka.ms/new-console-template for more information

using FiniteStateMachine;
using FiniteStateMachine.Abstract;
using FiniteStateMachine.Enums;
using FiniteStateMachine.Routes;

List<IRoute> routes =
[
    new StringRoute(@"ask", "A", "AS")
    {
        ErrorMessage = "Ожидается ключевое слово: ask.",
        ErrorOptions = new RouteErrorOptions
            { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\s\[\]]" }
    },
    new StringRoute(@" ", "AS", "T")
    {
        ErrorMessage = "Ожидается пробел.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
    },
    new StringRoute(@"turtles", "T", "I")
    {
        ErrorMessage = "Ожидается ключевое слово: turtles.",
        ErrorOptions = new RouteErrorOptions
            { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\s\[\]]" }
    },
    new StringRoute(@"[", "I", "C")
    {
        ErrorMessage = "Ожидается открывающаяся квадратная скобка.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\w]" }
    },

    new StringRoute(@"set", "C", "CS")
    {
        ErrorMessage = "Возможно имелась ввиду команда set.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\s\]]" }
    },
    new StringRoute(@" ", "CS", "P")
    {
        ErrorMessage = "Ожидается пробел.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
    },

    new StringRoute(@"color", "P", "PS")
    {
        ErrorMessage = "Возможно имелось ввиду свойство color.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\s\]]" }
    },
    new StringRoute(@" ", "PS", "R")
    {
        ErrorMessage = "Ожидается пробел.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
    },
    new StringRoute(@"red", "R", "E")
    {
        ErrorMessage = "Разрешенные объявленные цвета: red, green, blue.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\s\]\,]" }
    },
    new StringRoute(@"green", "R", "E")
    {
        ErrorMessage = "Разрешенные объявленные цвета: red, green, blue.",
        //ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip,  ErrorSymbolRegexPattern = @"[\s\]]" }
    },
    new StringRoute(@"blue", "R", "E")
    {
        ErrorMessage = "Разрешенные объявленные цвета: red, green, blue.",
        //ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip,  ErrorSymbolRegexPattern = @"[\s\]]" }
    },

    new StringRoute(@"heading", "P", "VS")
    {
        ErrorMessage = "Возможно имелось ввиду свойство heading.",
        //ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip,  ErrorSymbolRegexPattern = @"[\s]" }
    },


    new StringRoute(@"forward", "C", "VS")
    {
        ErrorMessage = "Возможно имелось ввиду направление forward.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip,  ErrorSymbolRegexPattern = @"[\s\]]" }
    },
    new StringRoute(@"back", "C", "VS")
    {
        ErrorMessage = "Возможно имелось ввиду направление back.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip,  ErrorSymbolRegexPattern = @"[\s\]]" }
    },
    new StringRoute(@"left", "C", "VS")
    {
        ErrorMessage = "Возможно имелось ввиду направление left.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip,  ErrorSymbolRegexPattern = @"[\s\]]" }
    },
    new StringRoute(@"right", "C", "VS")
    {
        ErrorMessage = "Возможно имелось ввиду направление right.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip,  ErrorSymbolRegexPattern = @"[\s\]]" }
    },

    new StringRoute(@" ", "VS", "V")
    {
        ErrorMessage = "Ожидается пробел.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
    },

    new StringRoute(@"x", "V", "E")
    {
        ErrorMessage = "Разрешенные объявленные переменные: x, y, z.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\,\]]" }
    },
    new StringRoute(@"y", "V", "E")
    {
        ErrorMessage = "Разрешенные объявленные переменные: x, y, z.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\,\]]" }
    },
    new StringRoute(@"z", "V", "E")
    {
        ErrorMessage = "Разрешенные объявленные переменные: x, y, z.",
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip, ErrorSymbolRegexPattern = @"[\,\]]" }
    },
    
    new StringRoute(@"]", "E", "F")
    {
        ErrorMessage = "Ожидается закрывающаяся квадратная скобка.",
        //Priority = -1,
        ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
    },
    new StringRoute(@",", "E", "C")
    {
        ErrorMessage = "Ожидается запятая.",
        Priority = -1,
        ErrorOptions = new RouteErrorOptions
            { Action = RouteErrorAction.RollBack }
    },

];


IFiniteStateMachine urlStateMachine = new FiniteStateMachine.FiniteStateMachine(routes, "A", "F")
    { ResetRoutesIfStartStateHasErrorSymbolsAtStart = false, AllowFindFutureWays = true };

var text = "ask turtles[set forward redback x]";

Console.WriteLine("INPUT:" + text);

//Console.Write("A -> ");
urlStateMachine.StateChanged += (sender, args) =>
{
    if (args.IsFinalState)
    {
        //Console.Write(args.CurrentState);
        Console.WriteLine("\nFIND: " + args.Result);
        return;
    }
    //Console.Write(args.CurrentState + $"({args.Route.ToString()})" + " -> ");
};

urlStateMachine.ErrorOccurred += (sender, args) => { Console.WriteLine($"Error({args.Error.Position}): ({args.Error.Text.ToString()})"); };

for (int i = 0; i < text.Length; i++)
{
    urlStateMachine.PutChar(text[i], i);
}