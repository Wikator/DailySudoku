```

BenchmarkDotNet v0.13.12, Linux Mint 21.3 (Virginia)
Intel Core i7-3770 CPU 3.40GHz (Ivy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.200
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX
  DefaultJob : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX


```
| Method     | Mean       | Error    | StdDev   |
|----------- |-----------:|---------:|---------:|
| Benchmark1 |   107.6 μs |  2.15 μs |  3.87 μs |
| Benchmark2 |   177.1 μs |  3.47 μs |  4.52 μs |
| Benchmark3 |   611.0 μs |  4.65 μs |  4.35 μs |
| Benchmark4 | 1,849.1 μs | 35.88 μs | 39.88 μs |
| Benchmark5 |   995.0 μs | 19.22 μs | 26.31 μs |
| Benchmark6 | 3,442.3 μs | 58.48 μs | 51.84 μs |
