using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;
using JsonByteBuffer = System.ReadOnlyMemory<byte>;

namespace WhiteTowerGames.DataFixerSharper.Json;

public sealed class JsonOps : IDynamicOps<JsonByteBuffer>
{
    public static JsonOps Instance { get; } = new();

    private JsonOps() { }

    private static JsonByteBuffer BytesFromString(string str) => Encoding.UTF8.GetBytes(str);

    private static readonly JsonByteBuffer EmptyValue = BytesFromString("{}");
    private static readonly JsonByteBuffer EmptyArrayValue = BytesFromString("[]");
    private static readonly JsonByteBuffer TrueValue = Encoding.UTF8.GetBytes("true");
    private static readonly JsonByteBuffer FalseValue = Encoding.UTF8.GetBytes("false");

    public JsonByteBuffer Empty() => EmptyValue;

    public JsonByteBuffer CreateNumeric(decimal number)
    {
        return JsonSerializer.SerializeToUtf8Bytes(number);
    }

    public JsonByteBuffer CreateString(string value) => JsonSerializer.SerializeToUtf8Bytes(value);

    public JsonByteBuffer CreateBool(bool value) => value ? TrueValue : FalseValue;

    public DataResult<decimal> GetNumber(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input.Span, true, default);
        if (!reader.Read())
            return DataResult<decimal>.Fail("Input was empty");

        return reader.TokenType switch
        {
            JsonTokenType.Number when reader.TryGetDecimal(out var num) =>
                DataResult<decimal>.Success(num),
            _ => DataResult<decimal>.Fail($"Expected number, instead got {reader.TokenType}"),
        };
    }

    public DataResult<string> GetString(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input.Span, true, default);
        if (!reader.Read())
            return DataResult<string>.Fail("Input was empty");

        return reader.TokenType == JsonTokenType.String
            ? DataResult<string>.Success(reader.GetString()!)
            : DataResult<string>.Fail(
                $"Expected JSON string literal, instead got {reader.TokenType}"
            );
    }

    public DataResult<bool> GetBool(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input.Span);
        if (!reader.Read())
            return DataResult<bool>.Fail("Input was empty");

        return reader.TokenType switch
        {
            JsonTokenType.True => DataResult<bool>.Success(true),
            JsonTokenType.False => DataResult<bool>.Success(false),
            _ => DataResult<bool>.Fail($"Expected boolean, instead got {reader.TokenType}"),
        };
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

    public DataResult<IEnumerable<JsonByteBuffer>> ReadList(JsonByteBuffer input)
    {
        var reader = new Utf8JsonReader(input.Span, true, default);

        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
            return DataResult<IEnumerable<JsonByteBuffer>>.Fail(
                $"Could not construct list: Expected JSON array, got {reader.TokenType} instead"
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

    public DataResult<IEnumerable<KeyValuePair<JsonByteBuffer, JsonByteBuffer>>> ReadMap(
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
        return input; // mutating the input while decoding is useless since our lookups are by-key
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

    private static bool IsArray(JsonByteBuffer buffer)
    {
        if (buffer.IsEmpty)
            return false;

        var r = new Utf8JsonReader(buffer.Span, true, default);
        return r.Read() && r.TokenType == JsonTokenType.StartArray;
    }

    private static bool IsEmptyArray(JsonByteBuffer buffer)
    {
        if (!IsArray(buffer))
            return false;

        var r = new Utf8JsonReader(buffer.Span, true, default);
        r.Read(); // StartArray
        return r.Read() && r.TokenType == JsonTokenType.EndArray;
    }

    private class JsonRecordBuilder : IRecordBuilder<JsonByteBuffer>
    {
        private readonly List<KeyValuePair<JsonByteBuffer, JsonByteBuffer>> _fields = new();
        private DataResult<JsonByteBuffer> _errorState = DataResult<JsonByteBuffer>.Success(
            JsonOps.Instance.Empty()
        );

        public IRecordBuilder<JsonByteBuffer> Add(JsonByteBuffer key, JsonByteBuffer value)
        {
            _fields.Add(new KeyValuePair<JsonByteBuffer, JsonByteBuffer>(key, value));
            return this;
        }

        public IRecordBuilder<JsonByteBuffer> Add(
            JsonByteBuffer key,
            DataResult<JsonByteBuffer> valueResult
        )
        {
            if (valueResult.IsError)
                _errorState = DataResult<JsonByteBuffer>.Fail(valueResult.ErrorMessage);
            else
                _fields.Add(
                    new KeyValuePair<JsonByteBuffer, JsonByteBuffer>(key, valueResult.GetOrThrow())
                );

            return this;
        }

        public DataResult<JsonByteBuffer> Build(JsonByteBuffer prefix)
        {
            if (_errorState.IsError)
                return _errorState;
            if (_fields.Count == 0)
                return DataResult<JsonByteBuffer>.Success(prefix);

            // EXACTLY ONE buffer and writer allocation for the entire record
            var buffer = new ArrayBufferWriter<byte>();
            using var writer = new Utf8JsonWriter(buffer);

            writer.WriteStartObject();

            if (!IsEmptyObject(prefix))
                WriteJsonObject(prefix, writer);

            foreach (var kvp in _fields)
            {
                var keyString = DecodeJsonString(kvp.Key);
                writer.WritePropertyName(keyString);
                writer.WriteRawValue(kvp.Value.Span, true);
            }

            writer.WriteEndObject();
            writer.Flush();

            return DataResult<JsonByteBuffer>.Success(buffer.WrittenMemory);
        }
    }

    private class JsonArrayBuilder : IListBuilder<JsonByteBuffer>
    {
        private readonly List<JsonByteBuffer> _elements = new();
        private DataResult<JsonByteBuffer> _errorState = DataResult<JsonByteBuffer>.Success(
            JsonOps.Instance.Empty()
        );

        public IListBuilder<JsonByteBuffer> Add(JsonByteBuffer value)
        {
            _elements.Add(value);
            return this;
        }

        public IListBuilder<JsonByteBuffer> Add(DataResult<JsonByteBuffer> value)
        {
            if (value.IsError)
                _errorState = value;
            else
                _elements.Add(value.GetOrThrow());
            return this;
        }

        public DataResult<JsonByteBuffer> Build(JsonByteBuffer prefix)
        {
            if (_errorState.IsError)
                return _errorState;
            if (_elements.Count == 0)
            {
                if (prefix.IsEmpty || IsEmptyObject(prefix))
                    return DataResult<JsonByteBuffer>.Success(EmptyArrayValue);

                return DataResult<JsonByteBuffer>.Success(prefix);
            }

            var buffer = new ArrayBufferWriter<byte>();
            using var writer = new Utf8JsonWriter(buffer);

            if (!IsEmptyObject(prefix))
                WriteJsonObject(prefix, writer);
            else if (!IsEmptyArray(prefix))
                WriteJsonArray(prefix, writer);

            writer.WriteStartArray();
            foreach (var item in _elements)
                writer.WriteRawValue(item.Span);

            writer.WriteEndArray();
            writer.Flush();
            return DataResult<JsonByteBuffer>.Success(buffer.WrittenMemory);
        }
    }

    private static void WriteJsonObject(JsonByteBuffer obj, Utf8JsonWriter writer)
    {
        var reader = new Utf8JsonReader(obj.Span, true, default);
        if (reader.Read() && reader.TokenType == JsonTokenType.StartObject)
        {
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    reader.Skip();
                    continue;
                }

                var keyString = reader.GetString()!;

                reader.Read(); // Move to the value
                var valStart = (int)reader.TokenStartIndex;
                reader.Skip(); // Fast-forward over the entire value token (even if it's a nested object)
                var valLength = (int)reader.BytesConsumed - valStart;

                writer.WritePropertyName(keyString);
                writer.WriteRawValue(obj.Slice(valStart, valLength).Span, true);
            }
        }
    }

    private static void WriteJsonArray(JsonByteBuffer array, Utf8JsonWriter writer)
    {
        var reader = new Utf8JsonReader(array.Span, true, default);
        if (reader.Read() && reader.TokenType == JsonTokenType.StartArray)
        {
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                var valStart = (int)reader.TokenStartIndex;
                reader.Skip();
                var valLength = (int)reader.BytesConsumed - valStart;

                writer.WriteRawValue(array.Slice(valStart, valLength).Span, true);
            }
        }
    }

    public IRecordBuilder<JsonByteBuffer> MapBuilder() => new JsonRecordBuilder();

    public IListBuilder<JsonByteBuffer> ListBuilder() => new JsonArrayBuilder();
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
