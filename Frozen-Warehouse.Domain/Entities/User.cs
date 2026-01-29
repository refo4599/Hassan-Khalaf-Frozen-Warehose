using System.ComponentModel.DataAnnotations;

namespace Frozen_Warehouse.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!; // plain text for testing only
        public string Role { get; set; } = null!; // Admin or StoreKeeper
    }
}