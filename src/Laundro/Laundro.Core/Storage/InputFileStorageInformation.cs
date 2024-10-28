using NodaTime;

namespace Laundro.Core.Storage;
public class InputFileStorageInformation
{
    public Guid Id { get; set; }
    public string? FileName { get; set; }
    public Instant DateUploaded { get; set; }
}
