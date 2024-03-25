using BenchmarkDotNet.Attributes;
using Sudoku.App.Enums;
using Sudoku.App.Helpers;
using Sudoku.App.Services.SudokuService;

namespace Sudoku.Benchmark;

public class SudokuServiceBenchmark
{
    private SudokuService _sudokuService = new();
    
    [Benchmark]
    public void Benchmark1()
    {
        var digits = new SudokuCell[9, 9];
        var tmp = new[,]
        {
            { 5, 0, 6, 9, 1, 8, 0, 2, 3 },
            { 7, 1, 3, 6, 0, 2, 0, 4, 9 },
            { 0, 8, 0, 0, 0, 0, 0, 5, 1 },
            { 6, 0, 4, 0, 0, 0, 0, 0, 2 },
            { 0, 2, 7, 8, 6, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 4, 1, 0, 0 },
            { 3, 0, 0, 1, 4, 0, 2, 0, 0 },
            { 0, 0, 0, 5, 0, 6, 3, 0, 8 },
            { 0, 7, 0, 0, 0, 0, 0, 0, 6 }
        };
        
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                digits[i, j] = new SudokuCell
                {
                    Value = (SudokuDigit)tmp[i, j],
                    IsFixed = tmp[i, j] != 0
                };
            }
        }
        
        _sudokuService.Solve(digits);
    }
    
    [Benchmark]
    public void Benchmark2()
    {
        var digits = new SudokuCell[9, 9];
        var tmp = new[,]
        {
            { 0, 0, 0, 0, 0, 5, 2, 0, 0 },
            { 3, 4, 9, 7, 2, 6, 5, 0, 0 },
            { 0, 0, 0, 8, 0, 0, 7, 4, 3 },
            { 4, 0, 0, 0, 0, 0, 6, 5, 0 },
            { 0, 8, 3, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 6, 8, 0, 0, 0, 2 },
            { 0, 3, 2, 0, 7, 9, 0, 0, 0 },
            { 0, 9, 8, 0, 0, 0, 0, 7, 4 },
            { 0, 0, 0, 0, 0, 0, 1, 0, 0 }
        };
        
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                digits[i, j] = new SudokuCell
                {
                    Value = (SudokuDigit)tmp[i, j],
                    IsFixed = tmp[i, j] != 0
                };
            }
        }
        
        _sudokuService.Solve(digits);
    }
    
    [Benchmark]
    public void Benchmark3()
    {
        var digits = new SudokuCell[9, 9];
        var tmp = new[,]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 3 },
            { 9, 0, 0, 3, 0, 2, 0, 0, 0 },
            { 6, 8, 0, 0, 0, 0, 0, 5, 0 },
            { 1, 0, 0, 0, 0, 5, 0, 0, 9 },
            { 5, 0, 0, 7, 0, 0, 0, 6, 2 },
            { 0, 0, 4, 0, 1, 0, 5, 3, 8 },
            { 3, 4, 0, 8, 0, 0, 7, 0, 0 },
            { 0, 0, 1, 9, 0, 0, 0, 0, 0 },
            { 0, 5, 0, 0, 7, 3, 0, 0, 0 }
        };
        
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                digits[i, j] = new SudokuCell
                {
                    Value = (SudokuDigit)tmp[i, j],
                    IsFixed = tmp[i, j] != 0
                };
            }
        }
        
        _sudokuService.Solve(digits);
    }
    
    [Benchmark]
    public void Benchmark4()
    {
        var digits = new SudokuCell[9, 9];
        var tmp = new[,]
        {
            { 0, 0, 6, 0, 0, 0, 9, 0, 0 },
            { 3, 8, 2, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 4, 0, 0, 0, 6, 0, 5 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 8 },
            { 0, 0, 0, 2, 0, 8, 0, 6, 0 },
            { 4, 0, 0, 3, 0, 0, 0, 0, 2 },
            { 0, 3, 0, 0, 4, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 5, 0, 1, 7, 0 },
            { 7, 0, 0, 0, 0, 6, 0, 0, 0 }
        };
        
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                digits[i, j] = new SudokuCell
                {
                    Value = (SudokuDigit)tmp[i, j],
                    IsFixed = tmp[i, j] != 0
                };
            }
        }
        
        _sudokuService.Solve(digits);
    }
    
    [Benchmark]
    public void Benchmark5()
    {
        var digits = new SudokuCell[9, 9];
        var tmp = new[,]
        {
            { 0, 1, 0, 0, 0, 6, 4, 0, 0 },
            { 0, 9, 0, 4, 5, 0, 0, 2, 0 },
            { 5, 0, 0, 0, 0, 7, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 5, 0, 0, 0 },
            { 9, 0, 0, 1, 8, 0, 7, 0, 0 },
            { 0, 2, 0, 0, 0, 0, 0, 0, 3 },
            { 8, 0, 0, 9, 4, 0, 1, 0, 0 },
            { 0, 0, 0, 6, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 7, 0 }
        };
        
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                digits[i, j] = new SudokuCell
                {
                    Value = (SudokuDigit)tmp[i, j],
                    IsFixed = tmp[i, j] != 0
                };
            }
        }
        
        _sudokuService.Solve(digits);
    }

    [Benchmark]
    public void Benchmark6()
    {
        var digits = new SudokuCell[9, 9];
        var tmp = new[,]
        {
            { 0, 0, 5, 3, 0, 0, 0, 0, 0 },
            { 8, 0, 0, 0, 0, 0, 0, 2, 0 },
            { 0, 7, 0, 0, 1, 0, 5, 0, 0 },
            { 4, 0, 0, 0, 0, 5, 3, 0, 0 },
            { 0, 1, 0, 0, 7, 0, 0, 0, 6 },
            { 0, 0, 3, 2, 0, 0, 0, 8, 0 },
            { 0, 6, 0, 5, 0, 0, 0, 0, 9 },
            { 0, 0, 4, 0, 0, 0, 0, 3, 0 },
            { 0, 0, 0, 0, 0, 9, 7, 0, 0 }
        };
        
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                digits[i, j] = new SudokuCell
                {
                    Value = (SudokuDigit)tmp[i, j],
                    IsFixed = tmp[i, j] != 0
                };
            }
        }
        
        _sudokuService.Solve(digits);
    }
}