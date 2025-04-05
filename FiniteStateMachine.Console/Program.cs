// See https://aka.ms/new-console-template for more information

using FiniteStateMachine.Abstract;
using FiniteStateMachine.Routes;

List<IRoute> states =
[
    new StringRoute(@"abo://", "A", "C") { Priority = 0 },
    new StringRoute(@"a.", "A", "C") { Priority = 1 },

    new StringRoute(@".", "C", "B"),
    new StringRoute(@",", "C", "B"),

    new StringRoute(@"ru", "B", "E"),
];

IFiniteStateMachine urlStateMachine = new FiniteStateMachine.FiniteStateMachine(states, "A", "E");

var text = "a.ru";

