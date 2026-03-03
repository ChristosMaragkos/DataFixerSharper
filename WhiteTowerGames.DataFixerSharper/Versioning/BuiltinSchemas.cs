using WhiteTowerGames.DataFixerSharper.Schemas;

namespace WhiteTowerGames.DataFixerSharper.Versioning;

public static class BuiltinSchemas
{
    public static readonly PrimitiveSchema Primitive = new();

    public static readonly PrimitiveSchema Number = Primitive;
    public static readonly PrimitiveSchema Boolean = Primitive;
    public static readonly PrimitiveSchema String = Primitive;
}
