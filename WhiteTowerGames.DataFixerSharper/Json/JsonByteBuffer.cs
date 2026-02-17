using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace WhiteTowerGames.DataFixerSharper.Json;

public readonly struct JsonByteBuffer
{
    public readonly ReadOnlyMemory<byte> Memory; // If we are decoding, this holds the data
    internal readonly ArrayBufferWriter<byte>? Writer; // If we are encoding, this holds the writer

    public JsonByteBuffer(ReadOnlyMemory<byte> memory)
    {
        Memory = memory;
        Writer = null;
    }

    public JsonByteBuffer(ArrayBufferWriter<byte> writer)
    {
        Memory = default;
        Writer = writer;
    }

    public bool IsEmpty => Memory.IsEmpty && Writer == null;

    public static implicit operator ReadOnlyMemory<byte>(JsonByteBuffer buffer) => buffer.Memory;

    public static implicit operator JsonByteBuffer(ReadOnlyMemory<byte> memory) =>
        new JsonByteBuffer(memory);

    public static implicit operator JsonByteBuffer(byte[] bytes) => new JsonByteBuffer(bytes);

    public static implicit operator ReadOnlySpan<byte>(JsonByteBuffer buffer) => buffer.Memory.Span;

    public string ToJsonString() => Encoding.UTF8.GetString(this);

    public JsonDocument ToJsonDocument() => JsonDocument.Parse(this);

    public JsonNode ToJsonNode() => JsonNode.Parse(ToJsonDocument().RootElement.GetRawText())!;

    public JsonObject ToJsonObject() =>
        ToJsonNode().AsObject()
        ?? throw new InvalidOperationException("JSON byte buffer was not a valid JSON object.");

    public JsonArray ToJsonArray() =>
        ToJsonNode().AsArray()
        ?? throw new InvalidOperationException("JSON byte buffer was not a valid JSON array.");
}
