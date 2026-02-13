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
}
