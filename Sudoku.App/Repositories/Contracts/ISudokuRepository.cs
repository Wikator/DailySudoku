using Sudoku.App.Enums;
using Sudoku.App.Helpers;

namespace Sudoku.App.Repositories.Contracts;

public interface ISudokuRepository
{
    Task CreateSudokuAsync(SudokuDigit[,] board, Solutions solutions, string userId);
    Task CreateDailySudokuAsync(SudokuBoard<SudokuDigit> board, DateTime date);
}