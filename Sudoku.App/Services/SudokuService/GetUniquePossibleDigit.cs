using Sudoku.App.Enums;
using Sudoku.App.Helpers;

namespace Sudoku.App.Services.SudokuService;

public partial class SudokuService
{
    /// <summary>
    /// Cell is checked for unique possible digit in its row, column, and 3x3 block.
    /// </summary>
    /// <param name="coords">Coordinates of the cell</param>
    /// <param name="possibleDigits">Algorithm's 2D array that stores which
    /// digits are legal for corresponding cells</param>
    /// <param name="uniqueDigit">Out parameter that returns found digit, or null</param>
    /// <returns>False if the board is found to be unsolvable, true otherwise</returns>
    private static bool GetUniquePossibleDigit(Coords coords, SudokuBoard<HashSet<SudokuDigit>> possibleDigits,
        out SudokuDigit? uniqueDigit)
    {
        // First, it checks which digits from its possible digits do not appear in any other cell of its row, column,
        // and 3x3 block.
        var digitSets = new HashSet<SudokuDigit>[3];
        digitSets[0] = GetRemainingDigits(coords, possibleDigits, (r, _, o) => new Coords(r, o));
        digitSets[1] = GetRemainingDigits(coords, possibleDigits, (_, c, o) => new Coords(o, c));
        digitSets[2] = GetRemainingDigits(coords, possibleDigits, Coords.BlockCoords);
        
        // An answer is only valid if either all sets have more than one digit, in which case
        // there is no unique digit, or when all sets with exactly one digit have the same digit.
        // Otherwise, if there are multiple sets with 1 digit, and they are not the same, the board is unsolvable,
        // because of the rule that each row, column, and block must contain all 9 different digits.
        foreach (var digits in digitSets)
        {
            if (digits.Count != 1)
                continue;
            
            var candidate = digits.First();
            if (digitSets.All(set => set.Count != 1 || set.First() == candidate))
            {
                uniqueDigit = candidate;
                return true;
            }
            uniqueDigit = null;
            return false;
        }

        uniqueDigit = null;
        return true;
    }

    private static HashSet<SudokuDigit> GetRemainingDigits(Coords coords,
        SudokuBoard<HashSet<SudokuDigit>> possibleDigits, Func<int, int, int, Coords> getCoords)
    {
        // It starts with a copy of its possible digits, and removes digits that appear in other cells.
        var remainingDigits = new HashSet<SudokuDigit>(possibleDigits[coords]);
        for (var offset = 0; offset < BoardSize; offset++)
        {
            // getCords is a function that turns the offsets into the indexes of the needed cells.
            var (r, c) = getCoords(coords.Row, coords.Column, offset);
            if (r == coords.Row && c == coords.Column)
                continue;

            foreach (var digit in possibleDigits[r, c])
            {
                remainingDigits.Remove(digit);
            }
        }
        return remainingDigits;
    }
}