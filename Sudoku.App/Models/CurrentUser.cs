namespace Sudoku.App.Models;

public class CurrentUser
{
    public Guid Id { get; init; }
    public required string Email { get; init; }
    public required string UserName { get; init; }
}