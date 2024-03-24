using Sudoku.App.Enums;
using Sudoku.App.Helpers;

namespace Sudoku.App.Services.SudokuService;

public partial class SudokuService
{
    // Cell with given coordinates is checked, removing digits from possibleDigits
    // hashset, leaving only the legal digits for the cell.
    // False is returned if cell has no possible digits, and board is unsolvable.
    private static bool GetPossibleDigits(SudokuDigit[,] cells, Coords coords,
        ICollection<SudokuDigit> possibleDigits)
    {
        for (var offset = 0; offset < BoardSize; offset++)
        {
            if (cells[coords.Row, offset]!= SudokuDigit.Empty)
                possibleDigits.Remove(cells[coords.Row, offset]);

            if (cells[offset, coords.Column]!= SudokuDigit.Empty)
                possibleDigits.Remove(cells[offset, coords.Column]);

            var blockCoords = Coords.BlockCoords(coords, offset);

            if (cells[blockCoords.Row, blockCoords.Column] == SudokuDigit.Empty)
                continue;

            possibleDigits.Remove(cells[blockCoords.Row, blockCoords.Column]);
        }
        
        // If cell has no possible digits and is not yet filled, board is unsolvable.
        return possibleDigits.Count > 0;
    }
}