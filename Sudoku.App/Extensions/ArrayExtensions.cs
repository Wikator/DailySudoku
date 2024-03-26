using Sudoku.App.Enums;
using Sudoku.App.Helpers;

namespace Sudoku.App.Extensions;

public static class ArrayExtensions
{
    public static string ToBoardString(this SudokuDigit[,] board) =>
        string.Join("", board.Cast<SudokuDigit>().Select(d => (int)d));
    
    public static SudokuDigit[,] ToDigits(this SudokuCell[,] board)
    {
        var newBoard = new SudokuDigit[9, 9];
        
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                newBoard[i, j] = board[i, j].Value;
            }
        }
        
        return newBoard;
    }
}