using Sudoku.App.Enums;
using Sudoku.App.Helpers;

namespace Sudoku.App.Services.SudokuService;

public partial class SudokuService
{
    public Task<SudokuBoard<SudokuDigit>> GenerateBoard()
    {
        var board = new SudokuBoard<SudokuDigit>(SudokuDigit.Empty);

        while (!IsFull(board))
        {
            var random = new Random();
            var coords = new Coords(random.Next(0, BoardSize), random.Next(0, BoardSize));
            
            if (board[coords] != SudokuDigit.Empty)
                continue;
            
            board[coords] = (SudokuDigit)random.Next(1, 10);
            
            var result = ValidateAndSolve(board);

            if (result is null)
            {   
                board[coords] = SudokuDigit.Empty;
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
            var item = board[coords];
            board[coords] = SudokuDigit.Empty;
            
            if (SolutionCount(board) != Solutions.OneSolution)
                board[coords] = item;
        }

        return Task.FromResult(board);
    }
    
    private static bool IsFull(SudokuBoard<SudokuDigit> cells)
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
}