using System.Text;
using WhiteTowerGames.DataFixerSharper.Codecs;
using WhiteTowerGames.DataFixerSharper.Json;

namespace WhiteTowerGames.DataFixerSharper.Tests.Primitives;

public class PrimitiveCodecs
{
    [Fact]
    public void Int32_Builtin_Deterministic()
    {
        // Given
        var codec = BuiltinCodecs.Int32;

        // When
        var encoded = codec.Encode<JsonOps, JsonByteBuffer>(42, JsonOps.Empty()).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        // Then
        Assert.Equal(42, decoded);
    }

    [Fact]
    public void Int64_Builtin_Deterministic()
    {
        // Given
        var codec = BuiltinCodecs.Int64;

        // When
        var encoded = codec.Encode<JsonOps, JsonByteBuffer>(42L, JsonOps.Empty()).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        // Then
        Assert.Equal(42, decoded);
    }

    [Fact]
    public void Float_Builtin_Deterministic()
    {
        // Given
        var codec = BuiltinCodecs.Float;

        // When
        var encoded = codec.Encode<JsonOps, JsonByteBuffer>(42f, JsonOps.Empty()).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        // Then
        Assert.Equal(42f, decoded);
    }

    [Fact]
    public void Double_Builtin_Deterministic()
    {
        // Given
        var codec = BuiltinCodecs.Double;

        // When
        var encoded = codec.Encode<JsonOps, JsonByteBuffer>(42d, JsonOps.Empty()).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        // Then
        Assert.Equal(42d, decoded);
    }

    [Fact]
    public void Bool_Builtin_Deterministic()
    {
        // Given
        var codec = BuiltinCodecs.Bool;

        // When
        var encoded = codec.Encode<JsonOps, JsonByteBuffer>(true, JsonOps.Empty()).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        // Then
        Assert.True(decoded);
    }

    [Fact]
    public void String_Builtin_Deterministic()
    {
        // Given
        var codec = BuiltinCodecs.String;

        // When
        var encoded = codec.Encode<JsonOps, JsonByteBuffer>("banana", JsonOps.Empty()).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        // Then
        Assert.Equal("banana", decoded);
    }

    [Fact]
    public void ConstantCodec_Works()
    {
        // Given
        var codec = ICodec.Constant(42);

        // When
        var encoded = codec.Encode<JsonOps, JsonByteBuffer>(120, JsonOps.Empty()).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        Assert.Equal(Encoding.UTF8.GetBytes("{}"), encoded.Memory);
        Assert.Equal(42, decoded);
    }
}
