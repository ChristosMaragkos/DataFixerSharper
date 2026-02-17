using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Json;

public sealed class JsonOps : IDynamicOps<JsonByteBuffer>
{
    public static JsonOps Instance { get; } = new();

    private JsonOps() { }

    private static JsonByteBuffer BytesFromString(string str) => Encoding.UTF8.GetBytes(str);

    private static readonly JsonByteBuffer EmptyValue = "{}"u8.ToArray(); // apparently c# lets you generate utf8-encoded strings. How long has this been a thing?
    private static readonly JsonByteBuffer EmptyArrayValue = "[]"u8.ToArray();
    private static readonly JsonByteBuffer TrueValue = "true"u8.ToArray();
    private static readonly JsonByteBuffer FalseValue = "false"u8.ToArray();

    #region Value Creation
    public JsonByteBuffer Empty() => EmptyValue;

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public JsonByteBuffer CreateNumeric(decimal number)
    {
        Span<byte> temp = stackalloc byte[32];
        if (!Utf8Formatter.TryFormat(number, temp, out var byteAmount))
            return default;

        var buffer = new byte[byteAmount];
        temp.Slice(0, byteAmount).CopyTo(buffer);

        return new JsonByteBuffer(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public JsonByteBuffer CreateString(string value) => JsonSerializer.SerializeToUtf8Bytes(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public JsonByteBuffer CreateBool(bool value) => value ? TrueValue : FalseValue;
    #endregion

    #region Value Reading
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public DataResult<decimal> GetNumber(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input, true, default);
        if (!reader.Read())
            return DataResult<decimal>.Fail("Input was empty");

        return reader.TokenType switch
        {
            JsonTokenType.Number when reader.TryGetDecimal(out var num) =>
                DataResult<decimal>.Success(num),
            _ => DataResult<decimal>.Fail($"Expected number, instead got {reader.TokenType}"),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public DataResult<string> GetString(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input, true, default);
        if (!reader.Read())
            return DataResult<string>.Fail("Input was empty");

        return reader.TokenType == JsonTokenType.String
            ? DataResult<string>.Success(reader.GetString()!)
            : DataResult<string>.Fail(
                $"Expected JSON string literal, instead got {reader.TokenType}"
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public DataResult<bool> GetBool(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input);
        if (!reader.Read())
            return DataResult<bool>.Fail("Input was empty");

        return reader.TokenType switch
        {
            JsonTokenType.True => DataResult<bool>.Success(true),
            JsonTokenType.False => DataResult<bool>.Success(false),
            _ => DataResult<bool>.Fail($"Expected boolean, instead got {reader.TokenType}"),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public DataResult<JsonByteBuffer> GetValue(JsonByteBuffer input, string name)
    {
        var reader = new Utf8JsonReader(input, true, default);

        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            return DataResult<JsonByteBuffer>.Fail(
                $"Could not fetch value under key '{name}' - input was not a JSON object."
            );

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals(name))
            {
                reader.Read(); // move to value
                var start = (int)reader.TokenStartIndex;
                reader.Skip(); // skip the entire thing
                var length = (int)reader.BytesConsumed - start; // count the bytes

                return DataResult<JsonByteBuffer>.Success(input.Memory.Slice(start, length));
            }
            reader.Skip(); // just skip for other properties
        }

        return DataResult<JsonByteBuffer>.Fail(
            $"Could not fetch value under key '{name}' - the key was not found."
        );
    }
    #endregion

    #region Enumerables
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public JsonByteBuffer CreateEmptyList()
    {
        var writer = new ArrayBufferWriter<byte>();
        writer.Write("["u8);
        return new JsonByteBuffer(writer);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public DataResult<JsonByteBuffer> AddToList(JsonByteBuffer list, JsonByteBuffer element)
    {
        if (list.Writer == null)
            return DataResult<JsonByteBuffer>.Fail(
                "Could not append value: list was read-only or finalized"
            );

        var writer = list.Writer;
        var json = new Utf8JsonWriter(writer);

        if (writer.WrittenSpan[^1] != (byte)'[') // if the last written byte was the opening bracket (i.e. the list is empty), no need for a comma
            writer.Write(","u8);

        writer.Write(element);
        json.Flush();

        return DataResult<JsonByteBuffer>.Success(list); // we return a new struct, but it points to the same memory region as the other one.
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public DataResult<Unit> ReadList<TState, TCon>(
        JsonByteBuffer input,
        ref TState state,
        TCon consumer
    )
        where TState : allows ref struct
        where TCon : ICollectionConsumer<TState, JsonByteBuffer>
    {
        var reader = new Utf8JsonReader(input, true, default);

        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return DataResult<Unit>.Fail($"Expected a JSON array, got {reader.TokenType} instead");

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            var start = (int)reader.TokenStartIndex;
            reader.Skip();
            var length = (int)reader.BytesConsumed - start;

            consumer.Accept(ref state, input.Memory.Slice(start, length));
        }

        return DataResult<Unit>.Success(default);
    }

    #endregion

    #region Maps
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public JsonByteBuffer CreateEmptyMap()
    {
        var writer = new ArrayBufferWriter<byte>();
        writer.Write("{"u8);
        return new JsonByteBuffer(writer);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public DataResult<JsonByteBuffer> AddToMap(
        JsonByteBuffer map,
        JsonByteBuffer key,
        JsonByteBuffer value
    )
    {
        if (map.Writer == null)
            return DataResult<JsonByteBuffer>.Fail(
                "Could not append to map: map was read-only or finalized."
            );

        var writer = map.Writer;
        if (writer.WrittenSpan[^1] != (byte)'{')
            writer.Write(","u8);

        writer.Write(key);
        writer.Write(":"u8);
        writer.Write(value);

        return DataResult<JsonByteBuffer>.Success(map);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public DataResult<Unit> ReadMap<TState, TCon>(
        JsonByteBuffer input,
        ref TState state,
        TCon consumer
    )
        where TState : allows ref struct
        where TCon : IMapConsumer<TState, JsonByteBuffer>
    {
        var reader = new Utf8JsonReader(input, true, default);

        if (!reader.Read() && reader.TokenType != JsonTokenType.StartObject)
            return DataResult<Unit>.Fail($"Expected a JSON object, instead got {reader.TokenType}");

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                reader.Skip();
                continue;
            }

            var keyStart = (int)reader.TokenStartIndex;
            reader.Read();

            var valStart = (int)reader.TokenStartIndex;
            reader.Skip();
            var valLength = (int)reader.BytesConsumed - valStart;

            var keyLength = (int)reader.BytesConsumed - keyStart - valLength;

            consumer.Accept(
                ref state,
                input.Memory.Slice(keyStart, keyLength),
                input.Memory.Slice(valStart, valLength)
            );
        }

        return DataResult<Unit>.Success(default);
    }
    #endregion

    #region Utils
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public JsonByteBuffer AppendToPrefix(JsonByteBuffer prefix, JsonByteBuffer value)
    {
        var finalizedValue = value;

        if (value.Writer != null)
        {
            var writer = value.Writer;

            if (writer.WrittenSpan[0] == (byte)'[')
                writer.Write("]"u8);
            else if (writer.WrittenSpan[0] == (byte)'{')
                writer.Write("}"u8);

            finalizedValue = new JsonByteBuffer(writer.WrittenMemory);
        }

        if (IsEmptyJson(in prefix))
            return finalizedValue;

        if (IsEmptyJson(in finalizedValue))
            return prefix;

        if (IsJsonArray(in prefix) && IsJsonArray(in finalizedValue))
            return MergeArrays(in prefix, in finalizedValue);

        if (IsJsonObject(in prefix) && IsJsonObject(in finalizedValue))
            return MergeObjects(in prefix, in finalizedValue);

        return finalizedValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public JsonByteBuffer RemoveFromInput(JsonByteBuffer input, JsonByteBuffer value) => input; // mutating the input while decoding is useless since our lookups are by-key

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static bool IsEmptyJson(in JsonByteBuffer buffer)
    {
        if (buffer.IsEmpty)
            return true;

        var span = buffer.Memory.Span;

        if (span.Length == 2 && span[0] == (byte)'{' && span[1] == (byte)'}')
            return true;
        if (span.Length == 2 && span[0] == (byte)'[' && span[1] == (byte)']')
            return true;

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsJsonArray(in JsonByteBuffer buffer)
    {
        return buffer.Memory.Span[0] == (byte)'[' && buffer.Memory.Span[^1] == (byte)']';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsJsonObject(in JsonByteBuffer buffer)
    {
        return buffer.Memory.Span[0] == (byte)'{' && buffer.Memory.Span[^1] == (byte)'}';
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private JsonByteBuffer MergeArrays(in JsonByteBuffer left, in JsonByteBuffer right)
    {
        if (IsEmptyJson(in left))
            return right;

        if (IsEmptyJson(in right))
            return left;

        // worst case scenario is we actually have to concat the arrays
        var leftValues = left.Memory.Span.Slice(1, left.Memory.Length - 2);
        var rightValues = right.Memory.Span.Slice(1, right.Memory.Length - 2);

        var byteAmount = 1 + leftValues.Length + 1 + rightValues.Length + 1; // [ + left + , + right + ]
        var merged = new byte[byteAmount];

        merged[0] = (byte)'[';
        merged[^1] = (byte)']';
        merged[leftValues.Length + 1] = (byte)',';

        leftValues.CopyTo(merged.AsSpan(1));
        rightValues.CopyTo(merged.AsSpan(leftValues.Length + 2));
        return new JsonByteBuffer(merged);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private JsonByteBuffer MergeObjects(in JsonByteBuffer left, in JsonByteBuffer right)
    {
        if (IsEmptyJson(in left))
            return right;

        if (IsEmptyJson(in right))
            return left;

        var leftValues = left.Memory.Span.Slice(1, left.Memory.Length - 2);
        var rightValues = right.Memory.Span.Slice(1, right.Memory.Length - 2);

        var byteAmount = 1 + leftValues.Length + 1 + rightValues.Length + 1;
        var merged = new byte[byteAmount];

        merged[0] = (byte)'{';
        merged[^1] = (byte)'}';
        merged[leftValues.Length + 1] = (byte)',';

        leftValues.CopyTo(merged.AsSpan(1));
        rightValues.CopyTo(merged.AsSpan(leftValues.Length + 2));
        return new JsonByteBuffer(merged);
    }
    #endregion
}
