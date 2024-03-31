using Sudoku.App.Helpers;

namespace Sudoku.App.Models;

public record SudokuWithIdAndValidation(Guid Id, SudokuBoard<SudokuCell> Board);