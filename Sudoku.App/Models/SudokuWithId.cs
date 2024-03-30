using Sudoku.App.Helpers;

namespace Sudoku.App.Models;

public record SudokuWithId(Guid Id, SudokuBoard<SudokuCell> Board);