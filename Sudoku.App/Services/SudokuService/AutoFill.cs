using Sudoku.App.Enums;
using Sudoku.App.Extensions;
using Sudoku.App.Helpers;

namespace Sudoku.App.Services.SudokuService;

public partial class SudokuService
{
    /// <summary>
    /// Fills a cell with a digit and removes the digit from the possible digits of the cells
    /// in the same row, column, and block. It does not add the cell to modifiedCells hashset.
    /// </summary>
    /// <param name="cells">9x9 sudoku board</param>
    /// <param name="coords">Coordinates of a cell that will be filled</param>
    /// <param name="digit">Digit to fill</param>
    /// <param name="possibleDigits">Algorithm's 2D array that stores which
    /// digits are legal for corresponding cells</param>
    /// <returns>False if board is found to be unsolvable</returns>
    private static bool AutoFill(SudokuBoard<SudokuDigit> cells, Coords coords, SudokuDigit digit,
        SudokuBoard<HashSet<SudokuDigit>> possibleDigits)
    {
        // Cell is filled with the digit, and its possible digits are set to none.
        cells[coords] = digit;
        possibleDigits[coords].Clear();
        
        // The filled digit is removed from the possible digits of the cells in the same row, column, and block.
        // If that drops cell's possible digits to 0, the board is unsolvable and false is returned.
        for (var offset = 0; offset < BoardSize; offset++)
        {
            if (possibleDigits[coords.Row, offset].Remove(digit)
                && possibleDigits[coords.Row, offset].Count == 0)
                return false;

            if (possibleDigits[offset, coords.Column].Remove(digit)
                && possibleDigits[offset, coords.Column].Count == 0)
                return false;

            var blockCoords = Coords.BlockCoords(coords, offset);

            if (possibleDigits[blockCoords].Remove(digit)
                && possibleDigits[blockCoords].Count == 0)
                return false;
            
        }
        
        // If no problems were found, true is returned.
        return true;
    }
}