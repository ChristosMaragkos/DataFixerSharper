```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD Ryzen 5 5600X 1.74GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method                     | Mean     | Error   | StdDev  | Gen0   | Allocated |
|--------------------------- |---------:|--------:|--------:|-------:|----------:|
| STJ_Serialize              | 147.5 ns | 1.04 ns | 0.87 ns | 0.0043 |      72 B |
| STJ_Serialize_IntArray     | 115.3 ns | 1.06 ns | 0.99 ns | 0.0024 |      40 B |
| STJ_Deserialize            | 242.1 ns | 1.20 ns | 1.07 ns | 0.0100 |     168 B |
| STJ_Deserialize_IntArray   | 147.6 ns | 1.28 ns | 1.07 ns | 0.0067 |     112 B |
| Codec_Serialize            | 721.6 ns | 5.51 ns | 5.15 ns | 0.0525 |     880 B |
| Codec_Deserialize          | 427.5 ns | 3.22 ns | 2.85 ns | 0.0143 |     240 B |
| Codec_Serialize_IntArray   | 797.9 ns | 4.15 ns | 3.47 ns | 0.0486 |     816 B |
| Codec_Deserialize_IntArray | 185.5 ns | 1.75 ns | 1.46 ns | 0.0553 |     928 B |
