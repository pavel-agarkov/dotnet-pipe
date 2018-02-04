# Naive implementations of a pipe operation in C#
This repository contains two working implementations of a pipe operation in C#.

## [Option 1](option1)
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
# [Option 2](option2)
```cs
var a = 1d
    | Pipe<double>.ToTimeSpanAsDays
    | Pipe<TimeSpan>.AddHours(3)
    | Pipe<TimeSpan>.ToString
    | Pipe<string>.Split(':', '.')
    | Pipe<string>.ParseAllTo<int>()
    | Pipe<int>.Sum
    | Pipe<int>.ToString
    | Pipe<string>.ToLower
    | Pipe<string>.ToConsole;

var b = Enumerable.Range(0, 100)
    | Pipe<int>.Select(n => n * 2)
    | Pipe<int>.Where(n => n % 3 > 1)
    | Pipe<int>.Sum
    | Pipe<int>.ToString
    | Pipe<string>.Join("-", "Test")
    | Pipe<string>.ToConsole;
```