using System.ComponentModel.DataAnnotations;

namespace Frozen_Warehouse.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string PasswordHash { get; set; } = null!; // store hashed
        public string Role { get; set; } = null!; // Admin or StoreKeeper
    }
}