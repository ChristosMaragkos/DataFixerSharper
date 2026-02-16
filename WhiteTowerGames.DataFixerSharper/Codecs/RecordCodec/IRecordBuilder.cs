using WhiteTowerGames.DataFixerSharper.Abstractions;

namespace WhiteTowerGames.DataFixerSharper.Codecs.RecordCodec;

public interface IRecordBuilder<TFormat>
{
    IRecordBuilder<TFormat> Add(TFormat key, TFormat value);
    IRecordBuilder<TFormat> Add(TFormat key, DataResult<TFormat> valueResult);
    DataResult<TFormat> Build(TFormat prefix);
}
