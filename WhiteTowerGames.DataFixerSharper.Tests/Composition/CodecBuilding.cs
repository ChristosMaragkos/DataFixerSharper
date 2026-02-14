using WhiteTowerGames.DataFixerSharper.Codecs;
using WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;

namespace WhiteTowerGames.DataFixerSharper.Tests.Composition;

public class CodecBuilding
{
    private static readonly JsonOps JsonOps = JsonOps.Instance;

    public sealed record Person(string Name, int Age = 0);

    public static IEnumerable<object[]> People()
    {
        yield return new object[] { new Person("John Doe", 18) };
        yield return new object[] { new Person("Jane Doe") };
    }

    private static readonly Codec<Person> PersonCodec = RecordCodecBuilder.Create<Person>(
        instance =>
            instance
                .WithFields(
                    BuiltinCodecs.String.Field((Person person) => person.Name, "Name"),
                    BuiltinCodecs.Int32.OptionalField((Person person) => person.Age, "Age", 0)
                )
                .WithCtor((name, age) => new Person(name, age))
    );

    [Theory]
    [MemberData(nameof(People))]
    public void RecordCodec_RoundTrip_ReturnsSame(Person person)
    {
        // Given, When
        var encoded = PersonCodec.EncodeStart(JsonOps, person);
        var decoded = PersonCodec.Parse(JsonOps, encoded.GetOrThrow());

        // Then
        Assert.False(encoded.IsError, encoded.ErrorMessage);
        Assert.False(decoded.IsError, decoded.ErrorMessage);
        Assert.Equal(person, decoded.GetOrThrow());
    }
}
