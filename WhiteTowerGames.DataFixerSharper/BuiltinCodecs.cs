using WhiteTowerGames.DataFixerSharper.Codecs;
using WhiteTowerGames.DataFixerSharper.Codecs.PrimitiveCodec;

namespace WhiteTowerGames.DataFixerSharper;

public static class BuiltinCodecs
{
    public static readonly ICodec<sbyte> Int8 = new Int8Codec();
    public static readonly ICodec<byte> UInt8 = new UInt8Codec();

    public static readonly ICodec<short> Int16 = new Int16Codec();
    public static readonly ICodec<ushort> UInt16 = new UInt16Codec();

    public static readonly ICodec<int> Int32 = new Int32Codec();
    public static readonly ICodec<uint> UInt32 = new UInt32Codec();

    public static readonly ICodec<long> Int64 = new Int64Codec();
    public static readonly ICodec<ulong> UInt64 = new UInt64Codec();

    public static readonly ICodec<float> Float = new FloatCodec();

    public static readonly ICodec<double> Double = new DoubleCodec();

    public static readonly ICodec<string> String = new StringCodec();

    public static readonly ICodec<bool> Bool = new BoolCodec();
}
