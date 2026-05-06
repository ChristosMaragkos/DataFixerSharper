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

        var v1Data = JsonOps.CreateEmptyMap();
        v1Data = JsonOps
            .AddToMap(v1Data, JsonOps.CreateString("hp"), JsonOps.CreateNumeric(5m))
            .GetOrThrow();

        v1Data = JsonOps.FinalizeMap(v1Data);

        var result = fixV1_1.Apply(new Dynamic<JsonByteBuffer>(JsonOps, v1Data));

        Assert.False(result.IsError, $"Migration failed: {result.ErrorMessage}");

        var migratedData = result.GetOrThrow();
        var oldKeyCheck = JsonOps.GetValue(migratedData.Value, "hp");
        Assert.True(oldKeyCheck.IsError, "The old 'hp' key was not removed");

        var newKeyCheck = JsonOps.GetValue(migratedData.Value, "health");
        Assert.False(newKeyCheck.IsError, "The new 'health' key was not added");

        var healthValue = JsonOps.GetNumber(newKeyCheck.GetOrThrow()).GetOrThrow();
        Assert.Equal(5m, healthValue);
    }

    private record Player;

    [Fact]
    public void TimelineBuilder_Executes_Correctly()
    {
        var engine = new DataFixEngine<JsonByteBuffer>(JsonOps);
        var playerTimeline = TimelineBuilder<JsonByteBuffer>
            .Create()
            .BaseSchema(
                new Dictionary<string, ISchemaType>
                {
                    { "hp", BuiltinSchemas.Number },
                    { "name", BuiltinSchemas.String },
                }
            )
            .SinceVersion(new Version(1, 1, 0))
            .FieldRenamed("hp", "health")
            .FieldAdded("mana", BuiltinSchemas.Number, 10)
            .EndVersion()
            .SinceVersion(new Version(1, 2, 0))
            .FieldRemoved("name")
            .CustomRule(dyn =>
            {
                var healthDyn = dyn.Get("health");
                if (healthDyn.IsError)
                    return dyn;

                var health = JsonOps.GetNumber(healthDyn.GetOrThrow().Value).GetOrThrow();
                var newHealth = JsonOps.CreateNumeric(health * 2);
                return dyn.Set("health", new Dynamic<JsonByteBuffer>(JsonOps, newHealth));
            })
            .EndVersion()
            .Build<Player>();

        engine.RegisterTimeline(playerTimeline);

        var v1Data = JsonOps.CreateEmptyMap();
        v1Data = JsonOps
            .AddToMap(v1Data, JsonOps.CreateString("hp"), JsonOps.CreateNumeric(50))
            .GetOrThrow();
        v1Data = JsonOps
            .AddToMap(v1Data, JsonOps.CreateString("name"), JsonOps.CreateString("Hero"))
            .GetOrThrow();
        v1Data = JsonOps.FinalizeMap(v1Data);

        var result = engine.Migrate<Player>(new Version(1, 0, 0), new Version(1, 2, 0), v1Data);

        Assert.False(result.IsError, $"Migration Failed: {result.ErrorMessage}");
        var migrated = result.GetOrThrow();

        Assert.True(JsonOps.GetValue(migrated, "hp").IsError, "Old field 'hp' was not removed");
        Assert.True(JsonOps.GetValue(migrated, "name").IsError, "Old field 'name' was not removed");

        var manaNode = JsonOps.GetValue(migrated, "mana");
        Assert.False(manaNode.IsError, "New field 'mana' was not added");
        Assert.Equal(10m, JsonOps.GetNumber(manaNode.GetOrThrow()).GetOrThrow());

        var healthNode = JsonOps.GetValue(migrated, "health");
        Assert.False(healthNode.IsError, "New field 'health' was not added");
        Assert.Equal(100m, JsonOps.GetNumber(healthNode.GetOrThrow()).GetOrThrow());
    }
}
