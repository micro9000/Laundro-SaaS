namespace Laundro.Core.Models
{
    public class Tenant : Entity
    {
        public int OwnerId { get; set; }
        public User? Owner { get; set; }
    }
}
