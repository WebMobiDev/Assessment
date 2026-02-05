using System.Net.Http.Json;
using Assessment.Web.Models;

namespace Assessment.Web.ApiClient;

public sealed class UsersApiClient
{
    private readonly HttpClient _http;

    public UsersApiClient(HttpClient http) => _http = http;

    public async Task<List<UserVm>> GetUsersAsync(CancellationToken ct)
        => await _http.GetFromJsonAsync<List<UserVm>>("/api/users", ct) ?? new();

    public async Task<UserVm?> GetUserAsync(int id, CancellationToken ct)
        => await _http.GetFromJsonAsync<UserVm>($"/api/users/{id}", ct);

    public async Task<UserVm?> CreateUserAsync(CreateUserVm vm, CancellationToken ct)
    {
        var resp = await _http.PostAsJsonAsync("/api/users", vm, ct);
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<UserVm>(cancellationToken: ct);
    }

    public async Task<bool> UpdateUserAsync(int id, UpdateUserVm vm, CancellationToken ct)
    {
        var resp = await _http.PutAsJsonAsync($"/api/users/{id}", vm, ct);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUserAsync(int id, CancellationToken ct)
    {
        var resp = await _http.DeleteAsync($"/api/users/{id}", ct);
        return resp.IsSuccessStatusCode;
    }
}
