namespace WhiteTowerGames.DataFixerSharper.Abstractions;

public interface ICollectionConsumer<TCol, TFormat>
    where TCol : allows ref struct
{
    void Accept(ref TCol collection, TFormat item);
}

public interface IMapConsumer<TKv, TFormat>
    where TKv : allows ref struct
{
    void Accept(ref TKv map, TFormat key, TFormat value);
}
