using WhiteTowerGames.DataFixerSharper.Json;

namespace WhiteTowerGames.DataFixerSharper.Tests.DynamicOps;

public class OpsMutation
{
    [Fact]
    public void Append_EmptyValue_DoesNothing()
    {
        // Given, When
        var ops = JsonOps.Instance;
        var existing = ops.CreateNumeric(42);
        var empty = ops.Empty();

        var result = ops.AppendToPrefix(existing, empty);

        // Then
        Assert.Equal(existing.ToJsonString(), result.ToJsonString());
    }

    [Fact]
    public void Append_NonEmpty_ToEmptyValue_ReturnsAppender()
    {
        // Given, When
        var ops = JsonOps.Instance;
        var existing = ops.Empty();
        var appender = ops.CreateNumeric(42);
        var result = ops.AppendToPrefix(existing, appender);

        // Then
        Assert.Equal(appender.ToJsonString(), result.ToJsonString());
    }
}
