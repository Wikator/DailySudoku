// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Sudoku.Benchmark;

BenchmarkRunner.Run<SudokuServiceBenchmark>();