namespace Assessment.Web.Models;
using System.ComponentModel.DataAnnotations;
public sealed class UpdateUserVm
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Display Name is required.")]
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; }
    
    public List<int>? GroupIds { get; set; }
}
