using Sudoku.App.Enums;

namespace Sudoku.App.Helpers;

public class SudokuCell
{
    public bool IsFixed { get; init; }
    private SudokuDigit _value;

    public SudokuDigit Value
    {
        get => _value;
        set
        {
            if (IsFixed)
                throw new InvalidOperationException("Cannot change the value of a fixed cell.");

            _value = value;
        }
    }

    public SudokuCell Clone() =>
        new()
        {
            IsFixed = IsFixed,
            _value = _value
        };
}
