```

BenchmarkDotNet v0.13.12, Linux Mint 21.3 (Virginia)
Intel Core i7-3770 CPU 3.40GHz (Ivy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.200
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX
  DefaultJob : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX


```
| Method     | Mean        | Error     | StdDev    |
|----------- |------------:|----------:|----------:|
| Benchmark1 |    93.42 μs |  0.298 μs |  0.249 μs |
| Benchmark2 |   151.21 μs |  1.850 μs |  1.730 μs |
| Benchmark3 |   539.77 μs |  7.961 μs |  7.447 μs |
| Benchmark4 | 1,610.28 μs | 18.498 μs | 17.303 μs |
| Benchmark5 |   841.90 μs |  3.099 μs |  2.588 μs |
| Benchmark6 | 2,871.03 μs | 45.020 μs | 42.112 μs |
