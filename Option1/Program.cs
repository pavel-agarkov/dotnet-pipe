using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Option1
{
    static class Program
    {
        static void Main(string[] args)
        {
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

            Console.ReadKey();
        }
    }

    public static class Pipe<TInput>
    {
        public static Func<TInput, TInput> ToConsole =>
            input =>
            {
                Console.WriteLine(input);
                return input;
            };

        public static Func<IEnumerable<TInput>, IEnumerable<TInput>> Where(Func<TInput, bool> filter) =>
            list => list.Where(filter);

        public static Func<IEnumerable<TInput>, IEnumerable<TOutput>> Select<TOutput>(Func<TInput, TOutput> selector) =>
            list => list.Select(selector);

        public static Func<IEnumerable<TInput>, int> Sum =>
            list =>
            {
                if (typeof(TInput).IsAssignableFrom(typeof(decimal)))
                {
                    return (list as IEnumerable<decimal>).Sum() as dynamic;
                }
                else if (typeof(TInput).IsAssignableFrom(typeof(int)))
                {
                    return (list as IEnumerable<int>).Sum() as dynamic;
                }
                throw Error(nameof(Sum));
            };

        public static Func<IEnumerable<string>, IEnumerable<TOutput>> ParseAllTo<TOutput>() =>
            list => list.Select(str =>
            {
                var descr = TypeDescriptor.GetConverter(typeof(TOutput));
                if (descr.CanConvertFrom(typeof(string)))
                {
                    return (TOutput)descr.ConvertFromString(str);
                }
                throw Error(nameof(ParseAllTo));
            });

        public static Func<string, string> Join(string separator, params string[] strArray) =>
            str => string.Join(separator, new[] { str }.Union(strArray));

        public static Func<string, string> ToLower =>
            str => str.ToLower();

        public static Func<string, IEnumerable<string>> Split(params char[] separators) =>
            str => str.Split(separators);

        public static Func<TInput, string> ToString =>
            input => Convert.ToString(input);

        private static ArgumentException Error(string methodName) =>
            new ArgumentException($"Method {methodName} does not support type {nameof(TInput)}");

        public static Func<TInput, TInput> AddHours(int hours) =>
            input =>
            {
                switch (input)
                {
                    case TimeSpan timeSpan:
                        return timeSpan.Add(TimeSpan.FromHours(hours)) as dynamic;

                    case DateTime dateTime:
                        return dateTime.AddHours(hours) as dynamic;

                    default:
                        throw Error(nameof(AddHours));
                }
            };

        public static Func<TInput, TimeSpan> ToTimeSpanAsDays =>
            input =>
            {
                switch (input)
                {
                    case double doubleDays:
                        return TimeSpan.FromDays(doubleDays);

                    case int intDays:
                        return TimeSpan.FromDays(intDays);

                    default:
                        throw Error(nameof(ToTimeSpanAsDays));
                }
            };
    }

    public static class PipeExtensions
    {
        public static TResult Pipe<TInput, TResult>
            (this TInput input,
                Func<TInput, TResult> func)
            => func(input);

        public static TResult1 Pipe<TInput, TResult, TResult1>
            (this TInput input,
                Func<TInput, TResult> func,
                Func<TResult, TResult1> func1)
            => func1(func(input));

        public static TResult2 Pipe<TInput, TResult, TResult1, TResult2>
            (this TInput input,
                Func<TInput, TResult> func,
                Func<TResult, TResult1> func1,
                Func<TResult1, TResult2> func2)
            => func2(func1(func(input)));

        public static TResult3 Pipe<TInput, TResult, TResult1, TResult2, TResult3>
            (this TInput input,
                Func<TInput, TResult> func,
                Func<TResult, TResult1> func1,
                Func<TResult1, TResult2> func2,
                Func<TResult2, TResult3> func3)
            => func3(func2(func1(func(input))));

        public static TResult4 Pipe<TInput, TResult, TResult1, TResult2, TResult3, TResult4>
            (this TInput input,
                Func<TInput, TResult> func,
                Func<TResult, TResult1> func1,
                Func<TResult1, TResult2> func2,
                Func<TResult2, TResult3> func3,
                Func<TResult3, TResult4> func4)
            => func4(func3(func2(func1(func(input)))));

        public static TResult5 Pipe<TInput, TResult, TResult1, TResult2, TResult3, TResult4, TResult5>
            (this TInput input,
                Func<TInput, TResult> func,
                Func<TResult, TResult1> func1,
                Func<TResult1, TResult2> func2,
                Func<TResult2, TResult3> func3,
                Func<TResult3, TResult4> func4,
                Func<TResult4, TResult5> func5)
            => func5(func4(func3(func2(func1(func(input))))));
    }
}
