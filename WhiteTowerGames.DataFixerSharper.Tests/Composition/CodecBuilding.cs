using WhiteTowerGames.DataFixerSharper.Codecs;
using WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;
using WhiteTowerGames.DataFixerSharper.Json;

namespace WhiteTowerGames.DataFixerSharper.Tests.Composition;

public class CodecBuilding
{
    private static readonly JsonOps JsonOps = JsonOps.Instance;

    public sealed record Person(string Name, int Age);

    public sealed record Relationship(Person Person1, Person Person2);

    public sealed record FriendGroup(params Person[] Members);

    public static IEnumerable<object[]> People()
    {
        yield return new object[] { new Person("John Doe", 18) };
        yield return new object[] { new Person("Jane Doe", 0) };
    }

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

    [Theory]
    [MemberData(nameof(People))]
    public void RecordCodec_RoundTrip_ReturnsSame(Person person)
    {
        // Given, When
        var encoded = PersonCodec.Encode(person, JsonOps, JsonOps.Empty());
        var decoded = PersonCodec.Decode(JsonOps, encoded.GetOrThrow());

        // Then
        Assert.False(encoded.IsError, encoded.ErrorMessage);
        Assert.False(decoded.IsError, decoded.ErrorMessage);
        Assert.Equal(person, decoded.GetOrThrow().Item1);
    }

    [Fact]
    public void RecordCodec_Deterministic_ForCollection()
    {
        // Given
        var people = new Person[] { new Person("John Doe", 18), new Person("Jane Doe", 21) };
        var peopleCodec = PersonCodec.ForArray();

        // When
        var encoded = peopleCodec.Encode(people, JsonOps, JsonOps.Empty());
        var decoded = peopleCodec.Decode(JsonOps, encoded.GetOrThrow());

        // Then
        Assert.False(encoded.IsError, encoded.ErrorMessage);
        Assert.False(
            decoded.IsError,
            decoded.ErrorMessage + $"\n{encoded.GetOrThrow().ToJsonString()}"
        );
        Assert.True(
            decoded.GetOrThrow().Item1.SequenceEqual(people),
            encoded.GetOrThrow().ToJsonString()
        );
    }

    [Theory]
    [MemberData(nameof(People))]
    public void RecordCodec_Works_WhenNested(Person person)
    {
        // Given
        var rel = new Relationship(person, person);

        // When
        var encoded = RelationshipCodec.Encode(rel, JsonOps, JsonOps.Empty());
        var decoded = RelationshipCodec.Parse(JsonOps, encoded.GetOrThrow());

        // Then
        Assert.False(encoded.IsError, encoded.ErrorMessage);
        Assert.False(decoded.IsError, decoded.ErrorMessage);
        Assert.Equal(rel, decoded.GetOrThrow());
    }

    [Theory]
    [MemberData(nameof(People))]
    public void RecordCodec_Works_WithNestedEnumerable(Person person)
    {
        // Given
        var group = new FriendGroup(person, person, person, person);

        // When
        var encoded = FriendGroupCodec.Encode(group, JsonOps, JsonOps.Empty());
        var decoded = FriendGroupCodec.Parse<JsonOps, JsonByteBuffer>(
            JsonOps,
            encoded.GetOrThrow()
        );

        // Then
        Assert.False(encoded.IsError, encoded.ErrorMessage);
        Assert.False(decoded.IsError, decoded.ErrorMessage);
        Assert.Equal(group.Members, decoded.GetOrThrow().Members);
    }
}
