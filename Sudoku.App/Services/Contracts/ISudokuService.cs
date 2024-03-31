using Sudoku.App.Enums;
using Sudoku.App.Helpers;

namespace Sudoku.App.Services.Contracts;

public interface ISudokuService
{
    /// <summary>
    /// Solves sudoku. If board containing errors is given, method takes very long time, and wrong result is returned.
    /// If there is more than 1 solution, method returns the first one it finds.
    /// Original board is not modified.
    /// </summary>
    /// <param name="cells">Board to solve. Method assumes the board is 9x9</param>
    /// <returns>Solved board, or null if it's unsolvable</returns>
    SudokuBoard<SudokuCell>? Solve(SudokuBoard<SudokuCell> cells);
    
    /// <summary>
    /// Combination of CheckBoardValidity and Solve sudoku.
    /// If there is more than 1 solution, method returns the first one it finds.
    /// Original board is not modified.
    /// </summary>
    /// <param name="cells">9x9 board to solve.</param>
    /// <returns>Solved board, or null if it's either unsolvable, or original board contained errors</returns>
    SudokuBoard<SudokuCell>? ValidateAndSolve(SudokuBoard<SudokuCell> cells);

    /// <summary>
    /// Generates completely random sudoku board, with only 1 possible solution.
    /// </summary>
    /// <returns>9x9 sudoku board</returns>
    Task<SudokuBoard<SudokuDigit>> GenerateBoard();
    
    /// <summary>
    /// Checks if boards is valid, and full.
    /// </summary>
    /// <param name="cells">9x9 sudoku boards</param>
    /// <returns>True if board is solved, false otherwise</returns>
    bool IsSolved(SudokuBoard<SudokuCell> cells);

    /// <summary>
    /// Checks if current sudoku board contains any errors.
    /// </summary>
    /// <param name="cells">9x9 sudoku board</param>
    /// <returns>True if board is valid, false otherwise</returns>
    bool CheckBoardValidity(SudokuBoard<SudokuDigit> cells);

    /// <summary>
    /// Check if the board has no solution, 1 solution, or more than 1 solution.
    /// </summary>
    /// <param name="originalCells"></param>
    /// <returns></returns>
    Solutions SolutionCount(SudokuBoard<SudokuDigit> originalCells);

}