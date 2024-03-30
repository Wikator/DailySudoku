```

BenchmarkDotNet v0.13.12, macOS Ventura 13.6.3 (22G436) [Darwin 22.6.0]
Intel Core i9-9880H CPU 2.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.203
  [Host]     : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2


```
| Method     | Mean        | Error     | StdDev    |
|----------- |------------:|----------:|----------:|
| Benchmark1 |    79.05 μs |  1.571 μs |  4.352 μs |
| Benchmark2 |   156.22 μs |  4.601 μs | 13.495 μs |
| Benchmark3 |   515.28 μs |  7.974 μs |  7.069 μs |
| Benchmark4 | 1,421.01 μs | 19.068 μs | 16.903 μs |
| Benchmark5 |   748.36 μs | 18.934 μs | 55.531 μs |
| Benchmark6 | 2,280.31 μs | 14.325 μs | 11.962 μs |
