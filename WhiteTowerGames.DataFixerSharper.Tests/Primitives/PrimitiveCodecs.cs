using System.Text;
using WhiteTowerGames.DataFixerSharper.Codecs;
using WhiteTowerGames.DataFixerSharper.Json;

namespace WhiteTowerGames.DataFixerSharper.Tests.Primitives;

public class PrimitiveCodecs
{
    [Fact]
    public void Int32_Builtin_Deterministic()
    {
        var codec = BuiltinCodecs.Int32;

        var encoded = codec.EncodeStart<JsonOps, JsonByteBuffer>(42).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        Assert.Equal(42, decoded);
    }

    [Fact]
    public void Int64_Builtin_Deterministic()
    {
        var codec = BuiltinCodecs.Int64;

        var encoded = codec.EncodeStart<JsonOps, JsonByteBuffer>(42L).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        Assert.Equal(42, decoded);
    }

    [Fact]
    public void Float_Builtin_Deterministic()
    {
        var codec = BuiltinCodecs.Float;

        var encoded = codec.EncodeStart<JsonOps, JsonByteBuffer>(42f).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        Assert.Equal(42f, decoded);
    }

    [Fact]
    public void Double_Builtin_Deterministic()
    {
        var codec = BuiltinCodecs.Double;

        var encoded = codec.EncodeStart<JsonOps, JsonByteBuffer>(42d).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        Assert.Equal(42d, decoded);
    }

    [Fact]
    public void Bool_Builtin_Deterministic()
    {
        var codec = BuiltinCodecs.Bool;

        var encoded = codec.EncodeStart<JsonOps, JsonByteBuffer>(true).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        Assert.True(decoded);
    }

    [Fact]
    public void String_Builtin_Deterministic()
    {
        var codec = BuiltinCodecs.String;

        var encoded = codec.EncodeStart<JsonOps, JsonByteBuffer>("banana").GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        Assert.Equal("banana", decoded);
    }

    [Fact]
    public void ConstantCodec_Works()
    {
        var codec = ICodec.Constant(42);

        var encoded = codec.EncodeStart<JsonOps, JsonByteBuffer>(120).GetOrThrow();
        var decoded = codec.Parse<JsonOps, JsonByteBuffer>(encoded).GetOrThrow();

        Assert.Equal(Encoding.UTF8.GetBytes("{}"), encoded.Memory);
        Assert.Equal(42, decoded);
    }
}