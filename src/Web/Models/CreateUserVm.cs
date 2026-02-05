namespace Assessment.Web.Models;
using System.ComponentModel.DataAnnotations;
public sealed class CreateUserVm
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]   
     public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Display Name is required.")]
    public string DisplayName { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    // optional (only if you want group assignment in UI)
    public List<int> GroupIds { get; set; } = new();
}
