using Sudoku.App.Enums;
using Sudoku.App.Helpers;
using Sudoku.App.Services.Contracts;

namespace Sudoku.App.Services.SudokuService;

public partial class SudokuService : ISudokuService
{
    private const int BoardSize = 9;

    private static SudokuBoard<SudokuDigit>? Solve(SudokuBoard<SudokuDigit> originalCells)
    {
        var cells = DeepCopy(originalCells);
        return SolveRecursive(cells);
    }

    private SudokuBoard<SudokuDigit>? ValidateAndSolve(SudokuBoard<SudokuDigit> cells)
    {
        return !CheckBoardValidity(cells) ? null : Solve(cells);
    }

    public SudokuBoard<SudokuCell>? Solve(SudokuBoard<SudokuCell> cells)
    {
        var sudokuDigits = new SudokuBoard<SudokuDigit>((row, col) => cells[row, col].Value);

        var result = Solve(sudokuDigits);
        if (result is null)
            return null;

        var solvedCells = new SudokuBoard<SudokuCell>((row, col) =>
            new SudokuCell
            {
                Value = result[row, col],
                IsFixed = cells[row, col].IsFixed
            });

        return solvedCells;
    }

    public SudokuBoard<SudokuCell>? ValidateAndSolve(SudokuBoard<SudokuCell> cells)
    {
        var sudokuDigits = new SudokuBoard<SudokuDigit>((row, col) => cells[row, col].Value);

        if (!CheckBoardValidity(sudokuDigits))
            return null;

        var result = Solve(sudokuDigits);
        if (result is null)
            return null;

        var solvedCells = new SudokuBoard<SudokuCell>((row, col) =>
            new SudokuCell
            {
                Value = result[row, col],
                IsFixed = cells[row, col].IsFixed
            });

        return solvedCells;
    }
    
    public Solutions SolutionCount(SudokuBoard<SudokuDigit> originalCells)
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
    
    private static SudokuBoard<SudokuDigit> DeepCopy(SudokuBoard<SudokuDigit> original)
    {
        var copy = new SudokuBoard<SudokuDigit>();

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