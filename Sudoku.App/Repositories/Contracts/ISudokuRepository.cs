using Sudoku.App.Enums;
using Sudoku.App.Helpers;
using Sudoku.App.Models;

namespace Sudoku.App.Repositories.Contracts;

public interface ISudokuRepository
{
    Task CreateSudokuAsync(SudokuDigit[,] board, Solutions solutions, string userId);
    Task RemoveSudokuFromSaved(string userId, string sudokuId);
    Task<PagedResult<SudokuWithId>> GetUserPagedSudoku(string userId, int pageNumber, int pageSize);
    Task<SudokuBoard<SudokuCell>?> GetSudoku(string id);
    Task CreateDailySudokuAsync(SudokuBoard<SudokuDigit> board, DateOnly date);
    Task<SudokuWithIdAndValidation> GetDailySudokuAsync(string? userId, DateOnly date);
    Task<bool> DailySudokuExists(DateOnly date);
    Task<List<DailySudokuStatus>> GetDailySudokuStatuses(string? userId);
    Task SaveDailySudokuProgress(string userId, string sudokuId, SudokuBoard<SudokuDigit> board, bool isSolved);
}