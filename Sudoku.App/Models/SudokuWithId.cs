using Sudoku.App.Enums;
using Sudoku.App.Helpers;

namespace Sudoku.App.Models;

public record SudokuWithId<T>(Guid Id, SudokuBoard<T> Board);