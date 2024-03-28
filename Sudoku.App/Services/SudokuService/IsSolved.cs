using Sudoku.App.Enums;
using Sudoku.App.Helpers;

namespace Sudoku.App.Services.SudokuService;

public partial class SudokuService
{
    public bool IsSolved(SudokuBoard<SudokuCell> cells)
    {
        var digits = new SudokuBoard<SudokuDigit>((row, col) => cells[row, col].Value);

        return IsFull(digits) && CheckBoardValidity(digits);
    }
}