```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD Ryzen 5 5600X 2.95GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method            | Mean       | Error    | StdDev   | Gen0   | Allocated |
|------------------ |-----------:|---------:|---------:|-------:|----------:|
| STJ_Serialize     |   154.1 ns |  0.81 ns |  0.63 ns | 0.0043 |      72 B |
| STJ_Deserialize   |   240.7 ns |  2.25 ns |  2.10 ns | 0.0100 |     168 B |
| Codec_Serialize   | 1,352.2 ns | 15.96 ns | 14.14 ns | 0.2460 |    4120 B |
| Codec_Deserialize | 1,416.1 ns | 19.64 ns | 18.37 ns | 0.1755 |    2960 B |
