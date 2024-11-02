using Laundro.Core.Domain.Entities;

namespace Laundro.API.Features.Employees.GetAll;

internal sealed class GetAllEmployeesResponse
{
    public IEnumerable<User>? Employees { get; set; }
}