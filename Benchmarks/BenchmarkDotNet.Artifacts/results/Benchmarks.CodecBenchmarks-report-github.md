```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD Ryzen 5 5600X 3.70GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method            | Mean      | Error     | StdDev    | Median   | Gen0   | Allocated |
|------------------ |----------:|----------:|----------:|---------:|-------:|----------:|
| STJ_Serialize     | 160.18 ns |  1.660 ns |  1.630 ns | 159.8 ns | 0.0043 |      72 B |
| STJ_Deserialize   | 270.18 ns |  3.675 ns |  3.438 ns | 269.0 ns | 0.0100 |     168 B |
| Codec_Serialize   | 521.49 ns | 15.332 ns | 45.206 ns | 541.8 ns | 0.0753 |    1264 B |
| Codec_Deserialize |  98.08 ns |  2.959 ns |  8.346 ns | 100.6 ns | 0.0162 |     272 B |
