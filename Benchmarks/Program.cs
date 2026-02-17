using System.Text;
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
    private static readonly Person Giannakhs = new Person("John", 10);

    private static readonly JsonOps JsonOps = JsonOps.Instance;

    public sealed record Person(string Name, int Age = 0);

    public sealed record Relationship(Person Person1, Person Person2);

    public sealed record FriendGroup(params Person[] Members);

    private static readonly ICodec<Person> PersonCodec = RecordCodecBuilder.Create<Person>(
        instance =>
            instance
                .WithFields(
                    BuiltinCodecs.String.Field((Person person) => person.Name, "Name"),
                    BuiltinCodecs.Int32.Field((Person person) => person.Age, "Age")
                )
                .WithCtor((name, age) => new Person(name, age))
    );

    private static readonly ICodec<List<int>> IntegerArrayCodec = BuiltinCodecs.Int32.ForList();
    private static readonly List<int> Integers = new() { 1, 2, 3 };

    [Benchmark]
    public void STJ_Serialize()
    {
        JsonSerializer.Serialize(Giannakhs);
    }

    [Benchmark]
    public void STJ_Serialize_IntArray()
    {
        JsonSerializer.Serialize(Integers);
    }

    [Benchmark]
    public void STJ_Deserialize()
    {
        JsonSerializer.Deserialize<Person>("""{"Name":"John","Age":10}""");
    }

    [Benchmark]
    public void STJ_Deserialize_IntArray()
    {
        JsonSerializer.Deserialize<int[]>("[1,2,3]");
    }

    [Benchmark]
    public void Codec_Serialize()
    {
        PersonCodec.EncodeStart<JsonOps, JsonByteBuffer>(JsonOps, Giannakhs);
    }

    [Benchmark]
    public void Codec_Deserialize()
    {
        PersonCodec.Parse<JsonOps, JsonByteBuffer>(JsonOps, MemoryPerson);
    }

    private static readonly JsonByteBuffer MemoryPerson = new JsonByteBuffer(
        Encoding.UTF8.GetBytes("""{"Name":"John","Age":10}""")
    );

    [Benchmark]
    public void Codec_Serialize_IntArray()
    {
        IntegerArrayCodec.EncodeStart<JsonOps, JsonByteBuffer>(JsonOps, Integers).GetOrThrow();
    }

    private static readonly JsonByteBuffer MemoryIntegers = new JsonByteBuffer(
        Encoding.UTF8.GetBytes("[1,2,3]")
    );

    [Benchmark]
    public void Codec_Deserialize_IntArray()
    {
        IntegerArrayCodec.Parse(JsonOps, MemoryIntegers);
    }
}

internal class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<CodecBenchmarks>();
        //     var bench = new CodecBenchmarks();
        //     for (var i = 0; i < 100; i++)
        //         bench.Codec_Deserialize();
        //     for (var i = 0; i < 10000; i++)
        //         bench.Codec_Deserialize();
    }
}
