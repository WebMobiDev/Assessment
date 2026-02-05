using Assessment.Api.Dtos;
using Assessment.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly UserService _svc;

    public UsersController(UserService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> List(CancellationToken ct)
        => Ok(await _svc.ListAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> Get(int id, CancellationToken ct)
    {
        var user = await _svc.GetByIdAsync(id, ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest req, CancellationToken ct)
    {
        try
        {
            var id = await _svc.CreateAsync(req, ct);
            var created = await _svc.GetByIdAsync(id, ct);
            return CreatedAtAction(nameof(Get), new { id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest req, CancellationToken ct)
    {
        try
        {
            var ok = await _svc.UpdateAsync(id, req, ct);
            return ok ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _svc.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("count")]
    public async Task<ActionResult<object>> TotalCount(CancellationToken ct)
        => Ok(new { count = await _svc.TotalCountAsync(ct) });

    [HttpGet("count-per-group")]
    public async Task<ActionResult<List<UsersPerGroupResponse>>> CountPerGroup(CancellationToken ct)
        => Ok(await _svc.UsersPerGroupAsync(ct));
}
