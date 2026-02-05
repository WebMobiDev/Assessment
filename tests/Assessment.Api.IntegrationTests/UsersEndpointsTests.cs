using System.Net;
using System.Net.Http.Json;
using Assessment.Api.Dtos;
using System.Linq;
using Xunit;

namespace Assessment.Api.IntegrationTests;

public sealed class UsersEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersEndpointsTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Post_creates_user_returns_201()
    {
        var req = new CreateUserRequest
        {
            Email = "test@example.com",
            DisplayName = "Test User",
            IsActive = true
        };

        var resp = await _client.PostAsJsonAsync("/api/users", req);

        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);

        var created = await resp.Content.ReadFromJsonAsync<UserResponse>();
        Assert.NotNull(created);
        Assert.Equal("test@example.com", created!.Email);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task Get_count_returns_total_users()
    {
        await _client.PostAsJsonAsync("/api/users", new CreateUserRequest
        {
            Email = "count1@example.com",
            DisplayName = "Count 1",
            IsActive = true
        });

        var resp = await _client.GetAsync("/api/users/count");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var payload = await resp.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        Assert.NotNull(payload);
        Assert.True(payload!.ContainsKey("count"));
        Assert.True(payload["count"] > 0);
    }

    [Fact]
    public async Task Delete_removes_user()
    {
        var create = await _client.PostAsJsonAsync("/api/users", new CreateUserRequest
        {
            Email = "del@example.com",
            DisplayName = "Delete Me",
            IsActive = true
        });

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var created = await create.Content.ReadFromJsonAsync<UserResponse>();
        Assert.NotNull(created);

        var id = created!.Id;

        var del = await _client.DeleteAsync($"/api/users/{id}");
        Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);

        var get = await _client.GetAsync($"/api/users/{id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task Post_duplicate_email_returns_409()
    {
        var req = new CreateUserRequest
        {
            Email = "dup@example.com",
            DisplayName = "Dup",
            IsActive = true
        };

        var first = await _client.PostAsJsonAsync("/api/users", req);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await _client.PostAsJsonAsync("/api/users", req);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
public async Task Put_updates_user_and_persists_changes()
{
    // Create a user
    var createResp = await _client.PostAsJsonAsync("/api/users", new CreateUserRequest
    {
        Email = "update@example.com",
        DisplayName = "Before",
        IsActive = true
    });

    Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);

    var created = await createResp.Content.ReadFromJsonAsync<UserResponse>();
    Assert.NotNull(created);
    var id = created!.Id;

    // Update user
    var updateResp = await _client.PutAsJsonAsync($"/api/users/{id}", new UpdateUserRequest
    {
        DisplayName = "After",
        IsActive = false
    });

    Assert.Equal(HttpStatusCode.NoContent, updateResp.StatusCode);

    // Fetch and verify
    var getResp = await _client.GetAsync($"/api/users/{id}");
    Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

    var updated = await getResp.Content.ReadFromJsonAsync<UserResponse>();
    Assert.NotNull(updated);
    Assert.Equal("After", updated!.DisplayName);
    Assert.False(updated.IsActive);
}

[Fact]
public async Task Get_count_per_group_includes_created_user_membership()
{
    // Create a user assigned to Admin group (Id=1)
    var createResp = await _client.PostAsJsonAsync("/api/users", new CreateUserRequest
    {
        Email = "groupcount@example.com",
        DisplayName = "Group Count",
        IsActive = true,
        GroupIds = new List<int> { 1 }
    });

    Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);

    // Call count-per-group
    var resp = await _client.GetAsync("/api/users/count-per-group");
    Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

    var rows = await resp.Content.ReadFromJsonAsync<List<UsersPerGroupResponse>>();
    Assert.NotNull(rows);
    Assert.True(rows!.Count > 0);

    // Find Admin group row
    var admin = rows.FirstOrDefault(x => x.GroupId == 1);
    Assert.NotNull(admin);

    // Should be at least 1 (we just added one)
    Assert.True(admin!.UserCount >= 1);
}

[Fact]
public async Task Post_invalid_email_returns_400()
{
    var resp = await _client.PostAsJsonAsync("/api/users", new CreateUserRequest
    {
        Email = "not-an-email",
        DisplayName = "Bad Email",
        IsActive = true
    });

    Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
}

[Fact]
public async Task Post_invalid_groupId_returns_400()
{
    var resp = await _client.PostAsJsonAsync("/api/users", new CreateUserRequest
    {
        Email = "badgroup@example.com",
        DisplayName = "Bad Group",
        IsActive = true,
        GroupIds = new List<int> { 9999 }
    });

    Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
}


}
