using Sudoku.App.Enums;
using Sudoku.App.Extensions;
using Sudoku.App.Helpers;

namespace Sudoku.App.Services.SudokuService;

public partial class SudokuService
{
    // This algorithm works exactly the same as Solve, except it doesn't fill the board, and it returns
    // the number of solutions
    // Original board is not modified.
    private static Solutions SolutionCountRecursive(SudokuDigit[,] cells, HashSet<SudokuDigit>[,]? possibleDigits = null)
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
        
        if (!lowestPossibleCoords.HasValue)
            return OneSolution();

        var alreadyFound = false;
        foreach (var digit in possibleDigits[lowestPossibleCoords.Value.Row, lowestPossibleCoords.Value.Column])
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
            
            if (!AutoFill(cells, lowestPossibleCoords.Value, digit, copiedPossibleDigits))
                continue;
            
            var result =  SolutionCountRecursive(cells, copiedPossibleDigits);
            
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
        
        cells.Set(lowestPossibleCoords.Value, SudokuDigit.Empty);
        return alreadyFound ? OneSolution() : NoSolution();

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
}