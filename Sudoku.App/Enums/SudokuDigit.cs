namespace Sudoku.App.Enums;

public enum SudokuDigit
{
    Empty,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine
}

public static class SudokuDigitExtensions
{
    public static string ToDigitString(this SudokuDigit digit) =>
        digit switch
        {
            SudokuDigit.Empty => string.Empty,
            SudokuDigit.One => "1",
            SudokuDigit.Two => "2",
            SudokuDigit.Three => "3",
            SudokuDigit.Four => "4",
            SudokuDigit.Five => "5",
            SudokuDigit.Six => "6",
            SudokuDigit.Seven => "7",
            SudokuDigit.Eight => "8",
            SudokuDigit.Nine => "9",
            _ => throw new ArgumentOutOfRangeException(nameof(digit))
        };
}
