using Sudoku.App.Enums;

namespace Sudoku.App.Models;

public record DailySudokuStatus(Guid Id, DateOnly Date, BoardStatus Status);
