```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3296/23H2/2023Update/SunValley3)
Intel Core i7-9700K CPU 3.60GHz (Coffee Lake), 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


```
| Method     | Mean        | Error     | StdDev    |
|----------- |------------:|----------:|----------:|
| Benchmark1 |    69.40 μs |  1.305 μs |  1.220 μs |
| Benchmark2 |   121.72 μs |  0.946 μs |  0.885 μs |
| Benchmark3 |   417.08 μs |  2.746 μs |  2.569 μs |
| Benchmark4 | 1,147.99 μs | 16.118 μs | 15.076 μs |
| Benchmark5 |   618.08 μs |  7.032 μs |  6.578 μs |
| Benchmark6 | 2,057.21 μs | 16.829 μs | 15.741 μs |
