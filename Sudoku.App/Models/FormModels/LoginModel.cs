using System.ComponentModel.DataAnnotations;

namespace Sudoku.App.Models.FormModels;

public class LoginModel
{
    [Required]
    public string Email { get; set; } = "";
    
    [Required]
    public string Password { get; set; } = "";
}