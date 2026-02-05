namespace Assessment.Api.Dtos;

public sealed class CreateUserRequest
{
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    // optional: assign to groups on create
    public List<int> GroupIds { get; set; } = new();
}
