using Sudoku.App.Enums;
using Sudoku.App.Helpers;
using Sudoku.App.Models;

namespace Sudoku.App.Repositories.Contracts;

public interface ISudokuRepository
{
    Task CreateSudokuAsync(SudokuDigit[,] board, Solutions solutions, string userId);
    Task<List<SudokuWithId>> GetUserSudoku(string userId);
    Task<SudokuBoard<SudokuCell>?> GetSudoku(string id);
    Task CreateDailySudokuAsync(SudokuBoard<SudokuDigit> board, DateTime date);
    Task<SudokuBoard<SudokuCell>> GetLatestDailySudokuAsync();
}