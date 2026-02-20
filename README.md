# DataFixerSharper

DataFixerSharper is a C# reimplementation of Mojang's [DataFixerUpper](https://www.github.com/Mojang/DataFixerUpper) Java library. It is designed to be used as a:
- Format-agnostic
- Bidirectional
- Composable

Serialization layer. What does that mean? It means you can create `Codec`s for simple classes and combine them like Lego bricks to build more complex `Codec`s.

There is a multitude of Codec types that range from simple to slightly less so, but the concept is relatively easy to grasp, especially if you've made Minecraft Java mods before.
The main entrypoint is the `ICodec<T>`. A codec knows how to leverage data in order to serialize to and from specific formats, like JSON.
There are built-in codecs for most primitive types, found in `BuiltinCodecs`, which can be used to incrementally create codecs for more complex classes by combining them and applying transformations on them.
For example, you can:
- Create an `ICodec<IEnumerable<T>>` out of any `ICodec<T>` with any of the relevant methods (`ICodec.ForArray`,`ICodec.ForList`, etc.)
- Create codecs by safely mapping between mutually convertible types such as a `Vector3` and a `float[]` with automatic runtime validation using `SafeMap` and its friends (which map 1-1 with DFU's `xmap` and `flatmap`).
- Serialize and deserialize enum values and bitfields either to integers or string arrays.

All conversions in DataFixerSharper return a `DataResult<T>`, which serves as a wrapper for data operations; A `DataResult` can be either `Success` or `Fail`, allowing you to safely catch errors without throwing ugly exceptions,
and codecs do not hold any reference to the format they operate with - you can implement `IDynamicOps<T>` for your format of choice and use all of your existing codecs for it. For a concrete example, take a look at `JsonOps`.

---
## Codec Types
A few implementations of `ICodec` allow you to seamlessly serialize even complex classes.

For example, `RecordCodecBuilder` is likely to be your best friend as it lets you map fields to getters and constructor parameters, and having the Codec resolve them for you.

Syntax:
```csharp
public sealed record Person(string Name, int Age = 0);

public static readonly Codec<Person> PersonCodec = RecordCodecBuilder.Create(instance =>
    instance.WithFields(
        BuiltinCodecs.String.Field(person => person.Name, "name"),
        BuiltinCodecs.Int32.OptionalField(person => person.Age, "age")
    )
    .WithCtor((name, age) => new Person(name, age))
);

// Then, to use it:
Person person = new Person("John Doe", 18);
DataResult<JsonNode> encoded = PersonCodec.EncodeStart(JsonOps.Instance, person); // EncodeStart encodes one single value without appending it to existing serialized data.

// Now "encoded" holds our serialized data - we can write it to a file or decode it back into a Person using the same codec:
var personData = encoded.ResultOrPartial();
DataResult<Person> decoded = PersonCodec.Parse(JsonOps.Instance, personData);

// Again - we have a DataResult<Person>, which may be successful or failed. We can query with IsError and handle accordingly.
```

Another great implementation is `DispatchCodec` which allows you to serialize polymorphically using a type discriminator field and getter.
A great implementation for it can be found in the [unit tests](https://github.com/ChristosMaragkos/DataFixerSharper/blob/main/WhiteTowerGames.DataFixerSharper.Tests/Composition/Polymorphism.cs), but the idea is this:

```csharp
private static readonly Codec<Circle> CircleCodec = RecordCodecBuilder.Create<Circle>(
        instance =>
            instance
                .WithFields(BuiltinCodecs.Float.Field((Circle circle) => circle.Radius, "radius"))
                .WithCtor(radius => new Circle(radius))
    );

    private static readonly Codec<Rectangle> RectCodec = RecordCodecBuilder.Create<Rectangle>(
        instance =>
            instance
                .WithFields(
                    BuiltinCodecs.Float.Field((Rectangle rect) => rect.W, "width"),
                    BuiltinCodecs.Float.Field((Rectangle rect) => rect.H, "height")
                )
                .WithCtor((width, height) => new Rectangle(width, height))
    );

    private static readonly Codec<Shape> ShapeDispatch = Codec.Dispatch<Shape, string>(
        BuiltinCodecs.String,
        shape => shape.ShapeType(),
        discr => CodecByType(discr)
    );

    private static Codec<Shape> CodecByType(string discriminator)
    {
        return discriminator switch
        {
            "circle" => CircleCodec.Upcast<Circle, Shape>(), // We need to wrap the subtype codecs in Codec.Upcast. Because of how generics work in C#,
            "rectangle" => RectCodec.Upcast<Rectangle, Shape>(), // a Codec<Circle> is not inherently a Codec<Shape>, and Upcast handles the mapping to and from the base class for us.
            _ => throw new InvalidOperationException("Invalid type discriminator"),
        };
    }
```

## `DataFix`es
DataFixerSharper has a form of data transformation rules you can use to migrate data between versions seamlessly: you can define a rule (a class that implements `IDataFix`)
and define how it manipulates data, and from which version. For example, say we have this JSON from version 1.0.0:
```json
{
  "Id": "player_1",
  "Speed": 15
}
```
Let's say that, in version 2.0.0, we renamed "Speed" to "Agility".
We can define a datafix class for this, like so:
```csharp
public class RenameSpeedToAgility : IDataFix
{
    public Version Since { get; init; } = new Version(2,0,0);
    public DataResult<Dynamic<TFormat>> Apply<TFormat>(Dynamic<TFormat> input)
    {
        return input.Get("Speed") // get the value for speed
            .Map(speed => input.Remove("Speed").Set("Agility", speed)) // replace "Speed" with "Agility" and the same value
            .GetOrElse(input); // if we replaced the value, return the new data. Otherwise, return the old data.
    }
}
```
To use our rule, we must first register it with the DataFixer class:
```csharp
DataFixer.RegisterDataFix(new RenameSpeedToAgility());
 ``` 

And, all we need to do in order to migrate our piece of data is call `DataFixer.Migrate`, like so:
```csharp
// Assuming we already have this outdated JSON in the format JsonOps expects (JsonByteBuffer):
// {
//  "Speed" : 15,
//  "Name" : "John"
//}

var migratedResult = DataFixer.Migrate(fromVersion: v1, toVersion: v2, oldJson); // returns a DataResult containing our converted JSON, or an error
var migrated = migratedResult.GetOrThrow();
Console.WriteLine(migrated.ToJsonString()); // Will output "{"Name":"John","Agility":15}"
```
