using Sudoku.App.Enums;
using Sudoku.App.Helpers;

namespace Sudoku.App.Services.SudokuService;

public partial class SudokuService
{
    // This algorithm works exactly the same as Solve, except it doesn't fill the board, and it returns
    // the number of solutions
    // Original board is not modified.
    private static Solutions SolutionCountRecursive(SudokuBoard<SudokuDigit> board,
        SudokuBoard<HashSet<SudokuDigit>>? possibleDigits = null)
    {
        var modifiedCells = new HashSet<Coords>();
        
        if (possibleDigits is null)
        {
            possibleDigits = new SudokuBoard<HashSet<SudokuDigit>>();
            for (var row = 0; row < BoardSize; row++)
            {
                for (var col = 0; col < BoardSize; col++)
                {
                    var cellCoords = new Coords(row, col);
                    if (board[cellCoords]== SudokuDigit.Empty)
                    {
                        possibleDigits[cellCoords] = AllDigits();
                        if (!GetPossibleDigits(board, cellCoords, possibleDigits[cellCoords]))
                            return NoSolution();
                    }
                    else
                    {
                        possibleDigits[cellCoords] = [];
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
                    switch (possibleDigits[coords].Count)
                    {
                        case <= 0:
                            continue;
                        case 1:
                        {
                            modifiedCells.Add(coords);
                            if (!AutoFill(board, coords, possibleDigits[coords].First(), possibleDigits))
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
                            if (!AutoFill(board, coords, digit.Value, possibleDigits))
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
                var count = possibleDigits[coords].Count;
                
                if (count == 0 || count >= lowestPossibleDigits)
                    continue;
                
                lowestPossibleDigits = count;
                lowestPossibleCoords = coords;
            }
        }
        
        if (!lowestPossibleCoords.HasValue)
            return OneSolution();

        var alreadyFound = false;
        foreach (var digit in possibleDigits[lowestPossibleCoords.Value.Row, lowestPossibleCoords.Value.Column])
        {
            var copiedPossibleDigits = new SudokuBoard<HashSet<SudokuDigit>>((row, col) =>
                [..possibleDigits[row, col]]);
            
            if (!AutoFill(board, lowestPossibleCoords.Value, digit, copiedPossibleDigits))
                continue;
            
            var result =  SolutionCountRecursive(board, copiedPossibleDigits);
            
            switch (result)
            {
                case Solutions.NoSolution:
                    continue;
                case Solutions.MultipleSolutions:
                    return MultipleSolutions();
                case Solutions.OneSolution:
                {
                    if (alreadyFound)
                        return MultipleSolutions();

                    alreadyFound = true;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        board[lowestPossibleCoords.Value] = SudokuDigit.Empty;
        return alreadyFound ? OneSolution() : NoSolution();

        Solutions NoSolution()
        {
            foreach (var coords in modifiedCells)
                board[coords] = SudokuDigit.Empty;
                        
            return Solutions.NoSolution; 
        }

        Solutions OneSolution()
        {
            foreach (var coords in modifiedCells)
                board[coords] = SudokuDigit.Empty;
                        
            return Solutions.OneSolution; 
        }

        Solutions MultipleSolutions()
        {
            foreach (var coords in modifiedCells)
                board[coords] = SudokuDigit.Empty;
                        
            return Solutions.MultipleSolutions; 
        }
    }
}