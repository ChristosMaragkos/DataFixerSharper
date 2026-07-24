using WhiteTowerGames.DataFixerSharper.Abstractions;
using WhiteTowerGames.DataFixerSharper.Datafixers;
using WhiteTowerGames.DataFixerSharper.Json;
using WhiteTowerGames.DataFixerSharper.Schemas;
using WhiteTowerGames.DataFixerSharper.Versioning;

namespace WhiteTowerGames.DataFixerSharper.Tests.DataFixing;

public class Migration
{
    [Fact]
    public void Dynamic_AddsFieldCorrectly()
    {
        var buf = JsonOps.CreateEmptyMap();
        buf = JsonOps.FinalizeMap(buf);

        var emptyMap = JsonOps.CreateEmptyMap();
        emptyMap = JsonOps.FinalizeMap(emptyMap);

        var dyn = new Dynamic<JsonOps, JsonByteBuffer>(buf)
            .Set("stats", emptyMap)
            .Get("stats")
            .Set("mana", JsonOps.CreateNumeric(10));

        var manaResult = dyn.Get("stats").Get("mana");

        Assert.False(manaResult.IsError, manaResult.ErrorMessage);

        var mana = DynamicOpsExtensions.GetInt32<JsonOps, JsonByteBuffer>(manaResult.GetOrThrow().Value).GetOrElse(0);

        Assert.Equal(10, mana);
    }

    [Fact]
    public void Engine_RecordMigration_RenamesFieldCorrectly()
    {
        var engine = new DataFixEngine<JsonOps, JsonByteBuffer>();

        var playerSchema = new RecordSchema(
            new Dictionary<string, ISchemaType> { { "hp", BuiltinSchemas.Number } }
        );

        static DynamicResult<JsonOps, JsonByteBuffer> rule(Dynamic<JsonOps, JsonByteBuffer> dyn) =>
            dyn.Rename("hp", "health");

        var fixV1_1 = new SchemaDrivenFix<JsonOps, JsonByteBuffer>(new Version(1, 1), playerSchema, rule);

        var v1Data = JsonOps.CreateEmptyMap();
        v1Data = JsonOps
            .AddToMap(v1Data, JsonOps.CreateString("hp"), JsonOps.CreateNumeric(5m))
            .GetOrThrow();

        v1Data = JsonOps.FinalizeMap(v1Data);

        var result = fixV1_1.Apply(new Dynamic<JsonOps, JsonByteBuffer>(v1Data));

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
        var engine = new DataFixEngine<JsonOps, JsonByteBuffer>();
        var playerTimeline = TimelineBuilder<JsonOps, JsonByteBuffer>
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
                return dyn.Set("health", newHealth);
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
