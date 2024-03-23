using Sudoku.App.Enums;
using Sudoku.App.Extensions;
using Sudoku.App.Helpers;
using Sudoku.App.Services.Contracts;

namespace Sudoku.App.Services;

public class SudokuService : ISudokuService
{
    private const int BoardSize = 9;

    public SudokuDigit[,]? Solve(SudokuDigit[,] originalCells)
    {
        var cells = DeepCopy(originalCells);
        return SolveRecursive(cells);
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

    public SudokuDigit[,]? ValidateAndSolve(SudokuDigit[,] cells)
    {
        if (!CheckBoardValidity(cells))
            return null;

        var copy = DeepCopy(cells);
        return SolveRecursive(copy);
    }

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

    public SudokuDigit[,] GenerateBoard()
    {
        var board = new SudokuDigit[BoardSize, BoardSize];
        board.Fill(SudokuDigit.Empty);

        while (!IsFull(board))
        {
            var random = new Random();
            var coords = new Coords(random.Next(0, BoardSize), random.Next(0, BoardSize));
            
            if (board.Get(coords) != SudokuDigit.Empty)
                continue;
            
            board.Set(coords, (SudokuDigit)random.Next(1, 10));
            
            var result = ValidateAndSolve(board);

            if (result is null)
            {   
                board.Set(coords, SudokuDigit.Empty);
            }
        }
        
        var positions = new Coords[BoardSize * BoardSize];
        for (var i = 0; i < BoardSize * BoardSize; i++)
        {
            positions[i] = new Coords(i / BoardSize, i % BoardSize);
        }
        
        Shuffle(new Random(), positions);

        foreach (var coords in positions)
        {
            board.Set(coords, SudokuDigit.Empty);
            
            if (SolutionCount(board) != Solutions.OneSolution)
                board.Set(coords, board.Get(coords));
        }

        return board;
    }
    
    public Solutions SolutionCount(SudokuDigit[,] originalCells)
    {
        if (!CheckBoardValidity(originalCells))
            return Solutions.NoSolution;
        
        var cells = DeepCopy(originalCells);
        return SolutionCountRec(cells);
    }
    
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

    // This algorithm works exactly the same as Solve, except it doesn't fill the board, and it returns
    // the number of solutions
    // Original board is not modified.
    private static Solutions SolutionCountRec(SudokuDigit[,] cells, HashSet<SudokuDigit>[,]? possibleDigits = null)
    {
        var modifiedCells = new HashSet<Coords>();
        if (possibleDigits is null)
        {
            possibleDigits = new HashSet<SudokuDigit>[BoardSize, BoardSize];
            for (var row = 0; row < BoardSize; row++)
            {
                for (var col = 0; col < BoardSize; col++)
                {
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
                        case <= 0:
                            continue;
                        case 1:
                        {
                            modifiedCells.Add(coords);
                            if (!AutoFill(cells, coords, possibleDigits.Get(coords).First(), possibleDigits))
                                return NoSolution();

                            repeat = true;
                            break;
                        }
                        default:
                        {
                            if (!GetUniquePossibleDigit(coords, possibleDigits, out var digit))
                                return NoSolution();

                            if (digit is null)
                                break;

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

        var lowestPossibleDigits = BoardSize;
        Coords? lowestPossibleCoords = null;
        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                var coords = new Coords(row, col);
                var count = possibleDigits.Get(coords).Count;
                
                if (count == 0 || count >= lowestPossibleDigits)
                    continue;
                
                lowestPossibleDigits = count;
                lowestPossibleCoords = coords;
            }
        }
        
        // If all cells have 0 possible digits, the board is solved.
        if (lowestPossibleCoords is null)
            return OneSolution();

        var alreadyFound = false;
        foreach (var digit in possibleDigits[lowestPossibleCoords.Row, lowestPossibleCoords.Column])
        {
            var copiedPossibleDigits = new HashSet<SudokuDigit>[BoardSize, BoardSize];
            
            for (var row = 0; row < BoardSize; row++)
            {
                for (var col = 0; col < BoardSize; col++)
                {
                    var coords = new Coords(row, col);
                    copiedPossibleDigits.Set(coords, new HashSet<SudokuDigit>(possibleDigits.Get(coords)));
                }
            }
            
            if (!AutoFill(cells, lowestPossibleCoords, digit, copiedPossibleDigits))
                continue;
            
            var result =  SolutionCountRec(cells, copiedPossibleDigits);
            
            if (result == Solutions.NoSolution)
                continue;

            if (result == Solutions.MultipleSolutions)
                return MultipleSolutions();

            if (alreadyFound)
                return MultipleSolutions();
            
            
            alreadyFound = true;
        }

        return !alreadyFound ? Solutions.OneSolution : NoSolution();

        Solutions NoSolution()
        {
            foreach (var coords in modifiedCells)
                cells.Set(coords, SudokuDigit.Empty);
                        
            return Solutions.NoSolution; 
        }

        Solutions OneSolution()
        {
            foreach (var coords in modifiedCells)
                cells.Set(coords, SudokuDigit.Empty);
                        
            return Solutions.OneSolution; 
        }

        Solutions MultipleSolutions()
        {
            foreach (var coords in modifiedCells)
                cells.Set(coords, SudokuDigit.Empty);
                        
            return Solutions.MultipleSolutions; 
        }
    }

    /// <summary>
    /// Cell is checked for unique possible digit in its row, column, and 3x3 block.
    /// </summary>
    /// <param name="coords">Coordinates of the cell</param>
    /// <param name="possibleDigits">Algorithm's 2D array that stores which
    /// digits are legal for corresponding cells</param>
    /// <param name="uniqueDigit">Out parameter that returns found digit, or null</param>
    /// <returns>False if the board is found to be unsolvable, true otherwise</returns>
    private static bool GetUniquePossibleDigit(Coords coords, HashSet<SudokuDigit>[,] possibleDigits,
        out SudokuDigit? uniqueDigit)
    {
        // First, it checks which digits from its possible digits do not appear in any other cell of its row, column,
        // and 3x3 block.
        var rowRemainingDigits = GetRemainingDigits(coords, possibleDigits, (r, _, o) => new Coords(r, o));
        var colRemainingDigits = GetRemainingDigits(coords, possibleDigits, (_, c, o) => new Coords(o, c));
        var blockRemainingDigits = GetRemainingDigits(coords, possibleDigits, Coords.BlockCoords);
        
        // Now that it has the remaining digits, some safety checks must be made.
        return GetUniqueDigit(out uniqueDigit, rowRemainingDigits, colRemainingDigits, blockRemainingDigits);
    }

    private static HashSet<SudokuDigit> GetRemainingDigits(Coords coords, HashSet<SudokuDigit>[,] possibleDigits,
        Func<int, int, int, Coords> getCoords)
    {
        // It starts with a copy of its possible digits, and removes digits that appear in other cells.
        var remainingDigits = new HashSet<SudokuDigit>(possibleDigits.Get(coords));
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

    private static bool GetUniqueDigit(out SudokuDigit? digit, params HashSet<SudokuDigit>[] digitSets)
    {
        // An answer is only valid if either all sets have more than one digit, in which case
        // there is no unique digit, or when all sets with exactly one digit have the same digit.
        // Otherwise, if there are multiple sets with 1 digit, and they are not the same, the board is unsolvable,
        // because of the rule that each row, column, and block must contain all 9 different digits.
        foreach (var digits in digitSets)
        {
            if (digits.Count != 1)
                continue;
            
            var uniqueDigit = digits.First();
            if (digitSets.All(set => set.Count != 1 || set.First() == uniqueDigit))
            {
                digit = uniqueDigit;
                return true;
            }
            digit = null;
            return false;
        }

        digit = null;
        return true;
    }
    
    // Cell with given coordinates is checked, removing digits from possibleDigits
    // hashset, leaving only the legal digits for the cell.
    // False is returned if cell has no possible digits, and board is unsolvable.
    private static bool GetPossibleDigits(SudokuDigit[,] cells, Coords coords,
        HashSet<SudokuDigit> possibleDigits)
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
    private static bool AutoFill(SudokuDigit[,] cells, Coords coords, SudokuDigit digit,
        HashSet<SudokuDigit>[,] possibleDigits)
    {
        // Cell is filled with the digit, and its possible digits are set to none.
        cells.Set(coords, digit);
        possibleDigits.Set(coords, []);
        
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

            if (possibleDigits.Get(blockCoords).Remove(digit)
                && possibleDigits.Get(blockCoords).Count == 0)
                return false;
            
        }
        
        // If no problems were found, true is returned.
        return true;
    }
    
    private static bool IsFull(SudokuDigit[,] cells)
    {
        for (var i = 0; i < BoardSize; i++)
        {
            for (var j = 0; j < BoardSize; j++)
            {
                if (cells[i, j] == SudokuDigit.Empty)
                    return false;
            }
        }

        return true;
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

    private static void PrintBoard(SudokuCell[,] cells)
    {
        for (var i = 0; i < 9; i++)
        {
            if (i % 3 == 0 && i != 0)
            {
                Console.WriteLine("------+-------+------");
            }

            for (var j = 0; j < 9; j++)
            {
                if (j % 3 == 0 && j != 0)
                {
                    Console.Write("| ");
                }

                var value = cells[i, j].Value;
                Console.Write(value == SudokuDigit.Empty ? "  " : (int)value + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
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
