namespace Laundro.MicrosoftEntraId.AuthExtension;
public class RedisConnectionOptions
{
    public const string RedisConnection = "RedisConnection";

    public string? Host { get; set; }
    public string? Port { get; set; }
    public bool IsSSL { get; set; }
    public string? Password { get; set; }
}
