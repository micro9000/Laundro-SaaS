using Laundro.Core.Data;
using Laundro.Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace Laundro.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ToDoListController : ControllerBase
{
    private readonly LaundroDbContext _laundroDbContext;

    public ToDoListController(LaundroDbContext laundroDbContext)
    {
        _laundroDbContext = laundroDbContext;
    }

    [HttpGet]
    [RequiredScopeOrAppPermission(RequiredScopesConfigurationKey = "AzureAD:Scopes:Read")]
    public async Task<IActionResult> GetAsync()
    {
        var users = await _laundroDbContext.Users.ToListAsync();

        return Ok(users);
    }

    [HttpPost]
    [RequiredScopeOrAppPermission(RequiredScopesConfigurationKey = "AzureAD:Scopes:Write")]
    public async Task<IActionResult> PostAsync([FromBody] ToDo toDo)
    {
        // Only let applications with global to-do access set the user ID or to-do's
        var ownerIdOfTodo = IsAppMakingRequest() ? toDo.Owner : GetUserId();

        var newToDo = new ToDo()
        {
            Owner = ownerIdOfTodo,
            Description = toDo.Description
        };

        await _laundroDbContext.ToDos!.AddAsync(newToDo);
        await _laundroDbContext.SaveChangesAsync();

        return Created($"/todo/{newToDo!.Id}", newToDo);
    }

    private bool RequestCanAccessToDo(Guid userId)
    {
        return IsAppMakingRequest() || (userId == GetUserId());
    }

    private Guid GetUserId()
    {
        Guid userId;
        if (!Guid.TryParse(HttpContext.User.GetObjectId(), out userId))
        {
            throw new Exception("User ID is not valid.");
        }
        return userId;
    }

    private bool IsAppMakingRequest()
    {
        if (HttpContext.User.Claims.Any(c => c.Type == "idtyp"))
        {
            return HttpContext.User.Claims.Any(c => c.Type == "idtyp" && c.Value == "app");
        }
        else
        {
            return HttpContext.User.Claims.Any(c => c.Type == "roles") && !HttpContext.User.Claims.Any(c => c.Type == "scp");
        }
    }
}
