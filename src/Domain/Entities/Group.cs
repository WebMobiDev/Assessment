namespace Assessment.Domain.Entities;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public ICollection<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();
}