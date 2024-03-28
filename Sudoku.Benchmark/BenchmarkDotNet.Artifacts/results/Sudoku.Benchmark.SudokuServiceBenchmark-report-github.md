```

BenchmarkDotNet v0.13.12, Linux Mint 21.3 (Virginia)
Intel Core i7-3770 CPU 3.40GHz (Ivy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.200
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX
  DefaultJob : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX


```
| Method     | Mean       | Error    | StdDev   |
|----------- |-----------:|---------:|---------:|
| Benchmark1 |   101.6 μs |  1.93 μs |  1.81 μs |
| Benchmark2 |   162.5 μs |  1.08 μs |  0.90 μs |
| Benchmark3 |   569.7 μs |  3.48 μs |  3.09 μs |
| Benchmark4 | 1,704.0 μs | 11.80 μs |  9.85 μs |
| Benchmark5 |   889.4 μs |  4.44 μs |  3.70 μs |
| Benchmark6 | 3,059.7 μs | 17.23 μs | 16.12 μs |
