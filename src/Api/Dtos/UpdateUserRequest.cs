namespace Assessment.Api.Dtos;

public sealed class UpdateUserRequest
{
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public bool? IsActive { get; set; }

    // optional: replace group memberships
    public List<int>? GroupIds { get; set; }
}
