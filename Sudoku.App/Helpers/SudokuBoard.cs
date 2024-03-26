namespace Sudoku.App.Helpers;

/// <summary>
/// Helper class that represents a 9x9 sudoku board. It provides several helper methods
/// and constructors, as well as an indexer for easy access to the cells using Coords struct.
/// </summary>
/// <typeparam name="T">Type of each cell.</typeparam>
public class SudokuBoard<T>
{
    public T[,] Board { get; }
    
    public SudokuBoard()
    {
        Board = new T[9, 9];
    }
    
    public SudokuBoard(T[,] board)
    {
        Board = board;
    }

    public SudokuBoard(T item)
    {
        Board = new T[9, 9];
        Fill(item);
    }

    public SudokuBoard(Func<int, int, T> func)
    {
        Board = new T[9, 9];
        Fill(func);
    }
    
    public void Fill(T item)
    {
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                Board[row, col] = item;
            }
        }
    }

    public void Fill(Func<int, int, T> func)
    {
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                Board[row, col] = func(row, col);
            }
        }
    }

    public T this[int row, int column]
    {
        get => Board[row, column];
        set => Board[row, column] = value;
    }
    
    public T this[Coords coords]
    {
        get => Board[coords.Row, coords.Column];
        set => Board[coords.Row, coords.Column] = value;
    }
}