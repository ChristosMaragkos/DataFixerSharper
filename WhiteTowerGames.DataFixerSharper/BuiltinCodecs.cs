using WhiteTowerGames.DataFixerSharper.Codecs;
using WhiteTowerGames.DataFixerSharper.Codecs.PrimitiveCodec;

namespace WhiteTowerGames.DataFixerSharper;

public static class BuiltinCodecs
{
    public static readonly ICodec<int> Int32 = new Int32Codec();

    public static readonly ICodec<long> Int64 = new Int64Codec();

    public static readonly ICodec<float> Float = new FloatCodec();

    public static readonly ICodec<double> Double = new DoubleCodec();

    public static readonly ICodec<string> String = new StringCodec();

    public static readonly ICodec<bool> Bool = new BoolCodec();
}
