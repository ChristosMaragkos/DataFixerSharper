using System.Buffers;
using System.Buffers.Text;
using System.Text;
using System.Text.Json;
using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Json;

public sealed class JsonOps : IDynamicOps<JsonByteBuffer>
{
    public static JsonOps Instance { get; } = new();

    private JsonOps() { }

    private static JsonByteBuffer BytesFromString(string str) => Encoding.UTF8.GetBytes(str);

    #region Pre-allocated strings
    private static readonly JsonByteBuffer EmptyValue = "{}"u8.ToArray(); // apparently c# lets you generate utf8-encoded strings. How long has this been a thing?
    private static readonly JsonByteBuffer EmptyArrayValue = "[]"u8.ToArray();
    private static readonly JsonByteBuffer TrueValue = "true"u8.ToArray();
    private static readonly JsonByteBuffer FalseValue = "false"u8.ToArray();

    private static readonly JsonByteBuffer ArrayOpen = "["u8.ToArray();
    private static readonly JsonByteBuffer ArrayClose = "]"u8.ToArray();
    private static readonly JsonByteBuffer ObjectOpen = "{"u8.ToArray();
    private static readonly JsonByteBuffer ObjectClose = "}"u8.ToArray();
    private static readonly JsonByteBuffer Comma = ","u8.ToArray();
    private static readonly JsonByteBuffer Colon = ":"u8.ToArray();

    private const string NumNotFound = "Could not fetch numeric value - the value was not found";
    private const string BoolNotFound = "Could not fetch boolean value - the value was not found";
    private const string StringNotFound = "Could not fetch string value - the value was not found";
    private const string KeyNotFound = "Could not fetch keyed value - the key was not found";
    private const string EmptyInput = "Input was empty.";
    private const string ImmutableList = "Could not append value: list was read-only or finalized";
    private const string ImmutableMap = "Could not append to map: map was read-only or finalized.";
    #endregion

    #region Value Creation
    public JsonByteBuffer Empty() => EmptyValue;

    public JsonByteBuffer CreateNumeric(decimal number)
    {
        Span<byte> temp = stackalloc byte[32];
        if (!Utf8Formatter.TryFormat(number, temp, out var byteAmount))
            return default;

        var buffer = new byte[byteAmount];
        temp.Slice(0, byteAmount).CopyTo(buffer);

        return new JsonByteBuffer(buffer);
    }

    public JsonByteBuffer CreateString(string value) => JsonSerializer.SerializeToUtf8Bytes(value);

    public JsonByteBuffer CreateBool(bool value) => value ? TrueValue : FalseValue;
    #endregion

    #region Value Reading
    public DataResult<decimal> GetNumber(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input, true, default);
        if (!reader.Read())
            return DataResult<decimal>.Fail(EmptyInput);

        return reader.TokenType switch
        {
            JsonTokenType.Number when reader.TryGetDecimal(out var num) =>
                DataResult<decimal>.Success(num),
            _ => DataResult<decimal>.Fail(NumNotFound),
        };
    }

    public DataResult<string> GetString(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input, true, default);
        if (!reader.Read())
            return DataResult<string>.Fail(EmptyInput);

        return reader.TokenType == JsonTokenType.String
            ? DataResult<string>.Success(reader.GetString()!)
            : DataResult<string>.Fail(StringNotFound);
    }

    public DataResult<bool> GetBool(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input);
        if (!reader.Read())
            return DataResult<bool>.Fail(EmptyInput);

        return reader.TokenType switch
        {
            JsonTokenType.True => DataResult<bool>.Success(true),
            JsonTokenType.False => DataResult<bool>.Success(false),
            _ => DataResult<bool>.Fail(BoolNotFound),
        };
    }

    public DataResult<JsonByteBuffer> GetValue(JsonByteBuffer input, string name)
    {
        var reader = new Utf8JsonReader(input, true, default);

        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            return DataResult<JsonByteBuffer>.Fail(KeyNotFound);

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

        return DataResult<JsonByteBuffer>.Fail(KeyNotFound);
    }
    #endregion

    #region Enumerables
    public JsonByteBuffer CreateEmptyList()
    {
        var writer = new ArrayBufferWriter<byte>();
        writer.Write(ArrayOpen);
        return new JsonByteBuffer(writer);
    }

    public DataResult<JsonByteBuffer> AddToList(JsonByteBuffer list, JsonByteBuffer element)
    {
        if (list.Writer == null)
            return DataResult<JsonByteBuffer>.Fail(ImmutableList);

        var writer = list.Writer;

        if (writer.WrittenSpan[^1] != (byte)'[') // if the last written byte was the opening bracket (i.e. the list is empty), no need for a comma
            writer.Write(Comma);

        writer.Write(element);

        return DataResult<JsonByteBuffer>.Success(list); // we return a new struct, but it points to the same memory region as the other one.
    }

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

    public JsonByteBuffer FinalizeList(JsonByteBuffer list)
    {
        if (list.Writer == null)
            return list;

        list.Writer.Write(ArrayClose);
        return new JsonByteBuffer(list.Writer.WrittenMemory);
    }
    #endregion

    #region Maps
    public JsonByteBuffer CreateEmptyMap()
    {
        var writer = new ArrayBufferWriter<byte>();
        writer.Write(ObjectOpen);
        return new JsonByteBuffer(writer);
    }

    public DataResult<JsonByteBuffer> AddToMap(
        JsonByteBuffer map,
        JsonByteBuffer key,
        JsonByteBuffer value
    )
    {
        if (map.Writer == null)
            return DataResult<JsonByteBuffer>.Fail(ImmutableMap);

        var writer = map.Writer;
        if (writer.WrittenSpan[^1] != (byte)'{')
            writer.Write(Comma);

        writer.Write(key);
        writer.Write(Colon);
        writer.Write(value);

        return DataResult<JsonByteBuffer>.Success(map);
    }

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

        while (reader.Read() || reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                reader.Skip();
                continue;
            }

            var keyString = reader.GetString()!;
            var keyBuffer = CreateString(keyString);

            reader.Read();
            var valStart = (int)reader.TokenStartIndex;
            reader.Skip();
            var valLength = (int)reader.BytesConsumed - valStart;

            consumer.Accept(ref state, keyBuffer, input.Memory.Slice(valStart, valLength));
        }

        return DataResult<Unit>.Success(default);
    }

    public JsonByteBuffer FinalizeMap(JsonByteBuffer map)
    {
        if (map.Writer == null)
            return map;

        map.Writer.Write(ObjectClose);
        return new JsonByteBuffer(map.Writer.WrittenMemory);
    }
    #endregion

    #region Utils
    public JsonByteBuffer AppendToPrefix(JsonByteBuffer prefix, JsonByteBuffer value)
    {
        var finalizedValue = value;

        if (value.Writer != null)
        {
            var writer = value.Writer;

            if (writer.WrittenSpan[0] == (byte)'[')
                writer.Write(ArrayClose);
            else if (writer.WrittenSpan[0] == (byte)'{')
                writer.Write(ObjectClose);

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

    public JsonByteBuffer RemoveFromInput(JsonByteBuffer input, string valueKey) => input; // mutating the input while decoding is useless since our lookups are by-key

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

    private static bool IsJsonArray(in JsonByteBuffer buffer)
    {
        return buffer.Memory.Span[0] == (byte)'[' && buffer.Memory.Span[^1] == (byte)']';
    }

    private static bool IsJsonObject(in JsonByteBuffer buffer)
    {
        return buffer.Memory.Span[0] == (byte)'{' && buffer.Memory.Span[^1] == (byte)'}';
    }

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
