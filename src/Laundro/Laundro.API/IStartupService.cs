namespace Laundro.API;

public interface IStartupService
{
    Task Initialize(CancellationToken cancellation);
}
