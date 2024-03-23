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
    SudokuDigit[,]? Solve(SudokuDigit[,] cells);
    
    SudokuCell[,]? Solve(SudokuCell[,] cells);
    
    /// <summary>
    /// Combination of CheckBoardValidity and Solve sudoku.
    /// If there is more than 1 solution, method returns the first one it finds.
    /// Original board is not modified.
    /// </summary>
    /// <param name="cells">9x9 board to solve.</param>
    /// <returns>Solved board, or null if it's either unsolvable, or original board contained errors</returns>
    SudokuDigit[,]? ValidateAndSolve(SudokuDigit[,] cells);

    /// <summary>
    /// Generates completely random sudoku board, with only 1 possible solution.
    /// </summary>
    /// <returns>9x9 sudoku board</returns>
    SudokuDigit[,] GenerateBoard();

    /// <summary>
    /// Checks if current sudoku board contains any errors.
    /// </summary>
    /// <param name="cells">9x9 sudoku board</param>
    /// <returns>True if board is valid, false otherwise</returns>
    bool CheckBoardValidity(SudokuDigit[,] cells);

    /// <summary>
    /// Check if the board has no solution, 1 solution, or more than 1 solution.
    /// </summary>
    /// <param name="originalCells"></param>
    /// <returns></returns>
    Solutions SolutionCount(SudokuDigit[,] originalCells);

}