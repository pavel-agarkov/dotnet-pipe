using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Option2
{
    class Program
    {
        static void Main(string[] args)
        {
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
                | Pipe<string>.Join("-", "Test")
                | Pipe<string>.ToConsole;

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
                        .ToString(),"Test"
                })
            );

            Console.ReadKey();
        }
    }

    public static class Pipe<TInput>
    {
        public static PipedFunction<TInput, TInput> ToConsole =>
            Piped.FromFunc<TInput, TInput>(input =>
            {
                Console.WriteLine(input);
                return input;
            });

        public static PipedFunction<IEnumerable<TInput>, IEnumerable<TInput>> Where(Func<TInput, bool> filter) =>
            Piped.FromFunc<IEnumerable<TInput>, IEnumerable<TInput>>(list => list.Where(filter));

        public static PipedFunction<IEnumerable<TInput>, IEnumerable<TOutput>> Select<TOutput>(Func<TInput, TOutput> selector) =>
            Piped.FromFunc<IEnumerable<TInput>, IEnumerable<TOutput>>(list => list.Select(selector));

        public static PipedFunction<IEnumerable<TInput>, int> Sum =>
            Piped.FromFunc<IEnumerable<TInput>, int>(list =>
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
            });

        public static PipedFunction<IEnumerable<string>, IEnumerable<TOutput>> ParseAllTo<TOutput>()
            => Piped.FromFunc<IEnumerable<string>, IEnumerable<TOutput>>(list => list.Select(str =>
            {
                var descr = TypeDescriptor.GetConverter(typeof(TOutput));
                if (descr.CanConvertFrom(typeof(string)))
                {
                    return (TOutput)descr.ConvertFromString(str);
                }
                throw Error(nameof(ParseAllTo));
            }));

        public static PipedFunction<string, string> Join(string separator, params string[] strToJoin) =>
            Piped.FromFunc<string, string>(str => string.Join(separator, new[] { str }.Union(strToJoin)));

        public static PipedFunction<string, string> ToLower =>
            Piped.FromFunc<string, string>(str => str.ToLower());

        public static PipedFunction<string, IEnumerable<string>> Split(params char[] separators) =>
            Piped.FromFunc<string, IEnumerable<string>>(str => str.Split(separators));

        public static PipedFunction<TInput, string> ToString =>
            Piped.FromFunc<TInput, string>(input => Convert.ToString(input));

        private static ArgumentException Error(string methodName) =>
            new ArgumentException($"Method {methodName} does not support type {nameof(TInput)}");

        public static PipedFunction<TInput, TInput> AddHours(int hours) =>
            Piped.FromFunc<TInput, TInput>(input =>
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
            });

        public static PipedFunction<TInput, TimeSpan> ToTimeSpanAsDays =>
            Piped.FromFunc<TInput, TimeSpan>(input =>
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
            });
    }

    public static class Piped
    {
        public static PipedFunction<TInput, TOutput> FromFunc<TInput, TOutput>(this Func<TInput, TOutput> func) => func;
    }

    public class PipedFunction<TInput, TOutput>
    {
        public Func<TInput, TOutput> Func { get; set; }

        public PipedFunction(Func<TInput, TOutput> func)
        {
            Func = func;
        }

        public static implicit operator PipedFunction<TInput, TOutput>(Func<TInput, TOutput> func)
        {
            return new PipedFunction<TInput, TOutput>(func);
        }

        public static implicit operator Func<TInput, TOutput>(PipedFunction<TInput, TOutput> piped)
        {
            return piped.Func;
        }

        public static TOutput operator |(TInput input, PipedFunction<TInput, TOutput> piped)
        {
            return piped.Func(input);
        }
    }
}
