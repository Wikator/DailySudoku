using Sudoku.App.Enums;
using Sudoku.App.Helpers;

namespace Sudoku.App.Extensions;

public static class ArrayExtensions
{
    /// <summary>
    /// Gets value from 2D array using Coords record.
    /// </summary>
    /// <param name="array">2D array</param>
    /// <param name="coords">Coordinates</param>
    /// <typeparam name="T">Type of 2D array</typeparam>
    /// <returns>An object from the array</returns>
    public static T Get<T>(this T[,] array, Coords coords) =>
        array[coords.Row, coords.Column];
    
    /// <summary>
    /// Sets value in 2D array using Coords record.
    /// </summary>
    /// <param name="array">2D array</param>
    /// <param name="coords">Coordinates</param>
    /// <param name="value">Value to set</param>
    /// <typeparam name="T">Type of 2D array</typeparam>
    public static void Set<T>(this T[,] array, Coords coords, T value) =>
        array[coords.Row, coords.Column] = value;
    
    public static void Fill<T>(this T[,] array, T value)
    {
        for (var i = 0; i < array.GetLength(0); i++)
        {
            for (var j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] = value;
                
            }
        }
    }
    
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