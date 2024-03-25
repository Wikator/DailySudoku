```

BenchmarkDotNet v0.13.12, Linux Mint 21.3 (Virginia)
Intel Core i7-3770 CPU 3.40GHz (Ivy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.200
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX
  DefaultJob : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX


```
| Method     | Mean       | Error    | StdDev    | Median     |
|----------- |-----------:|---------:|----------:|-----------:|
| Benchmark1 |   104.7 μs |  3.34 μs |   9.64 μs |   101.6 μs |
| Benchmark2 |   160.2 μs |  2.65 μs |   2.35 μs |   159.9 μs |
| Benchmark3 |   576.4 μs |  4.98 μs |   4.42 μs |   577.3 μs |
| Benchmark4 | 1,919.9 μs | 59.11 μs | 174.27 μs | 1,860.5 μs |
| Benchmark5 |   894.1 μs | 23.02 μs |  67.51 μs |   871.2 μs |
| Benchmark6 | 3,157.6 μs | 35.24 μs |  31.24 μs | 3,146.5 μs |
