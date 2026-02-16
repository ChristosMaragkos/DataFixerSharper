using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using WhiteTowerGames.DataFixerSharper.Abstractions;
using JsonByteBuffer = System.ReadOnlyMemory<byte>;

namespace WhiteTowerGames.DataFixerSharper.Json;

public sealed class JsonOps : IDynamicOps<JsonByteBuffer>
{
    public static JsonOps Instance { get; } = new();

    private JsonOps() { }

    private static JsonByteBuffer BytesFromString(string str) => Encoding.UTF8.GetBytes(str);

    private static readonly JsonByteBuffer EmptyValue = BytesFromString("{}");

    public JsonByteBuffer Empty() => EmptyValue;

    public JsonByteBuffer CreateNumeric(decimal number) =>
        BytesFromString(number.ToString(System.Globalization.CultureInfo.InvariantCulture));

    public JsonByteBuffer CreateString(string value)
    {
        using var ms = new MemoryStream();
        using (var writer = new Utf8JsonWriter(ms))
        {
            writer.WriteStringValue(value);
        }
        return ms.ToArray();
    }

    public JsonByteBuffer CreateBool(bool value) => BytesFromString(value ? "true" : "false");

    public DataResult<decimal> GetNumber(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input.Span, true, default);
        if (reader.TokenType == JsonTokenType.None && !reader.Read())
            return DataResult<decimal>.Fail("Invalid numeric input: JSON string was empty");

        if (
            reader.TokenType == JsonTokenType.Number
            || reader.Read() && reader.TokenType == JsonTokenType.Number
        )
        {
            if (reader.TryGetDecimal(out var result))
                return DataResult<decimal>.Success(result);

            return DataResult<decimal>.Fail("Invalid numeric input: not a number.");
        }

        return DataResult<decimal>.Fail($"Expected a JSON number, instead got {reader.TokenType}");
    }

    public DataResult<string> GetString(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input.Span, true, default);
        if (reader.TokenType == JsonTokenType.None && !reader.Read())
            return DataResult<string>.Fail("Invalid string input: no JSON string was found.");

        if (
            reader.TokenType == JsonTokenType.String
            || reader.Read() && reader.TokenType == JsonTokenType.String
        )
            return DataResult<string>.Success(reader.GetString()!);

        return DataResult<string>.Fail(
            $"Expected a JSON string literal, instead got {reader.TokenType}"
        );
    }

    public DataResult<bool> GetBool(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input.Span, true, default);
        if (reader.TokenType == JsonTokenType.None && !reader.Read())
            return DataResult<bool>.Fail("Invalid boolean input: no JSON string was found.");

        if (reader.TokenType == JsonTokenType.True)
            return DataResult<bool>.Success(true);
        if (reader.TokenType == JsonTokenType.False)
            return DataResult<bool>.Success(false);

        if (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.True)
                return DataResult<bool>.Success(true);
            if (reader.TokenType == JsonTokenType.False)
                return DataResult<bool>.Success(false);
        }

        return DataResult<bool>.Fail($"Expected a boolean value, instead got {reader.TokenType}");
    }

    public DataResult<JsonByteBuffer> GetValue(JsonByteBuffer input, string name)
    {
        var reader = new Utf8JsonReader(input.Span);

        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            return DataResult<JsonByteBuffer>.Fail(
                $"Could not fetch value under key {name} - input was not a JSON object."
            );

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals(name))
            {
                reader.Read();
                var start = (int)reader.TokenStartIndex;
                reader.Skip();
                var length = (int)reader.BytesConsumed - start;

                return DataResult<JsonByteBuffer>.Success(input.Slice(start, length));
            }

            reader.Skip();
        }

        return DataResult<JsonByteBuffer>.Fail(
            $"Could not fetch value under key {name} - key not found."
        );
    }

    public JsonByteBuffer CreateList(IEnumerable<JsonByteBuffer> elements)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);

        writer.WriteStartArray();
        foreach (var item in elements)
            writer.WriteRawValue(item.Span, true);

        writer.WriteEndArray();
        writer.Flush();
        return buffer.WrittenMemory;
    }

    public DataResult<IEnumerable<JsonByteBuffer>> ReadAsStream(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input.Span, true, default);

        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return DataResult<IEnumerable<JsonByteBuffer>>.Fail(
                "Could not construct list: Input was not a JSON array"
            );

        var elements = new List<JsonByteBuffer>();

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            var start = (int)reader.TokenStartIndex;
            reader.Skip();
            var length = (int)reader.BytesConsumed - start;

            elements.Add(input.Slice(start, length));
        }

        return DataResult<IEnumerable<JsonByteBuffer>>.Success(elements);
    }

    public JsonByteBuffer CreateMap(IEnumerable<KeyValuePair<JsonByteBuffer, JsonByteBuffer>> map)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);

        writer.WriteStartObject();

        foreach (var kvp in map)
        {
            var key = DecodeJsonString(kvp.Key);
            writer.WritePropertyName(key);
            writer.WriteRawValue(kvp.Value.Span, true);
        }

        writer.WriteEndObject();
        writer.Flush();
        return buffer.WrittenMemory;
    }

    public DataResult<IEnumerable<KeyValuePair<JsonByteBuffer, JsonByteBuffer>>> ReadAsMap(
        JsonByteBuffer input
    )
    {
        var reader = new Utf8JsonReader(input.Span, true, default);

        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            return DataResult<IEnumerable<KeyValuePair<JsonByteBuffer, JsonByteBuffer>>>.Fail(
                "Could not construct map: input was not a JSON object."
            );

        var pairs = new List<KeyValuePair<JsonByteBuffer, JsonByteBuffer>>();

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                reader.Skip();
                continue;
            }

            var keyStart = (int)reader.TokenStartIndex;
            reader.Read(); // move to value

            var valStart = (int)reader.TokenStartIndex;
            reader.Skip();
            var valLength = (int)reader.BytesConsumed - valStart;

            var keyLength = (int)reader.BytesConsumed - keyStart - valLength;

            pairs.Add(
                new KeyValuePair<JsonByteBuffer, JsonByteBuffer>(
                    input.Slice(keyStart, keyLength),
                    input.Slice(valStart, valLength)
                )
            );
        }

        return DataResult<IEnumerable<KeyValuePair<JsonByteBuffer, JsonByteBuffer>>>.Success(pairs);
    }

    public JsonByteBuffer Merge(JsonByteBuffer key, JsonByteBuffer value)
    {
        // JSON keys must be strings
        var keyString = DecodeJsonString(key);
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);

        writer.WriteStartObject();
        writer.WritePropertyName(keyString);
        writer.WriteRawValue(value.Span, true);
        writer.WriteEndObject();
        writer.Flush();

        return buffer.WrittenMemory;
    }

    public JsonByteBuffer AppendToPrefix(JsonByteBuffer prefix, JsonByteBuffer value)
    {
        if (IsEmptyObject(prefix))
            return value;
        if (IsEmptyObject(value))
            return prefix;

        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);

        writer.WriteStartObject();
        WriteProperties(prefix); // this just rewrites the two objects as one. I can't be bothered to slice and merge.
        WriteProperties(value);
        writer.WriteEndObject();
        writer.Flush();

        return buffer.WrittenMemory;

        void WriteProperties(JsonByteBuffer objectSlice)
        {
            var reader = new Utf8JsonReader(objectSlice.Span, true, default);
            if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
                return;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    reader.Skip();
                    continue;
                }

                var keyString = reader.GetString()!;

                reader.Read();
                var valStart = (int)reader.TokenStartIndex;
                reader.Skip();
                var valLength = (int)reader.BytesConsumed - valStart;

                writer.WritePropertyName(keyString);
                writer.WriteRawValue(objectSlice.Slice(valStart, valLength).Span, true);
            }
        }
    }

    public JsonByteBuffer MergeAndAppend(
        JsonByteBuffer map,
        JsonByteBuffer key,
        JsonByteBuffer value
    )
    {
        return AppendToPrefix(map, Merge(key, value));
    }

    public JsonByteBuffer RemoveFromInput(JsonByteBuffer input, JsonByteBuffer value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);

        // Collect keys to remove
        var keysToRemove = new List<byte[]>();
        var valReader = new Utf8JsonReader(value.Span, true, default);
        if (valReader.Read() && valReader.TokenType == JsonTokenType.StartObject)
        {
            while (valReader.Read() && valReader.TokenType != JsonTokenType.EndObject)
            {
                if (valReader.TokenType == JsonTokenType.PropertyName)
                    keysToRemove.Add(valReader.ValueSpan.ToArray());

                valReader.Skip();
            }
        }

        // Iterate input and write only keys not in remove list
        var inputReader = new Utf8JsonReader(input.Span, true, default);
        if (!inputReader.Read() || inputReader.TokenType != JsonTokenType.StartObject)
            return input; // not an object, nothing to remove

        writer.WriteStartObject();

        while (inputReader.Read() && inputReader.TokenType != JsonTokenType.EndObject)
        {
            if (inputReader.TokenType != JsonTokenType.PropertyName)
                continue;

            var keySpan = inputReader.ValueSpan;

            inputReader.Read(); // move to value
            var valStart = (int)inputReader.TokenStartIndex;
            inputReader.Skip();
            var valLength = (int)inputReader.BytesConsumed - valStart;

            if (!IsKeyMatch(keysToRemove, keySpan))
            {
                writer.WritePropertyName(keySpan);
                writer.WriteRawValue(input.Slice(valStart, valLength).Span, true);
            }
        }

        writer.WriteEndObject();
        writer.Flush();
        return buffer.WrittenMemory;

        static bool IsKeyMatch(List<byte[]> keys, ReadOnlySpan<byte> currentKey)
        {
            foreach (var key in keys)
                if (currentKey.SequenceEqual(key))
                    return true;
            return false;
        }
    }

    private static string DecodeJsonString(JsonByteBuffer buffer)
    {
        var reader = new Utf8JsonReader(buffer.Span, true, default);
        if (!reader.Read() || reader.TokenType != JsonTokenType.String)
            throw new InvalidOperationException("Input buffer was not a JSON-encoded string.");

        return reader.GetString()!;
    }

    private static bool IsObject(JsonByteBuffer buffer)
    {
        if (buffer.IsEmpty)
            return false;

        var r = new Utf8JsonReader(buffer.Span, true, default);
        return r.Read() && r.TokenType == JsonTokenType.StartObject;
    }

    private static bool IsEmptyObject(JsonByteBuffer buffer)
    {
        if (!IsObject(buffer))
            return false;

        var r = new Utf8JsonReader(buffer.Span, true, default);
        r.Read(); // StartObject
        return r.Read() && r.TokenType == JsonTokenType.EndObject;
    }
}

public static class JsonByteBufferExtensions
{
    public static string ToJsonString(this JsonByteBuffer buffer) =>
        Encoding.UTF8.GetString(buffer.Span);

    public static JsonDocument ToJsonDocument(this JsonByteBuffer buffer) =>
        JsonDocument.Parse(buffer.ToArray());

    public static JsonNode ToJsonNode(this JsonByteBuffer buffer) =>
        JsonNode.Parse(ToJsonDocument(buffer).RootElement.GetRawText())!;

    public static JsonObject ToJsonObject(this JsonByteBuffer buffer) =>
        ToJsonNode(buffer).AsObject()
        ?? throw new InvalidOperationException("JSON byte buffer was not a valid JSON object.");

    public static JsonArray ToJsonArray(this JsonByteBuffer buffer) =>
        ToJsonNode(buffer).AsArray()
        ?? throw new InvalidOperationException("JSON byte buffer was not a valid JSON array.");
}
