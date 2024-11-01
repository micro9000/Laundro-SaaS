using NodaTime;

namespace Laundro.Core.Storage;
public class InputFileStorageInformation
{
    public required Guid Id { get; set; }
    public required Guid TenantGuid { get; set; }
    public required string? FileName { get; set; }
    public required Instant DateUploaded { get; set; }
}
