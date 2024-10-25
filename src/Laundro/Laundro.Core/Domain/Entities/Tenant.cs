namespace Laundro.Core.Domain.Entities
{
    public class Tenant : Entity
    {
        public int OwnerId { get; set; }
        public User? Owner { get; set; }
        public string? TenantName { get; set; }
        public Guid TenantGuid { get; set; }

        public string? CompanyAddress { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? BusinessRegistrationNumber { get; set; }
        public string? PrimaryContactName { get; set; }
        public string? ContactEmail { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
