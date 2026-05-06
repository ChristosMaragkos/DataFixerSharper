using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using WhiteTowerGames.DataFixerSharper;
using WhiteTowerGames.DataFixerSharper.Codecs;
using WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;
using WhiteTowerGames.DataFixerSharper.Json;

namespace Benchmarks;

[MemoryDiagnoser]
public class CodecBenchmarks
{
    private static readonly Person Giannakhs = new(
        "John",
        ["WoW"],
        "Unemployed",
        0,
        "McDonalds",
        10
    );

    private static readonly JsonOps JsonOps = JsonOps.Instance;

    public sealed record Person(
        string Name,
        string[] Hobbies,
        string Job,
        int NumberOfFriends,
        string FavoriteFood,
        int Age = 0
    );

    private static readonly ICodec<Person> PersonCodec = RecordCodecBuilder.Create<Person>(
        instance =>
            instance
                .WithFields(
                    BuiltinCodecs.String.Field((Person person) => person.Name, "Name"),
                    BuiltinCodecs
                        .String.ForArray()
                        .Field((Person person) => person.Hobbies, "Hobbies"),
                    BuiltinCodecs.String.Field((Person person) => person.Job, "Job"),
                    BuiltinCodecs.Int32.Field(
                        (Person person) => person.NumberOfFriends,
                        "NumberOfFriends"
                    ),
                    BuiltinCodecs.String.Field(
                        (Person person) => person.FavoriteFood,
                        "FavoriteFood"
                    ),
                    BuiltinCodecs.Int32.Field((Person person) => person.Age, "Age")
                )
                .WithCtor(
                    (name, hobbies, job, numOfFriends, favFood, age) =>
                        new Person(name, hobbies, job, numOfFriends, favFood, age)
                )
    );

    private static readonly ICodec<List<int>> IntegerArrayCodec = BuiltinCodecs.Int32.ForList();
    private static readonly List<int> Integers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];

    [Benchmark]
#pragma warning disable CA1822 // Mark members as static
    public void STJ_Serialize()
#pragma warning restore CA1822 // Mark members as static
    {
        JsonSerializer.Serialize(Giannakhs);
    }

    [Benchmark]
#pragma warning disable CA1822 // Mark members as static
    public void STJ_Serialize_IntArray()
#pragma warning restore CA1822 // Mark members as static
    {
        JsonSerializer.Serialize(Integers);
    }

    [Benchmark]
#pragma warning disable CA1822 // Mark members as static
    public void STJ_Deserialize()
#pragma warning restore CA1822 // Mark members as static
    {
        JsonSerializer.Deserialize<Person>(
            """{"Name":"John","Hobbies": ["Wow"], "Job": "Unemployed", "NumberOfFriends":0, "FavoriteFood":"McDonalds","Age":10}"""
        );
    }

    [Benchmark]
#pragma warning disable CA1822 // Mark members as static
    public void STJ_Deserialize_IntArray()
#pragma warning restore CA1822 // Mark members as static
    {
        JsonSerializer.Deserialize<int[]>("[1,2,3,4,5,6,7,8,9,10,11,12]");
    }

    [Benchmark]
#pragma warning disable CA1822 // Mark members as static
    public void Codec_Serialize()
#pragma warning restore CA1822 // Mark members as static
    {
        PersonCodec.EncodeStart<JsonOps, JsonByteBuffer>(JsonOps, Giannakhs);
    }

    [Benchmark]
#pragma warning disable CA1822 // Mark members as static
    public void Codec_Serialize_IntArray()
#pragma warning restore CA1822 // Mark members as static
    {
        IntegerArrayCodec.EncodeStart<JsonOps, JsonByteBuffer>(JsonOps, Integers).GetOrThrow();
    }

    [Benchmark]
#pragma warning disable CA1822 // Mark members as static
    public void Codec_Deserialize()
#pragma warning restore CA1822 // Mark members as static
    {
        PersonCodec.Parse(JsonOps, MemoryPerson);
    }

    private static readonly JsonByteBuffer MemoryPerson =
        """{"Name":"John","Hobbies": ["Wow"], "Job": "Unemployed", "NumberOfFriends":0, "FavoriteFood":"McDonalds","Age":10}"""u8.ToArray();

    private static readonly JsonByteBuffer MemoryIntegers =
        "[1,2,3,4,5,6,7,8,9,10,11,12]"u8.ToArray();

    [Benchmark]
#pragma warning disable CA1822 // Mark members as static
    public void Codec_Deserialize_IntArray()
#pragma warning restore CA1822 // Mark members as static
    {
        IntegerArrayCodec.Parse(JsonOps, MemoryIntegers);
    }
}

internal class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<CodecBenchmarks>();
    }
}
