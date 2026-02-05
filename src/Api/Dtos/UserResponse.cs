namespace Assessment.Api.Dtos;

public sealed class UserResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<GroupSummary> Groups { get; set; } = new();

    public sealed class GroupSummary
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
