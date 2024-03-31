using Sudoku.App.Enums;
using Sudoku.App.Helpers;

namespace Sudoku.App.Models;

public record SudokuWithId(Guid Id, SudokuBoard<SudokuDigit> Board, Solutions Solutions);