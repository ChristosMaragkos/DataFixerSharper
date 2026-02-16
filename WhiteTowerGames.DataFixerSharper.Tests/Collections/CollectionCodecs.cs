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
        var codec = BuiltinCodecs.Int32.ForEnumerable();

        // When
        var encoded = codec.EncodeStart(JsonOps, numbers);
        var decoded = codec.Parse(JsonOps, encoded.GetOrThrow());

        // Then
        Assert.False(encoded.IsError, encoded.ErrorMessage);
        Assert.False(decoded.IsError, decoded.ErrorMessage);
        Assert.True(numbers.SequenceEqual(decoded.GetOrThrow()));
    }

    [Theory]
    [InlineData(new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 })]
    [InlineData(new int[] { }, new int[] { 4, 5, 6 })]
    public void Append_CollectionToCollection_CreatesTwoCollections(
        int[] numbersFirst,
        int[] numbersSecond
    )
    {
        var codec = BuiltinCodecs.Int32.ForArray();

        var encodedFirst = codec.EncodeStart(JsonOps, numbersFirst);
        var encodedSecond = codec.Encode(numbersSecond, JsonOps, encodedFirst.GetOrThrow());
        var decodedFirst = codec.Decode(JsonOps, encodedSecond.GetOrThrow());
        var decodedSecond = codec.Parse(JsonOps, decodedFirst.GetOrThrow().Item2);

        Assert.False(encodedFirst.IsError, encodedFirst.ErrorMessage);
        Assert.False(encodedSecond.IsError, encodedSecond.ErrorMessage);
        Assert.False(decodedFirst.IsError, decodedFirst.ErrorMessage);
        Assert.False(decodedSecond.IsError, decodedSecond.ErrorMessage);
        Assert.True(
            decodedFirst.GetOrThrow().Item1.SequenceEqual(numbersFirst),
            $"Expected: {ConcatSequence(numbersFirst)}\n Got: {decodedFirst.GetOrThrow().Item1}"
        );
        Assert.True(decodedSecond.GetOrThrow().SequenceEqual(numbersSecond));
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

    private static string ConcatSequence(int[] sequence)
    {
        var str = "";
        foreach (var item in sequence)
            str += item.ToString() + " ";

        return str;
    }
}
