using WhiteTowerGames.DataFixerSharper.Json;

namespace WhiteTowerGames.DataFixerSharper.Tests.Enums;

public class EnumCodecs
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

    private static readonly JsonOps JsonOps = JsonOps.Instance;

    [Fact]
    public void EnumByValue_Roundtrip_Deterministic()
    {
        // Given
        var codec = BuiltinCodecs.EnumByValue<CardinalDirections>();
        foreach (var value in Enum.GetValues<CardinalDirections>())
        {
            // When
            var encoded = codec.Encode(value, JsonOps, JsonOps.Empty()).GetOrThrow();
            var decoded = codec.Parse(JsonOps, encoded).GetOrThrow();

            // Then
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void EnumByName_Roundtrip_Deterministic()
    {
        // Given
        var codec = BuiltinCodecs.EnumByName<CardinalDirections>();
        foreach (var value in Enum.GetValues<CardinalDirections>())
        {
            // When
            var encoded = codec.Encode(value, JsonOps, JsonOps.Empty()).GetOrThrow();
            var decoded = codec.Parse(JsonOps, encoded).GetOrThrow();

            // Then
            Assert.Equal(value, decoded);
        }
    }

    [Fact]
    public void EnumByValue_HandlesFlags()
    {
        // Given
        var codec = BuiltinCodecs.EnumByName<BitFlags>();
        var testValue = BitFlags.One | BitFlags.Two;

        // When
        var encoded = codec.Encode(testValue, JsonOps, JsonOps.Empty()).GetOrThrow();
        var decoded = codec.Parse(JsonOps, encoded).GetOrThrow();

        // Then
        Assert.Equal(testValue, decoded);
    }

    [Fact]
    public void FlagsByName_Roundtrip_HandlesFlags()
    {
        // Given
        var codec = BuiltinCodecs.FlagsByName<BitFlags>();
        var flagsArray = new string[] { "One", "Two" };
        var bitflags = BitFlags.One | BitFlags.Two;

        // When
        var encoded = codec.Encode(bitflags, JsonOps, JsonOps.Empty()).GetOrThrow();
        var decoded = codec.Parse(JsonOps, encoded).GetOrThrow();

        // Then
        Assert.All(encoded.ToJsonArray(), node => flagsArray.Contains(node!.ToString()));
        Assert.Equal(decoded, bitflags);
    }

    [Fact]
    public void FlagsByName_Fails_ForNonFlagEnum()
    {
        // Given
        var codec = BuiltinCodecs.FlagsByName<CardinalDirections>();
        var value = CardinalDirections.North | CardinalDirections.South;

        // When
        var encoded = codec.Encode(value, JsonOps, JsonOps.Empty()).GetOrThrow();

        // Then
        Assert.True(codec.Parse(JsonOps, encoded).IsError);
    }
}
