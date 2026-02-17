```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD Ryzen 5 5600X 1.74GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method                     | Mean     | Error   | StdDev   | Median   | Gen0   | Allocated |
|--------------------------- |---------:|--------:|---------:|---------:|-------:|----------:|
| STJ_Serialize              | 141.4 ns | 1.46 ns |  1.36 ns | 141.4 ns | 0.0043 |      72 B |
| STJ_Serialize_IntArray     | 120.4 ns | 0.85 ns |  0.80 ns | 120.0 ns | 0.0024 |      40 B |
| STJ_Deserialize            | 250.2 ns | 1.51 ns |  1.26 ns | 249.9 ns | 0.0100 |     168 B |
| STJ_Deserialize_IntArray   | 149.0 ns | 0.89 ns |  0.84 ns | 148.7 ns | 0.0067 |     112 B |
| Codec_Serialize            | 506.1 ns | 4.82 ns |  4.51 ns | 504.1 ns | 0.0257 |     440 B |
| Codec_Deserialize          | 315.6 ns | 6.27 ns | 11.31 ns | 310.8 ns | 0.0567 |     952 B |
| Codec_Serialize_IntArray   | 398.4 ns | 2.81 ns |  2.63 ns | 398.0 ns | 0.0486 |     816 B |
| Codec_Deserialize_IntArray | 500.5 ns | 2.02 ns |  1.69 ns | 500.3 ns | 0.0114 |     192 B |
