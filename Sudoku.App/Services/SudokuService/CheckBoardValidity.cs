using Sudoku.App.Enums;
using Sudoku.App.Helpers;

namespace Sudoku.App.Services.SudokuService;

public partial class SudokuService
{
    // Board is checked for validity, meaning that no digit is duplicated in any row, column, or 3x3 block
    public SudokuBoard<bool> CheckBoardValidity(SudokuBoard<SudokuCell> cells)
    {
        var illegalDigits = new SudokuBoard<bool>(false);
        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                var coords = new Coords(row, col);
                
                if (cells[coords].IsFixed)
                    continue;

                var currentValue = cells[coords].Value;
                
                if (currentValue == SudokuDigit.Empty)
                    continue;
                
                for (var offset = 0; offset < BoardSize; offset++)
                {
                    if (coords.Column != offset && currentValue == cells[coords.Row, offset].Value)
                    {
                        illegalDigits[coords] = true;
                        continue;
                    }

                    if (coords.Row != offset && currentValue == cells[offset, coords.Column].Value)
                    {
                        illegalDigits[coords] = true;
                        continue;
                    }
                
                    var blockCoords = Coords.BlockCoords(coords.Row, coords.Column, offset);

                    if (blockCoords != coords && currentValue == cells[blockCoords.Row, blockCoords.Column].Value)
                        illegalDigits[coords] = true;
                }
            }
        }

        return illegalDigits;
    }
    
    private static bool CheckBoardValidity(SudokuBoard<SudokuDigit> cells)
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