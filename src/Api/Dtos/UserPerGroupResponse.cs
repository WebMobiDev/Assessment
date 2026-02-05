namespace Assessment.Api.Dtos;

public sealed class UsersPerGroupResponse
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public int UserCount { get; set; }
}
