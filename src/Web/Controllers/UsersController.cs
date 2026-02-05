using Assessment.Web.ApiClient;
using Assessment.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Web.Controllers;

public sealed class UsersController : Controller
{
    private readonly UsersApiClient _api;

    public UsersController(UsersApiClient api) => _api = api;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var users = await _api.GetUsersAsync(ct);
        return View(users);
    }

    public IActionResult Create() => View(new CreateUserVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        var created = await _api.CreateUserAsync(vm, ct);
        if (created is null)
        {
            ModelState.AddModelError("", "Failed to create user (API rejected request).");
            return View(vm);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var user = await _api.GetUserAsync(id, ct);
        if (user is null) return NotFound();

        var vm = new UpdateUserVm
        {
            Email = user.Email,
            DisplayName = user.DisplayName,
            IsActive = user.IsActive
        };

        ViewBag.UserId = id;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateUserVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.UserId = id;
            return View(vm);
        }

        var ok = await _api.UpdateUserAsync(id, vm, ct);
        if (!ok)
        {
            ModelState.AddModelError("", "Failed to update user (API rejected request).");
            ViewBag.UserId = id;
            return View(vm);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var user = await _api.GetUserAsync(id, ct);
        if (user is null) return NotFound();
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        await _api.DeleteUserAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}
