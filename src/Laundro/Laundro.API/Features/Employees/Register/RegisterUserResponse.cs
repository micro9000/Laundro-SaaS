using Laundro.Core.Domain.Entities;

namespace Laundro.API.Features.Employees.Register;

internal sealed class RegisterUserResponse
{
    public User? Employee { get; set; }
}