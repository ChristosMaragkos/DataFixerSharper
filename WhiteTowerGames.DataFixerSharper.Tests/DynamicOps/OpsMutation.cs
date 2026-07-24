using WhiteTowerGames.DataFixerSharper.Json;

namespace WhiteTowerGames.DataFixerSharper.Tests.DynamicOps;

public class OpsMutation
{
    [Fact]
    public void Append_EmptyValue_DoesNothing()
    {
        // Given, When
        var existing = JsonOps.CreateNumeric(42);
        var empty = JsonOps.Empty();

        var result = JsonOps.AppendToPrefix(existing, empty);

        // Then
        Assert.Equal(existing.ToJsonString(), result.ToJsonString());
    }

    [Fact]
    public void Append_NonEmpty_ToEmptyValue_ReturnsAppender()
    {
        // Given, When
        var existing = JsonOps.Empty();
        var appender = JsonOps.CreateNumeric(42);
        var result = JsonOps.AppendToPrefix(existing, appender);

        // Then
        Assert.Equal(appender.ToJsonString(), result.ToJsonString());
    }
}
