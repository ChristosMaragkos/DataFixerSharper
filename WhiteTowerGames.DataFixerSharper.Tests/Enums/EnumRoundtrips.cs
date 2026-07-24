using WhiteTowerGames.DataFixerSharper.Extensions.Enums;
using WhiteTowerGames.DataFixerSharper.Json;

namespace WhiteTowerGames.DataFixerSharper.Tests.Enums;

public class EnumRoundtrips
{
    private enum CardinalDirections
    {
        North,
        South,
        East,
        West,
    }

    [Flags]
    private enum BitFlags
    {
        One = 1,
        Two = 2,
        Four = 4,
        Eight = 8,
    }

    [Fact]
    public void EnumByValue_Roundtrip_Deterministic()
    {
        // Given
        var codec = EnumCodecs.EnumByValue<CardinalDirections>();
        foreach (var value in Enum.GetValues<CardinalDirections>())
        {
            // When
            var encoded = codec.Encode<JsonOps, JsonByteBuffer>(value, JsonOps.Empty()).GetOrThrow();
            var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

            // Then
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void EnumByName_Roundtrip_Deterministic()
    {
        // Given
        var codec = EnumCodecs.EnumByName<CardinalDirections>();
        foreach (var value in Enum.GetValues<CardinalDirections>())
        {
            // When
            var encoded = codec.Encode<JsonOps, JsonByteBuffer>(value, JsonOps.Empty()).GetOrThrow();
            var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

            // Then
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void EnumByValue_HandlesFlags()
    {
        // Given
        var codec = EnumCodecs.EnumByName<BitFlags>();
        var testValue = BitFlags.One | BitFlags.Two;

        // When
        var encoded = codec.Encode<JsonOps, JsonByteBuffer>(testValue, JsonOps.Empty()).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        // Then
        Assert.Equal(testValue, decoded);
    }

    [Fact]
    public void FlagsByName_Roundtrip_HandlesFlags()
    {
        // Given
        var codec = EnumCodecs.FlagsByName<BitFlags>();
        var flagsArray = new string[] { "One", "Two" };
        var bitflags = BitFlags.One | BitFlags.Two;

        // When
        var encoded = codec.Encode<JsonOps, JsonByteBuffer>(bitflags, JsonOps.Empty()).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        // Then
        Assert.All(encoded.ToJsonArray(), node => flagsArray.Contains(node!.ToString()));
        Assert.Equal(decoded, bitflags);
    }

    [Fact]
    public void FlagsByName_Throws_ForNonFlagEnum()
    {
        // Given
        var value = CardinalDirections.North | CardinalDirections.South;

        // When, Then
        Assert.ThrowsAny<Exception>(() =>
        {
            var codec = EnumCodecs.FlagsByName<CardinalDirections>();
            codec.Encode<JsonOps, JsonByteBuffer>(value, JsonOps.Empty()).GetOrThrow();
        });
    }
}
