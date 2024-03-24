using System.ComponentModel.DataAnnotations;

namespace Sudoku.App.Models.FormModels;

public class RegisterModel
{
    [EmailAddress, Required] public string Email { get; set; } = "";
    [Required, Length(minimumLength:3, maximumLength:12)] public string UserName { get; set; } = "";
    [Required, Length(minimumLength:6, maximumLength:20)] public string Password { get; set; } = "";
    [Required, Compare(nameof(Password))] public string ConfirmPassword { get; set; } = "";
}