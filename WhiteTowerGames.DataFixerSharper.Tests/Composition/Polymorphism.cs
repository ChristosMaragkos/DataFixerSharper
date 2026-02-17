using WhiteTowerGames.DataFixerSharper.Codecs;
using WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;
using WhiteTowerGames.DataFixerSharper.Json;

namespace WhiteTowerGames.DataFixerSharper.Tests.Composition;

public class Polymorphism
{
    public abstract record Shape
    {
        public abstract string Type();
    }

    public sealed record Circle(float Radius) : Shape
    {
        public override string Type() => "circle";
    }

    public sealed record Rectangle(float W, float H) : Shape
    {
        public override string Type() => "rectangle";
    }

    public static IEnumerable<object[]> Shapes()
    {
        yield return new object[] { new Circle(42f), typeof(Circle) };
        yield return new object[] { new Rectangle(10f, 20f), typeof(Rectangle) };
    }

    private static readonly JsonOps JsonOps = JsonOps.Instance;

    private static readonly ICodec<Circle> CircleCodec = RecordCodecBuilder.Create<Circle>(
        instance =>
            instance
                .WithFields(BuiltinCodecs.Float.Field((Circle circle) => circle.Radius, "radius"))
                .WithCtor(radius => new Circle(radius))
    );

    private static readonly ICodec<Rectangle> RectCodec = RecordCodecBuilder.Create<Rectangle>(
        instance =>
            instance
                .WithFields(
                    BuiltinCodecs.Float.Field((Rectangle rect) => rect.W, "width"),
                    BuiltinCodecs.Float.Field((Rectangle rect) => rect.H, "height")
                )
                .WithCtor((width, height) => new Rectangle(width, height))
    );

    private static readonly ICodec<Shape> ShapeDispatch = ICodec.Dispatch<Shape, string>(
        BuiltinCodecs.String,
        shape => shape.Type(),
        discr => CodecByType(discr)
    );

    private static ICodec<Shape> CodecByType(string discriminator)
    {
        return discriminator switch
        {
            "circle" => CircleCodec.Upcast<Circle, Shape>(),
            "rectangle" => RectCodec.Upcast<Rectangle, Shape>(),
            _ => throw new InvalidOperationException("Invalid type discriminator"),
        };
    }

    [Theory]
    [MemberData(nameof(Shapes))]
    public void PolymorphicDispatch_Roundtrip_ReturnsSameObject(Shape shape, Type shapeType)
    {
        // Given, When
        var encoded = ShapeDispatch.EncodeStart<JsonOps, JsonByteBuffer>(JsonOps, shape);
        var decoded = ShapeDispatch.Parse<JsonOps, JsonByteBuffer>(JsonOps, encoded.GetOrThrow());

        // Then
        Assert.False(encoded.IsError, encoded.ErrorMessage);
        Assert.False(decoded.IsError, decoded.ErrorMessage);
        Assert.Equal(shape, decoded.GetOrThrow());
        Assert.IsType(shapeType, decoded.GetOrThrow(), true);
    }
}
