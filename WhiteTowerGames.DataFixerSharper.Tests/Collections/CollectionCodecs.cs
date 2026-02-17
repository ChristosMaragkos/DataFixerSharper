using WhiteTowerGames.DataFixerSharper.Codecs;
using WhiteTowerGames.DataFixerSharper.Json;

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
        var codec = BuiltinCodecs.Int32.ForArray();

        // When
        var encoded = codec.Encode(numbers, JsonOps, JsonOps.Empty());
        var decoded = codec.Parse(JsonOps, encoded.GetOrThrow());

        // Then
        Assert.False(encoded.IsError, encoded.ErrorMessage);
        Assert.False(decoded.IsError, decoded.ErrorMessage);
        Assert.True(numbers.SequenceEqual(decoded.GetOrThrow()));
    }

    [Theory]
    [InlineData(new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 })]
    [InlineData(new int[] { }, new int[] { 4, 5, 6 })]
    public void Append_Collection_ToCollection_Merges(int[] numbersFirst, int[] numbersSecond)
    {
        // Given
        var codec = BuiltinCodecs.Int32.ForArray();
        var merged = numbersFirst.Concat(numbersSecond).ToArray();

        // When
        var encodedFirst = codec.Encode(numbersFirst, JsonOps, JsonOps.Empty());
        var encodedSecond = codec.Encode(numbersSecond, JsonOps, encodedFirst.GetOrThrow());
        var encodedMerged = codec.Encode(merged, JsonOps, JsonOps.Empty());

        var decoded = codec.Parse(JsonOps, encodedSecond.GetOrThrow());
        var decodedMerged = codec.Parse(JsonOps, encodedMerged.GetOrThrow());

        // Then
        Assert.False(encodedSecond.IsError, encodedSecond.ErrorMessage);
        Assert.False(decoded.IsError, decoded.ErrorMessage);
        Assert.False(decodedMerged.IsError, decodedMerged.ErrorMessage);
        Assert.True(
            decoded.GetOrThrow().SequenceEqual(decodedMerged.GetOrThrow()),
            $"Expected: {ConcatSequence(decodedMerged.GetOrThrow())}\nGot: {ConcatSequence(decoded.GetOrThrow())}"
        );
    }

    [Fact]
    public void DictionaryCodec_Roundtrip_Works()
    {
        // Given
        var dict = new Dictionary<string, int>() { { "zero", 0 }, { "one", 1 } };
        var codec = ICodec.Dictionary<string, int>(BuiltinCodecs.String, BuiltinCodecs.Int32);

        // When
        var encoded = codec.Encode(dict, JsonOps, JsonOps.Empty());
        var decoded = codec.Parse(JsonOps, encoded.GetOrThrow());
        var value = decoded.GetOrThrow();

        // Then
        Assert.False(encoded.IsError);
        Assert.False(decoded.IsError);
        Assert.True(dict.SequenceEqual(value));
    }

    private static string ConcatSequence(int[] sequence)
    {
        var str = "";
        foreach (var item in sequence)
            str += item.ToString() + " ";

        return str;
    }
}
