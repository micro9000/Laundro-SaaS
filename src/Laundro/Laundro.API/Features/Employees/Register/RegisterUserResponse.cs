using Laundro.Core.Domain.Entities;

namespace Laundro.API.Features.Employees.Register;

internal sealed class RegisterUserResponse
{
    public User? User { get; set; }
}