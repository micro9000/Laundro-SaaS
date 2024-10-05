using Laundro.Core.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laundro.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public UserController(ICurrentUserAccessor currentUserAccessor)
    {
        _currentUserAccessor = currentUserAccessor;
    }

    [HttpGet("context")]
    public IActionResult GetContext()
    {
        var currentUserContext = _currentUserAccessor.GetCurrentUser();
        return Ok(currentUserContext);
    }
}
