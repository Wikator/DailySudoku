using Sudoku.App.Enums;
using Sudoku.App.Helpers;
using Sudoku.App.Services.Contracts;

namespace Sudoku.App.Services.SudokuService;

public partial class SudokuService : ISudokuService
{
    private const int BoardSize = 9;

    private static SudokuDigit[,]? Solve(SudokuDigit[,] originalCells)
    {
        var cells = DeepCopy(originalCells);
        return SolveRecursive(cells);
    }

    private SudokuDigit[,]? ValidateAndSolve(SudokuDigit[,] cells)
    {
        return !CheckBoardValidity(cells) ? null : Solve(cells);
    }

    public SudokuCell[,]? Solve(SudokuCell[,] cells)
    {
        var sudokuDigits = new SudokuDigit[BoardSize, BoardSize];
        for (var i = 0; i < BoardSize; i++)
        {
            for (var j = 0; j < BoardSize; j++)
            {
                sudokuDigits[i, j] = cells[i, j].Value;
            }
        }

        var result = Solve(sudokuDigits);
        if (result is null)
            return null;

        var solvedCells = new SudokuCell[BoardSize, BoardSize];
        for (var i = 0; i < BoardSize; i++)
        {
            for (var j = 0; j < BoardSize; j++)
            {
                solvedCells[i, j] = new SudokuCell
                {
                    Value = result[i, j],
                    IsFixed = cells[i, j].IsFixed
                };
            }
        }

        return solvedCells;
    }

    public SudokuCell[,]? ValidateAndSolve(SudokuCell[,] cells)
    {
        var sudokuDigits = new SudokuDigit[BoardSize, BoardSize];
        for (var i = 0; i < BoardSize; i++)
        {
            for (var j = 0; j < BoardSize; j++)
            {
                sudokuDigits[i, j] = cells[i, j].Value;
            }
        }

        if (!CheckBoardValidity(sudokuDigits))
            return null;

        var result = Solve(sudokuDigits);
        if (result is null)
            return null;

        var solvedCells = new SudokuCell[BoardSize, BoardSize];
        for (var i = 0; i < BoardSize; i++)
        {
            for (var j = 0; j < BoardSize; j++)
            {
                solvedCells[i, j] = new SudokuCell
                {
                    Value = result[i, j],
                    IsFixed = cells[i, j].IsFixed
                };
            }
        }

        return solvedCells;
    }
    
    public Solutions SolutionCount(SudokuDigit[,] originalCells)
    {
        if (!CheckBoardValidity(originalCells))
            return Solutions.NoSolution;
        
        var cells = DeepCopy(originalCells);
        return SolutionCountRecursive(cells);
    }
    
    private static void Shuffle<T>(Random rng, IList<T> array)
    {
        var n = array.Count;
        while (n > 1) 
        {
            var k = rng.Next(n--);
            (array[n], array[k]) = (array[k], array[n]);
        }
    }
    
    private static SudokuDigit[,] DeepCopy(SudokuDigit[,] original)
    {
        var copy = new SudokuDigit[BoardSize, BoardSize];

        for (var i = 0; i < BoardSize; i++)
        {
            for (var j = 0; j < BoardSize; j++)
            {
                copy[i, j] = original[i, j];
            }
        }

        return copy;
    }

    private static HashSet<SudokuDigit> AllDigits() =>
    [
        SudokuDigit.One, SudokuDigit.Two, SudokuDigit.Three, SudokuDigit.Four, SudokuDigit.Five,
        SudokuDigit.Six, SudokuDigit.Seven, SudokuDigit.Eight, SudokuDigit.Nine
    ];
}