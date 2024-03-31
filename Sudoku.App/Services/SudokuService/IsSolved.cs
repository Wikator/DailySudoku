using Sudoku.App.Enums;
using Sudoku.App.Helpers;

namespace Sudoku.App.Services.SudokuService;

public partial class SudokuService
{
    public bool IsSolved(SudokuBoard<SudokuCell> cells, out SudokuBoard<bool> illegalDigits)
    {
        illegalDigits = CheckBoardValidity(cells);

        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                if (illegalDigits[row, col])
                    return false;
            }
        }
        
        var digits = new SudokuBoard<SudokuDigit>((row, col) => cells[row, col].Value);
        return IsFull(digits);
    }
}