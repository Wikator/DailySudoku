using Sudoku.App.Enums;
using Sudoku.App.Helpers;

namespace Sudoku.App.Services.SudokuService;

public partial class SudokuService
{
    // Board is checked for validity, meaning that no digit is duplicated in any row, column, or 3x3 block
    public bool CheckBoardValidity(SudokuDigit[,] cells)
    {
        for (var row = 0; row < BoardSize; row++)
        {
            var rowDigits = AllDigits();
            var colDigits = AllDigits();
            var blockDigits = AllDigits();
            for (var col = 0; col < BoardSize; col++)
            {
                if (cells[row, col] != SudokuDigit.Empty && !rowDigits.Remove(cells[row, col]))
                    return false;

                if (cells[col, row] != SudokuDigit.Empty && !colDigits.Remove(cells[col, row]))
                    return false;
                
                var blockCoords = Coords.BlockCoords(row, col);
                
                if (cells[blockCoords.Row, blockCoords.Column] != SudokuDigit.Empty &&
                    !blockDigits.Remove(cells[blockCoords.Row, blockCoords.Column]))
                    return false;
            }
        }

        return true;
    }
}