namespace Assessment.Domain.Entities;

public class Permission 
{
    public int Id { get; set; }
    public string Key { get; set; } = null!; //e.g. "users.read"
    public string? Description { get; set; }

    public ICollection<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();
}