using Sudoku.App.Enums;
using Sudoku.App.Extensions;
using Sudoku.App.Helpers;

namespace Sudoku.App.Services.SudokuService;

public partial class SudokuService
{
    private static SudokuDigit[,]? SolveRecursive(SudokuDigit[,] cells, HashSet<SudokuDigit>[,]? possibleDigits = null)
    {
        // Initialize hashset that stores which digits were modified in the current recursive call, so that they can be
        // reverted if backtracking is needed.
        var modifiedCells = new HashSet<Coords>();

        // Each hashset in possibleDigits 2D array represents the possible digits for the corresponding cell.
        // If no existing hashset was given, one is initialized and calculated here.
        // After this point, empty hashset means that the corresponding cell is already filled.
        // It is assumed that given possibleDigits follow this rule.
        if (possibleDigits is null)
        {
            possibleDigits = new HashSet<SudokuDigit>[BoardSize, BoardSize];
            for (var row = 0; row < BoardSize; row++)
            {
                for (var col = 0; col < BoardSize; col++)
                {
                    // If space is not filled, set all 1-9 digits in the hash set, then remove illegal digits,
                    // based on rows, columns, and 3x3 blocks.
                    // If no digits remain after this operation for any of the cells, the board is unsolvable.
                    // If space is already filled, possible digits are set to none, to follow the rule established
                    // above.
                    var cellCoords = new Coords(row, col);
                    if (cells.Get(cellCoords) == SudokuDigit.Empty)
                    {
                        possibleDigits.Set(cellCoords, AllDigits());
                        if (!GetPossibleDigits(cells, cellCoords, possibleDigits.Get(cellCoords)))
                            return NoSolution();
                    }
                    else
                    {
                        possibleDigits.Set(cellCoords, []);
                    }
                }
            }
        }

        // The board is now filled based on two factors.
        // 1. Cells that have only one possible digit are filled.
        // 2. Cells look if they have an unique possible digit for their row, column, or 3x3 block, and fill it.
        // This process is repeated until no changes have been made in the last iteration.
        // After this point, each cell was filled when possible, and now the algorithm will have to "guess" and
        // backtrack.
        var repeat = true;
        while (repeat)
        {
            repeat = false;
            for (var row = 0; row < BoardSize; row++)
            {
                for (var col = 0; col < BoardSize; col++)
                {
                    var coords = new Coords(row, col);
                    switch (possibleDigits.Get(coords).Count)
                    {
                        // If cell has no possible digits, it means that it is already filled.
                        case <= 0:
                            continue;
                        case 1:
                        {
                            // Cell is filled with the only possible digit, and possibleDigits array is updated to
                            // reflect this change.
                            // This method returns false, if it finds that the board is unsolvable
                            modifiedCells.Add(coords);
                            if (!AutoFill(cells, coords, possibleDigits.Get(coords).First(), possibleDigits))
                                return NoSolution();

                            repeat = true;
                            break;
                        }
                        default:
                        {
                            // Unique possible digit for the cell's row, column, or 3x3 block is looked for.
                            // For example, if there are 3 empty cells in a row, one with 1, 3, and 6 as legal digits,
                            // and remaining two have only 1 and 3, the first cell should be filled with 6.
                            // This method returns false, if it finds that the board is unsolvable.
                            // Otherwise, it returns true, and digit is returned through the out parameter
                            if (!GetUniquePossibleDigit(coords, possibleDigits, out var digit))
                                return NoSolution();

                            // The returned digit can be null, if no unique digit was found.
                            if (digit is null)
                                break;

                            // Cell is filled in the same manner as in the first case, and possibleDigits array is
                            // updated to reflect this change.
                            modifiedCells.Add(coords);
                            if (!AutoFill(cells, coords, digit.Value, possibleDigits))
                                return NoSolution();

                            repeat = true;
                            break;
                        }
                    }
                }
            }
        }

        // Find the coordinates of the cell with the lowest number of possible digits,
        // to minimize the number of recursive calls.
        // At the same time, it is checked if the board is solved.
        var lowestPossibleDigits = BoardSize;
        Coords? lowestPossibleCoords = null;
        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                var coords = new Coords(row, col);
                var count = possibleDigits.Get(coords).Count;
                // At this point, only filled spaces should have 0 possible digits.
                if (count == 0 || count >= lowestPossibleDigits)
                    continue;
                
                lowestPossibleDigits = count;
                lowestPossibleCoords = coords;
            }
        }
        
        // If all cells have 0 possible digits, the board is solved.
        if (lowestPossibleCoords is null)
            return cells;
        
        // At this point, algorithm needs to "guess" the digits and backtrack, if the guessed digit was incorrect.
        // Each possible digit is attempted to be filled in the previously found cell.
        foreach (var digit in possibleDigits[lowestPossibleCoords.Row, lowestPossibleCoords.Column])
        {
            // To avoid the first step of the algorithm, the possibleDigits array is deep copied, so that it can be
            // used in the next iteration.
            var copiedPossibleDigits = new HashSet<SudokuDigit>[BoardSize, BoardSize];
            
            for (var row = 0; row < BoardSize; row++)
            {
                for (var col = 0; col < BoardSize; col++)
                {
                    var coords = new Coords(row, col);
                    copiedPossibleDigits.Set(coords, new HashSet<SudokuDigit>(possibleDigits.Get(coords)));
                }
            }
            
            // Guess is made, and the digit is filled.
            // Copied possibleDigits array is given here instead of the original one, because the original array
            // is no longer needed, and the copy, which will be used by the next recursive call, needs to be updated
            // to reflect the guessed fill.
            if (!AutoFill(cells, lowestPossibleCoords, digit, copiedPossibleDigits))
                continue;
            
            // Recursive call is made, with possibleDigitsCopy provided to avoid first step of the algorithm.
            var result =  SolveRecursive(cells, copiedPossibleDigits);
            
            // If null is returned, the attempted digit was not correct, and the next digit is attempted
            // Otherwise, the attempted digit was the correct one, and the solved board is returned
            if (result is not null)
                return result;
        }
        
        // If no possible digit solves the board, the board is unsolvable.
        // Since the backtracking operation did not update modifiedCells for performance reasons, the cell
        // is manually reverted to its original, empty state.
        cells.Set(lowestPossibleCoords, SudokuDigit.Empty);
        return NoSolution();
        
        // This local function is called everytime the board is unsolvable.
        // It returns null, and reverts the modified cells to their original, empty state.
        SudokuDigit[,]? NoSolution()
        {
            foreach (var coords in modifiedCells)
                cells.Set(coords, SudokuDigit.Empty);

            return null;
        }
    }
}