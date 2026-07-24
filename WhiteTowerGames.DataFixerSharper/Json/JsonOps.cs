using System.Buffers.Text;
using System.Text;
using System.Text.Json;
using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Json;

public sealed class JsonOps : IDynamicOps<JsonByteBuffer>
{
    [ThreadStatic]
    private static Stack<PooledJsonWriter>? _writerPool;

    private static PooledJsonWriter RentWriter()
    {
        var stack = _writerPool ??= new Stack<PooledJsonWriter>(4);
        if (stack.Count > 0)
        {
            var w = stack.Pop();
            w.Reset();
            return w;
        }
        return new PooledJsonWriter();
    }

    private static void ReturnWriter(PooledJsonWriter writer) => _writerPool!.Push(writer);

    #region Pre-allocated constants
    private static readonly JsonByteBuffer EmptyValue = "{}"u8.ToArray();
    private static readonly JsonByteBuffer TrueValue = "true"u8.ToArray();
    private static readonly JsonByteBuffer FalseValue = "false"u8.ToArray();

    private static readonly JsonByteBuffer ObjectOpen = "{"u8.ToArray();
    private static readonly JsonByteBuffer ObjectClose = "}"u8.ToArray();
    private static readonly JsonByteBuffer ArrayOpen = "["u8.ToArray();
    private static readonly JsonByteBuffer ArrayClose = "]"u8.ToArray();
    private static readonly JsonByteBuffer Comma = ","u8.ToArray();
    private static readonly JsonByteBuffer Colon = ":"u8.ToArray();

    private const string NumNotFound = "Could not fetch numeric value - the value was not found";
    private const string BoolNotFound = "Could not fetch boolean value - the value was not found";
    private const string StringNotFound = "Could not fetch string value - the value was not found";
    private const string KeyNotFound = "Could not fetch keyed value - the key was not found";
    private const string EmptyInput = "Input was empty.";
    #endregion
    public static JsonByteBuffer Empty() => EmptyValue;

    public static JsonByteBuffer CreateNumeric(decimal number)
    {
        var buf = CreateEmptyBuffer();
        WriteDecimal(buf, number);
        return FinalizeBuffer(buf);
    }

    public static JsonByteBuffer CreateString(string value)
    {
        var buf = CreateEmptyBuffer();
        WriteString(buf, value);
        return FinalizeBuffer(buf);
    }

    public static JsonByteBuffer CreateBool(bool value) => value ? TrueValue : FalseValue;

    public static JsonByteBuffer CreateEmptyBuffer()
    {
        var writer = RentWriter();
        return new JsonByteBuffer(writer);
    }

    public static JsonByteBuffer FinalizeBuffer(JsonByteBuffer buf)
    {
        if (buf.Writer == null)
            return buf;

        var writer = buf.Writer;
        var result = new JsonByteBuffer(writer.WrittenSpan.ToArray());
        ReturnWriter(writer);
        return result;
    }

    public static void WriteInteger(JsonByteBuffer target, long value)
    {
        Span<byte> temp = stackalloc byte[32];
        Utf8Formatter.TryFormat(value, temp, out var written);
        target.Writer!.Write(temp[..written]);
    }

    public static void WriteIntegerUnsigned(JsonByteBuffer target, ulong value)
    {
        Span<byte> temp = stackalloc byte[32];
        Utf8Formatter.TryFormat(value, temp, out var written);
        target.Writer!.Write(temp[..written]);
    }

    public static void WriteDouble(JsonByteBuffer target, double value)
    {
        Span<byte> temp = stackalloc byte[32];
        Utf8Formatter.TryFormat(value, temp, out var written);
        target.Writer!.Write(temp[..written]);
    }

    public static void WriteDecimal(JsonByteBuffer target, decimal value)
    {
        Span<byte> temp = stackalloc byte[32];
        if (!Utf8Formatter.TryFormat(value, temp, out var written))
            return;

        target.Writer!.Write(temp[..written]);
    }

    public static void WriteString(JsonByteBuffer target, string value)
    {
        target.Writer!.WriteEscapedJsonString(value.AsSpan());
    }

    public static void WriteBool(JsonByteBuffer target, bool value)
    {
        target.Writer!.Write(value ? TrueValue : FalseValue);
    }

    public static void WriteMapStart(JsonByteBuffer target)
    {
        target.Writer!.Write(ObjectOpen);
    }

    public static void WriteMapEnd(JsonByteBuffer target)
    {
        target.Writer!.Write(ObjectClose);
    }

    public static void WriteListStart(JsonByteBuffer target)
    {
        target.Writer!.Write(ArrayOpen);
    }

    public static void WriteListEnd(JsonByteBuffer target)
    {
        target.Writer!.Write(ArrayClose);
    }

    public static void WriteKey(JsonByteBuffer target, JsonByteBuffer key)
    {
        var writer = target.Writer!;
        if (writer.WrittenSpan[^1] != (byte)'{')
            writer.Write(Comma);
        writer.Write(key);
        writer.Write(Colon);
    }

    public static void WriteListSeparator(JsonByteBuffer target)
    {
        target.Writer!.Write(Comma);
    }

    public static void WriteContent(JsonByteBuffer target, JsonByteBuffer finalizedValue)
    {
        target.Writer!.Write(finalizedValue);
    }

    public static JsonByteBuffer CreateEmptyList()
    {
        var writer = RentWriter();
        writer.Write(ArrayOpen);
        return new JsonByteBuffer(writer);
    }

    public static DataResult<JsonByteBuffer> AddToList(JsonByteBuffer list, JsonByteBuffer element)
    {
        if (list.Writer == null)
            return DataResult<JsonByteBuffer>.Fail("Could not append value: list was read-only or finalized");

        var writer = list.Writer;
        if (writer.WrittenSpan[^1] != (byte)'[')
            writer.Write(Comma);
        writer.Write(element);
        return DataResult<JsonByteBuffer>.Success(list);
    }

    public static JsonByteBuffer FinalizeList(JsonByteBuffer list)
    {
        if (list.Writer == null)
            return list;

        var writer = list.Writer;
        writer.Write(ArrayClose);
        var result = new JsonByteBuffer(writer.WrittenSpan.ToArray());
        ReturnWriter(writer);
        return result;
    }

    public static JsonByteBuffer CreateEmptyMap()
    {
        var writer = RentWriter();
        writer.Write(ObjectOpen);
        return new JsonByteBuffer(writer);
    }

    public static DataResult<JsonByteBuffer> AddToMap(JsonByteBuffer map, JsonByteBuffer key, JsonByteBuffer value)
    {
        if (map.Writer == null)
            return DataResult<JsonByteBuffer>.Fail("Could not append to map: map was read-only or finalized.");

        var writer = map.Writer;
        if (writer.WrittenSpan[^1] != (byte)'{')
            writer.Write(Comma);
        writer.Write(key);
        writer.Write(Colon);
        writer.Write(value);
        return DataResult<JsonByteBuffer>.Success(map);
    }

    public static JsonByteBuffer FinalizeMap(JsonByteBuffer map)
    {
        if (map.Writer == null)
            return map;

        var writer = map.Writer;
        writer.Write(ObjectClose);
        var result = new JsonByteBuffer(writer.WrittenSpan.ToArray());
        ReturnWriter(writer);
        return result;
    }

    public static DataResult<decimal> GetNumber(JsonByteBuffer input)
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

    public static DataResult<string> GetString(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input, true, default);
        if (!reader.Read())
            return DataResult<string>.Fail(EmptyInput);

        return reader.TokenType == JsonTokenType.String
            ? DataResult<string>.Success(reader.GetString()!)
            : DataResult<string>.Fail(StringNotFound);
    }

    public static DataResult<bool> GetBool(JsonByteBuffer input)
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

    public static DataResult<JsonByteBuffer> GetValue(JsonByteBuffer input, string name)
    {
        var reader = new Utf8JsonReader(input, true, default);

        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            return DataResult<JsonByteBuffer>.Fail(KeyNotFound);

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals(name))
            {
                reader.Read();
                var start = (int)reader.TokenStartIndex;
                reader.Skip();
                var length = (int)reader.BytesConsumed - start;

                return DataResult<JsonByteBuffer>.Success(input.Memory.Slice(start, length));
            }
            reader.Skip();
        }

        return DataResult<JsonByteBuffer>.Fail(KeyNotFound);
    }

    public static DataResult<Unit> ReadList<TState, TCon>(
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

    public static DataResult<Unit> ReadMap<TState, TCon>(
        JsonByteBuffer input,
        ref TState state,
        TCon consumer
    )
        where TState : allows ref struct
        where TCon : IMapConsumer<TState, JsonByteBuffer>
    {
        var reader = new Utf8JsonReader(input, true, default);

        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            return DataResult<Unit>.Fail($"Expected a JSON object, instead got {reader.TokenType}");

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                reader.Skip();
                continue;
            }

            var keyStart = (int)reader.TokenStartIndex;
            var keyLength = (int)reader.BytesConsumed - keyStart - 1;

            var keyBuffer = new JsonByteBuffer(input.Memory.Slice(keyStart, keyLength));

            reader.Read();
            var valStart = (int)reader.TokenStartIndex;
            reader.Skip();
            var valLength = (int)reader.BytesConsumed - valStart;

            consumer.Accept(ref state, keyBuffer, input.Memory.Slice(valStart, valLength));
        }

        return DataResult<Unit>.Success(default);
    }

    public static JsonByteBuffer AppendToPrefix(JsonByteBuffer prefix, JsonByteBuffer value)
    {
        var finalizedValue = value;

        if (value.Writer != null)
        {
            var writer = value.Writer;
            byte firstByte = writer.WrittenSpan[0];

            if (firstByte == (byte)'[')
                writer.Write(ArrayClose);
            else if (firstByte == (byte)'{')
                writer.Write(ObjectClose);

            finalizedValue = new JsonByteBuffer(writer.WrittenSpan.ToArray());
            ReturnWriter(writer);
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

    public static JsonByteBuffer RemoveFromInput(JsonByteBuffer input, string valueKey) => input;

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
        var span = buffer.Memory.Span;
        return span.Length >= 2 && span[0] == (byte)'[' && span[^1] == (byte)']';
    }

    private static bool IsJsonObject(in JsonByteBuffer buffer)
    {
        var span = buffer.Memory.Span;
        return span.Length >= 2 && span[0] == (byte)'{' && span[^1] == (byte)'}';
    }

    private static JsonByteBuffer MergeArrays(in JsonByteBuffer left, in JsonByteBuffer right)
    {
        if (IsEmptyJson(in left))
            return right;

        if (IsEmptyJson(in right))
            return left;

        var leftValues = left.Memory.Span.Slice(1, left.Memory.Length - 2);
        var rightValues = right.Memory.Span.Slice(1, right.Memory.Length - 2);

        var byteAmount = 1 + leftValues.Length + 1 + rightValues.Length + 1;
        var merged = new byte[byteAmount];

        merged[0] = (byte)'[';
        merged[^1] = (byte)']';
        merged[leftValues.Length + 1] = (byte)',';

        leftValues.CopyTo(merged.AsSpan(1));
        rightValues.CopyTo(merged.AsSpan(leftValues.Length + 2));
        return new JsonByteBuffer(merged);
    }

    private static JsonByteBuffer MergeObjects(in JsonByteBuffer left, in JsonByteBuffer right)
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

    public static bool StringsMatch(JsonByteBuffer key, string targetKey)
    {
        var span = key.Memory.Span;

        if (span.Length >= 2 && span[0] == (byte)'"' && span[^1] == (byte)'"')
        {
            span = span[1..^1];
        }

        var byteAmount = Encoding.UTF8.GetMaxByteCount(targetKey.Length);
        Span<byte> buffer = byteAmount <= 512 ? stackalloc byte[byteAmount] : new byte[byteAmount];

        var actualBytes = Encoding.UTF8.GetBytes(targetKey, buffer);

        return span.SequenceEqual(buffer[0..actualBytes]);
    }
}
