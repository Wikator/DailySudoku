using Sudoku.App.Enums;
using Sudoku.App.Helpers;
using Sudoku.App.Models;

namespace Sudoku.App.Repositories.Contracts;

public interface ISudokuRepository
{
    Task CreateSudokuAsync(SudokuDigit[,] board, Solutions solutions, string userId);
    Task<PagedResult<SudokuWithId>> GetUserPagedSudoku(string userId, int pageNumber, int pageSize);
    Task<SudokuBoard<SudokuCell>?> GetSudoku(string id);
    Task CreateDailySudokuAsync(SudokuBoard<SudokuDigit> board, DateTime date);
    Task<SudokuBoard<SudokuCell>> GetLatestDailySudokuAsync();
}