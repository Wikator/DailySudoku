namespace Sudoku.App.Helpers;

public sealed record Coords(int Row, int Column)
{
    public static Coords BlockCoords(int row, int col, int offset) =>
        new(row / 3 * 3 + offset / 3, col / 3 * 3 + offset % 3);
    
    public static Coords BlockCoords(Coords coords, int offset) =>
        new(coords.Row / 3 * 3 + offset / 3, coords.Column / 3 * 3 + offset % 3);
    
    public static Coords BlockCoords(int row, int col) =>
        new(row / 3 * 3 + col / 3, row % 3 * 3 + col % 3);

    public bool Equals(Coords? other) =>
        other is not null && Row == other.Row && Column == other.Column;

    public override int GetHashCode() =>
        HashCode.Combine(Row, Column);
}