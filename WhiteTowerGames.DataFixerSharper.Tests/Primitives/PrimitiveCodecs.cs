using System.Text;
using WhiteTowerGames.DataFixerSharper.Json;

namespace WhiteTowerGames.DataFixerSharper.Tests.Primitives;

public class PrimitiveCodecs
{
    private static readonly JsonOps JsonOps = JsonOps.Instance;

    [Fact]
    public void Int32_Builtin_Deterministic()
    {
        // Given
        var codec = BuiltinCodecs.Int32;

        // When
        var encoded = codec.EncodeStart(JsonOps, 42).GetOrThrow();
        var decoded = codec.Parse(JsonOps, encoded).GetOrThrow();

        // Then
        Assert.Equal(42, decoded);
    }

    [Fact]
    public void Int64_Builtin_Deterministic()
    {
        // Given
        var codec = BuiltinCodecs.Int64;

        // When
        var encoded = codec.EncodeStart(JsonOps, 42).GetOrThrow();
        var decoded = codec.Parse(JsonOps, encoded).GetOrThrow();

        // Then
        Assert.Equal(42, decoded);
    }

    [Fact]
    public void Float_Builtin_Deterministic()
    {
        // Given
        var codec = BuiltinCodecs.Float;

        // When
        var encoded = codec.EncodeStart(JsonOps, 42f).GetOrThrow();
        var decoded = codec.Parse(JsonOps, encoded).GetOrThrow();

        // Then
        Assert.Equal(42f, decoded);
    }

    [Fact]
    public void Double_Builtin_Deterministic()
    {
        // Given
        var codec = BuiltinCodecs.Double;

        // When
        var encoded = codec.EncodeStart(JsonOps, 42d).GetOrThrow();
        var decoded = codec.Parse(JsonOps, encoded).GetOrThrow();

        // Then
        Assert.Equal(42d, decoded);
    }

    [Fact]
    public void Bool_Builtin_Deterministic()
    {
        // Given
        var codec = BuiltinCodecs.Bool;

        // When
        var encoded = codec.EncodeStart(JsonOps, true).GetOrThrow();
        var decoded = codec.Parse(JsonOps, encoded).GetOrThrow();

        // Then
        Assert.True(decoded);
    }

    [Fact]
    public void String_Builtin_Deterministic()
    {
        // Given
        var codec = BuiltinCodecs.String;

        // When
        var encoded = codec.EncodeStart(JsonOps, "banana").GetOrThrow();
        var decoded = codec.Parse(JsonOps, encoded).GetOrThrow();

        // Then
        Assert.Equal("banana", decoded);
    }

    [Fact]
    public void ConstantCodec_Works()
    {
        // Given
        var codec = BuiltinCodecs.Int32.Constant(42);

        // When
        var encoded = codec.EncodeStart(JsonOps, 120).GetOrThrow();
        var decoded = codec.Parse(JsonOps, encoded).GetOrThrow();

        Assert.Equal(Encoding.UTF8.GetBytes("{}"), encoded);
        Assert.Equal(42, decoded);
    }
}
