using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Tests.DynamicOps;

public class OpsMutation
{
    public static IEnumerable<object[]> OpsImplementations()
    {
        yield return new object[] { JsonOps.Instance };
    }

    [Theory]
    [MemberData(nameof(OpsImplementations))]
    public void Append_EmptyValue_DoesNothing(IDynamicOps ops)
    {
        // Given, When
        var existing = ops.CreateInt32(42);
        var empty = ops.Empty();
        var result = ops.AppendToPrefix(existing, empty);

        // Then
        Assert.Equal(existing, result);
    }

    [Theory]
    [MemberData(nameof(OpsImplementations))]
    public void Append_NonEmpty_ToEmptyValue_ReturnsAppender(IDynamicOps ops)
    {
        // Given, When
        var existing = ops.Empty();
        var appender = ops.CreateInt32(42);
        var result = ops.AppendToPrefix(existing, appender);

        // Then
        Assert.Equal(appender, result);
    }
}
