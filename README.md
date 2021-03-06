# Naive implementations of a pipe operation in C#
This repository contains two working implementations of a pipe operation in C#.

## [Option 1](../master/Option1)
```cs
1d.Pipe(
    Pipe<double>.ToTimeSpanAsDays,
    Pipe<TimeSpan>.AddHours(3),
    Pipe<TimeSpan>.ToString,
    Pipe<string>.ToLower,
    Pipe<string>.ToConsole
);

Enumerable.Range(0, 100).Pipe(
    Pipe<int>.Select(n => n * 2),
    Pipe<int>.Where(n => n % 3 > 1),
    Enumerable.Sum,
    Convert.ToString,
    Pipe<string>.ToConsole
);
```
## [Option 2](../master/Option2)
```cs
var piped1 = 1d
    | Pipe<double>.ToTimeSpanAsDays
    | Pipe<TimeSpan>.AddHours(3)
    | Pipe<TimeSpan>.ToString
    | Pipe<string>.Split(':', '.')
    | Pipe<string>.ParseAllTo<int>()
    | Pipe<int>.Sum
    | Pipe<int>.ToString
    | Pipe<string>.ToLower
    | Pipe<string>.ToConsole;

var piped2 = Enumerable.Range(0, 100)
    | Pipe<int>.Select(n => n * 2)
    | Pipe<int>.Where(n => n % 3 > 1)
    | Pipe<int>.Sum
    | Pipe<int>.ToString
    | Pipe<string>.Join("-", "Option", "#", "2")
    | Pipe<string>.ToConsole;
```
## Option 2 without pipes
```cs
Console.WriteLine(TimeSpan
    .FromDays(1d)
    .Add(TimeSpan.FromHours(3))
    .ToString()
    .Split(':', '.')
    .Select(str => int.Parse(str))
    .Sum()
    .ToString()
    .ToLower()
);

Console.WriteLine(
    string.Join("-", new[] {
        Enumerable.Range(0, 100)
            .Select(n => n * 2)
            .Where(n => n % 3 > 1)
            .Sum()
            .ToString(),
        "Option", "#", "2"
    })
);
```