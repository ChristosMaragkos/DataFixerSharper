using WhiteTowerGames.DataFixerSharper.Datafixers;
using WhiteTowerGames.DataFixerSharper.Json;
using WhiteTowerGames.DataFixerSharper.Schemas;
using WhiteTowerGames.DataFixerSharper.Versioning;

namespace WhiteTowerGames.DataFixerSharper.Tests.DataFixing;

public class Migration
{
    private static readonly JsonOps JsonOps = JsonOps.Instance;

    [Fact]
    public void Engine_RecordMigration_RenamesFieldCorrectly()
    {
        var engine = new DataFixEngine<JsonByteBuffer>(JsonOps);

        var playerSchema = new RecordSchema(
            new Dictionary<string, ISchemaType> { { "hp", BuiltinSchemas.Number } }
        );

        Func<Dynamic<JsonByteBuffer>, DynamicResult<JsonByteBuffer>> rule = dyn =>
            dyn.Rename("hp", "health");

        var fixV1_1 = new SchemaDrivenFix<JsonByteBuffer>(new Version(1, 1), playerSchema, rule);

        engine.RegisterDatafix(fixV1_1);

        var v1Data = JsonOps.CreateEmptyMap();
        v1Data = JsonOps
            .AddToMap(v1Data, JsonOps.CreateString("hp"), JsonOps.CreateNumeric(5m))
            .GetOrThrow();

        v1Data = JsonOps.FinalizeMap(v1Data);

        var from = new Version(1, 0);
        var to = new Version(2, 0);

        var result = engine.Migrate(from, to, v1Data);

        Assert.False(result.IsError, $"Migration failed: {result.ErrorMessage}");

        var migratedData = result.GetOrThrow();
        var oldKeyCheck = JsonOps.GetValue(migratedData, "hp");
        Assert.True(oldKeyCheck.IsError, "The old 'hp' key was not removed");

        var newKeyCheck = JsonOps.GetValue(migratedData, "health");
        Assert.False(newKeyCheck.IsError, "The new 'health' key was not added");

        var healthValue = JsonOps.GetNumber(newKeyCheck.GetOrThrow()).GetOrThrow();
        Assert.Equal(5m, healthValue);
    }
}
