using Assessment.Api.Dtos;
using Assessment.Domain.Entities;
using Assessment.Api.Errors;
using Assessment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Api.Services;

public sealed class UserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db) => _db = db;

    public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                DisplayName = u.DisplayName,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                Groups = u.UserGroups.Select(ug => new UserResponse.GroupSummary
                {
                    Id = ug.GroupId,
                    Name = ug.Group.Name
                }).ToList()
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<UserResponse>> ListAsync(CancellationToken ct)
    {
        return await _db.Users
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                DisplayName = u.DisplayName,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                Groups = u.UserGroups.Select(ug => new UserResponse.GroupSummary
                {
                    Id = ug.GroupId,
                    Name = ug.Group.Name
                }).ToList()
            })
            .ToListAsync(ct);
    }

    public async Task<int> CreateAsync(CreateUserRequest req, CancellationToken ct)
    {
        // basic validation
        req.Email = req.Email.Trim();
        req.DisplayName = req.DisplayName.Trim();

        var exists = await _db.Users.AnyAsync(u => u.Email == req.Email, ct);
        if (exists)
            throw new ConflictException("Email already exists.");

        // validate groups (if provided)
        if (req.GroupIds.Count > 0)
        {
            var validGroupCount = await _db.Groups.CountAsync(g => req.GroupIds.Contains(g.Id), ct);
            if (validGroupCount != req.GroupIds.Distinct().Count())
                throw new BadRequestException("One or more GroupIds are invalid.");

        }

        var user = new User
        {
            Email = req.Email,
            DisplayName = req.DisplayName,
            IsActive = req.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var gid in req.GroupIds.Distinct())
        {
            user.UserGroups.Add(new UserGroup { GroupId = gid, AddedUtc = DateTime.UtcNow });
        }

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return user.Id;
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserRequest req, CancellationToken ct)
    {
        var user = await _db.Users
            .Include(u => u.UserGroups)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user is null) return false;

        if (req.Email is not null)
        {
            var newEmail = req.Email.Trim();
            var emailExists = await _db.Users.AnyAsync(u => u.Email == newEmail && u.Id != id, ct);
            if (emailExists)
                throw new ConflictException("Email already exists.");

            user.Email = newEmail;
        }

        if (req.DisplayName is not null)
            user.DisplayName = req.DisplayName.Trim();

        if (req.IsActive.HasValue)
            user.IsActive = req.IsActive.Value;

        // replace group membership if provided
        if (req.GroupIds is not null)
        {
            var distinct = req.GroupIds.Distinct().ToList();
            var validGroupCount = await _db.Groups.CountAsync(g => distinct.Contains(g.Id), ct);
            if (validGroupCount != distinct.Count)
                throw new BadRequestException("One or more GroupIds are invalid.");

            user.UserGroups.Clear();
            foreach (var gid in distinct)
                user.UserGroups.Add(new UserGroup { UserId = user.Id, GroupId = gid, AddedUtc = DateTime.UtcNow });
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null) return false;

        _db.Users.Remove(user);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public Task<int> TotalCountAsync(CancellationToken ct)
        => _db.Users.AsNoTracking().CountAsync(ct);

    public async Task<List<UsersPerGroupResponse>> UsersPerGroupAsync(CancellationToken ct)
    {
        // includes groups with 0 users
        var results = await _db.Groups
            .AsNoTracking()
            .Select(g => new UsersPerGroupResponse
            {
                GroupId = g.Id,
                GroupName = g.Name,
                UserCount = g.UserGroups.Count
            })
            .OrderBy(x => x.GroupName)
            .ToListAsync(ct);

        return results;
    }
}
