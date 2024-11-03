using FastEndpoints;

namespace Laundro.API.Features.Employees;

public class EmployeeGroup : Group
{
    public EmployeeGroup()
    {
        Configure("employee", ep =>
        {
            ep.Description(x => x.Produces(401).WithTags("Employee"));
        });
    }
}
