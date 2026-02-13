using System.Text.Json.Nodes;
using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper;

public sealed class JsonOps : IDynamicOps<JsonNode>
{
    public static JsonOps? Instance { get; private set; }

    static JsonOps()
    {
        Instance = new();
    }

    private static readonly JsonNode EmptyValue = JsonValue.Create("{}");

    public JsonNode Empty() => EmptyValue;

    public JsonNode CreateNumeric(decimal number) => JsonValue.Create(number);

    public JsonNode CreateString(string value) => JsonValue.Create(value);

    public JsonNode CreateBool(bool value) => JsonValue.Create(value);

    public DataResult<decimal> GetNumber(JsonNode input)
    {
        if (input is JsonValue v && v.TryGetValue(out decimal d))
            return DataResult<decimal>.Success(d);

        return DataResult<decimal>.Fail("Expected numeric JSON value");
    }

    public DataResult<string> GetString(JsonNode input)
    {
        if (input is JsonValue v && v.TryGetValue<string>(out var s))
            return DataResult<string>.Success(s);

        return DataResult<string>.Fail("Expected string JSON value");
    }

    public DataResult<bool> GetBool(JsonNode input)
    {
        if (input is JsonValue v && v.TryGetValue<bool>(out var b))
            return DataResult<bool>.Success(b);

        return DataResult<bool>.Fail("Expected bool JSON value");
    }

    public JsonNode CreateList(IEnumerable<JsonNode> elements)
    {
        var arr = new JsonArray();
        foreach (var e in elements)
            arr.Add(e);

        return arr;
    }

    public DataResult<IEnumerable<JsonNode>> ReadAsStream(JsonNode input)
    {
        if (input is JsonArray arr)
            return DataResult<IEnumerable<JsonNode>>.Success(arr!);

        return DataResult<IEnumerable<JsonNode>>.Fail("Expected JSON array");
    }

    public JsonNode CreateMap(IEnumerable<KeyValuePair<JsonNode, JsonNode>> map)
    {
        // PSA: JSON keys are strings
        var obj = new JsonObject();
        foreach (var kvp in map)
            obj[kvp.Key.GetValue<string>()] = kvp.Value;

        return obj;
    }

    public DataResult<IEnumerable<KeyValuePair<JsonNode, JsonNode>>> ReadAsMap(JsonNode input)
    {
        if (input is not JsonObject obj)
            return DataResult<IEnumerable<KeyValuePair<JsonNode, JsonNode>>>.Fail(
                "Expected JSON map"
            );

        return DataResult<IEnumerable<KeyValuePair<JsonNode, JsonNode>>>.Success(
            obj.Select(kvp => new KeyValuePair<JsonNode, JsonNode>(
                JsonValue.Create(kvp.Key),
                kvp.Value!
            ))
        );
    }

    public JsonNode Merge(JsonNode key, JsonNode value)
    {
        if (key is not JsonValue keyVal || !keyVal.TryGetValue<string>(out var keyStr))
            throw new InvalidOperationException(
                $"Expected a JSON string as a key, instead got {key.GetType().FullName}"
            );

        var obj = new JsonObject { [keyStr] = value };
        return obj;
    }

    public JsonNode MergeAndAppend(JsonNode map, JsonNode key, JsonNode value)
    {
        return AppendToPrefix(map, Merge(key, value));
    }

    public JsonNode AppendToPrefix(JsonNode prefix, JsonNode value)
    {
        if (value == EmptyValue || IsEmptyObject(value))
            return prefix;

        if (prefix == EmptyValue || IsEmptyObject(prefix))
            return value;

        if (prefix is JsonObject prefixObj && value is JsonObject valObj)
        {
            foreach (var kv in valObj)
                prefixObj[kv.Key] = kv.Value!;

            return prefixObj;
        }

        return value;

        bool IsEmptyObject(JsonNode node) => node is JsonObject obj && obj.Count == 0;
    }

    public JsonNode RemoveFromInput(JsonNode input, JsonNode value)
    {
        if (input is JsonObject obj && value is JsonObject valObj)
        {
            foreach (var k in valObj.Select(kvp => kvp.Key))
                obj.Remove(k);
        }

        return input;
    }
}
