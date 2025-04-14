# FiniteStateMachine
## __Example:__
### _For example create our own custom state machine._
#### From state A to state B you can move by words: setsomany, set.
#### Also you can move from state A to C by 'setso'.
#### From C to D by ' '. 
#### From D to F by 'man'.
#### From F to G by ' '.
#### From G to B by 'x'.
#### From state B to E you can move by '.' and it's our finale state.
#### It means that we have only three right lines of code: __'setsomany.'__, __'set.'__, __'setso man x.'__.
### In code it looks like:
```csharp
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
            new StringRoute(@"man", "D", "F"),
            new StringRoute(@" ", "F", "G")
            {
                ErrorMessage = "missing space",
                ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.SkipState }
            },
            new StringRoute(@"x", "G", "B"),


            new StringRoute(@"set", "A", "B")
            {
                ErrorMessage = "unsupported key word",
                Priority = 2
            },

            new StringRoute(@".", "B", "E")
            {
                ErrorMessage = "unsupported keys",
                ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
            },
            new StringRoute(@",", "B", "E")
            {
                ErrorMessage = "unsupported keys",
                ErrorOptions = new RouteErrorOptions { Action = RouteErrorAction.Skip }
            },
        ];

        _stateMachine = new FiniteStateMachine(states, "A", "E");
```

### With this configuration let's try this string: _'setsomanx.'_.
```csharp
        var input= "setsomanx.";
        
        Console.WriteLine($"INPUT: {input}\n");

        _stateMachine.StateChanged += (sender, args) =>
        {
            if (args.IsFinalState && args.StartIndex == 0 && args.Length == input.Length)
                Console.WriteLine($"\nFOUND: {args.Result}");
        };

        _stateMachine.ErrorOccurred += (sender, args) =>
        {
            Console.WriteLine(
                $"(pos {args.Error.Position}): {args.Error.Text}");
        };

        for (int i = 0; i < input.Length; i++)
        {
            _stateMachine.PutChar(input[i], i);
        }
```
### In output we have:
```

INPUT: setsomanx.

(pos 0): missing space
(pos 3): missing space

FOUND: setsomanx.
```

## __Happy using!__
