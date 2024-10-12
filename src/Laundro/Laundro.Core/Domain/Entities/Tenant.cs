namespace Laundro.Core.Domain.Entities
{
    public class Tenant : Entity
    {
        public int OwnerId { get; set; }
        public User? Owner { get; set; }
        public string? CampanyName { get; set; }
    }
}
