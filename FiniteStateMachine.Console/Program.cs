// See https://aka.ms/new-console-template for more information

using FiniteStateMachine.Abstract;
using FiniteStateMachine.Routes;

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

IFiniteStateMachine urlStateMachine = new FiniteStateMachine.FiniteStateMachine(states, "A", "G");