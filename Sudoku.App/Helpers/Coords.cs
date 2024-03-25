namespace Sudoku.App.Helpers;

public record struct Coords(int Row, int Column)
{
    public static Coords BlockCoords(int row, int col, int offset) =>
        new(row / 3 * 3 + offset / 3, col / 3 * 3 + offset % 3);
    
    public static Coords BlockCoords(Coords coords, int offset) =>
        new(coords.Row / 3 * 3 + offset / 3, coords.Column / 3 * 3 + offset % 3);
    
    public static Coords BlockCoords(int row, int col) =>
        new(row / 3 * 3 + col / 3, row % 3 * 3 + col % 3);
}