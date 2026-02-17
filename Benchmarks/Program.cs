using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
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
                    BuiltinCodecs.Int32.OptionalField((Person person) => person.Age, "Age", 0)
                )
                .WithCtor((name, age) => new Person(name, age))
    );

    private static readonly ICodec<Relationship> RelationshipCodec =
        RecordCodecBuilder.Create<Relationship>(instance =>
            instance
                .WithFields(
                    PersonCodec.Field((Relationship rel) => rel.Person1, "Person_1"),
                    PersonCodec.Field((Relationship rel) => rel.Person2, "Person_2")
                )
                .WithCtor((p1, p2) => new Relationship(p1, p2))
        );

    private static readonly ICodec<FriendGroup> FriendGroupCodec =
        RecordCodecBuilder.Create<FriendGroup>(instance =>
            instance
                .WithFields(
                    PersonCodec.ForArray().Field((FriendGroup group) => group.Members, "members")
                )
                .WithCtor(people => new FriendGroup(people))
        );

    private static readonly ICodec<int[]> IntegerArrayCodec = BuiltinCodecs.Int32.ForArray();
    private static readonly int[] Integers = new int[] { 1, 2, 3 };

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
        PersonCodec.EncodeStart<JsonOps, ReadOnlyMemory<byte>>(JsonOps, Giannakhs);
    }

    [Benchmark]
    public void Codec_Deserialize()
    {
        PersonCodec.Parse<JsonOps, ReadOnlyMemory<byte>>(JsonOps, MemoryPerson);
    }

    private static readonly ReadOnlyMemory<byte> MemoryPerson = new ReadOnlyMemory<byte>(
        Encoding.UTF8.GetBytes("""{"Name":"John","Age":10}""")
    );

    private static readonly ReadOnlyMemory<byte> MemoryIntegers = new ReadOnlyMemory<byte>(
        Encoding.UTF8.GetBytes("{[1,2,3]}")
    );

    [Benchmark]
    public void Codec_Serialize_IntArray()
    {
        IntegerArrayCodec.EncodeStart<JsonOps, ReadOnlyMemory<byte>>(JsonOps, Integers);
    }

    [Benchmark]
    public void Codec_Deserialize_IntArray()
    {
        IntegerArrayCodec.Parse<JsonOps, ReadOnlyMemory<byte>>(JsonOps, MemoryIntegers);
    }
}

internal class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<CodecBenchmarks>();
    }
}
