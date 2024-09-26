namespace Laundro.Core.Data;
public class DistributedCacheSettings
{
    public const string SectionName = "DistributedCache";
    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = "LaundroInstance";
}
