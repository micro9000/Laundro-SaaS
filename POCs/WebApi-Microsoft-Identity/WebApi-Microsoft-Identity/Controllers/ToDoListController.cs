using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using WebApi_Microsoft_Identity.models;

namespace WebApi_Microsoft_Identity.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ToDoListController : ControllerBase
{
    private readonly ToDoContext _toDoContext;

    public ToDoListController(ToDoContext toDoContext)
    {
        _toDoContext = toDoContext;
    }

    [HttpGet]
    [RequiredScopeOrAppPermission(
    RequiredScopesConfigurationKey = "AzureAD:Scopes:user_impersonation",
    RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:All.ReadWrite")]
    public async Task<IActionResult> GetAsync()
    {
        var toDos = await _toDoContext.ToDos!
            .Where(td => RequestCanAccessToDo(td.Owner))
            .ToListAsync();

        return Ok(toDos);
    }

    [HttpPost]
    [RequiredScopeOrAppPermission(
    RequiredScopesConfigurationKey = "AzureAD:Scopes:user_impersonation",
    RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:All.ReadWrite")]
    public async Task<IActionResult> PostAsync([FromBody] ToDo toDo)
    {
        // Only let applications with global to-do access set the user ID or to-do's
        var ownerIdOfTodo = IsAppMakingRequest() ? toDo.Owner : GetUserId();

        var newToDo = new ToDo()
        {
            Owner = ownerIdOfTodo,
            Description = toDo.Description
        };

        await _toDoContext.ToDos!.AddAsync(newToDo);
        await _toDoContext.SaveChangesAsync();

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

    private bool IsAppMakingRequest() {
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