using System.Numerics;
using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Codecs;
using WhiteTowerGames.DataFixerSharper.Json;

namespace WhiteTowerGames.DataFixerSharper.Tests.CodecMapping;

public class Mappers
{
    [Fact]
    public void Vector3_ToFloatArray_Succeeds()
    {
        // Given
        ICodec<Vector3> codec = BuiltinCodecs
            .Float.ForArray()
            .Unsafe2SafeMap(
                vec => new float[] { vec.X, vec.Y, vec.Z },
                arr =>
                    arr.Length != 3
                        ? DataResult<Vector3>.Fail(
                            $"Expected array with 3 items to convert to Vector3, found {arr.Length} items instead"
                        )
                        : DataResult<Vector3>.Success(new Vector3(arr[0], arr[1], arr[2]))
            );
        var vec = new Vector3(1f, 2f, 3f);

        // When
        var encoded = codec.EncodeStart<JsonOps, JsonByteBuffer>(vec).GetOrThrow();
        var encodedArray = encoded
            .ToJsonArray()
            .Select(node => float.Parse(node!.ToJsonString()))
            .ToArray();

        Assert.True(
            encodedArray[0] == vec.X && encodedArray[1] == vec.Y && encodedArray[2] == vec.Z
        );
    }

    [Fact]
    public void ValidFloatArray_ToVector3_Succeeds()
    {
        // Given
        ICodec<Vector3> vectorCodec = BuiltinCodecs
            .Float.ForArray()
            .Unsafe2SafeMap(
                vec => new float[] { vec.X, vec.Y, vec.Z },
                arr =>
                    arr.Length != 3
                        ? DataResult<Vector3>.Fail(
                            $"Expected array with 3 items to convert to Vector3, found {arr.Length} items instead"
                        )
                        : DataResult<Vector3>.Success(new Vector3(arr[0], arr[1], arr[2]))
            );
        ICodec<float[]> floatArrayCodec = BuiltinCodecs.Float.ForArray();
        float[] array = { 1f, 2f, 3f };

        // When
        var encoded = floatArrayCodec.EncodeStart<JsonOps, JsonByteBuffer>(array).GetOrThrow();
        var decoded = vectorCodec.Parse<JsonOps, JsonByteBuffer>(encoded);

        // Then
        Assert.False(decoded.IsError);
        Assert.Equal(decoded.GetOrThrow(), new Vector3(1f, 2f, 3f));
    }

    [Fact]
    public void InvalidFloatArray_ToVector3_Fails()
    {
        // Given
        ICodec<Vector3> vectorCodec = BuiltinCodecs
            .Float.ForArray()
            .Unsafe2SafeMap(
                vec => new float[] { vec.X, vec.Y, vec.Z },
                arr =>
                    arr.Length != 3
                        ? DataResult<Vector3>.Fail(
                            $"Expected array with 3 items to convert to Vector3, found {arr.Length} items instead"
                        )
                        : DataResult<Vector3>.Success(new Vector3(arr[0], arr[1], arr[2]))
            );
        ICodec<float[]> floatArrayCodec = BuiltinCodecs.Float.ForArray();
        float[] array = { 1f, 2f, 3f, 4f };

        // When
        var encoded = floatArrayCodec.EncodeStart<JsonOps, JsonByteBuffer>(array).GetOrThrow();
        var decoded = vectorCodec.Parse<JsonOps, JsonByteBuffer>(encoded);

        // Then
        Assert.True(decoded.IsError);
    }
}
