using WhiteTowerGames.DataFixerSharper.Codecs;

namespace WhiteTowerGames.DataFixerSharper.Tests.Collections;

public class CollectionCodecs
{
    private static readonly JsonOps JsonOps = JsonOps.Instance;

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(new int[] { })]
    public void CollectionCodec_Roundrtrip_DoesNotMutate(params int[] numbers)
    {
        // Given
        var codec = BuiltinCodecs.Int32.ForEnumerable();

        // When
        var encoded = codec.EncodeStart(JsonOps, numbers).GetOrThrow();
        var decoded = codec.Parse(JsonOps, encoded).GetOrThrow();

        // Then
        Assert.True(numbers.SequenceEqual(decoded));
    }

    [Fact]
    public void DictionaryCodec_Roundtrip_Works()
    {
        // Given
        var dict = new Dictionary<string, int>() { { "zero", 0 }, { "one", 1 } };
        var codec = Codec.Dictionary<string, int>(BuiltinCodecs.String, BuiltinCodecs.Int32);

        // When
        var encoded = codec.EncodeStart(JsonOps, dict);
        var decoded = codec.Parse(JsonOps, encoded.GetOrThrow());
        var value = decoded.GetOrThrow();

        // Then
        Assert.False(encoded.IsError);
        Assert.False(decoded.IsError);
        Assert.True(dict.SequenceEqual(value));
    }
}
