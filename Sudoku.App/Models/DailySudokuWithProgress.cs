using Sudoku.App.Helpers;

namespace Sudoku.App.Models;

public record DailySudokuWithProgress(Guid Id, TimeSpan TimeTaken, SudokuBoard<SudokuCell> Board);