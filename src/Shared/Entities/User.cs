using System;

namespace YourNamespace.Models
{
    // Enum matching PostgreSQL role_type
    public enum RoleType
    {
        USER,
        ADMIN
    }

    public class User
    {
        // Primary key
        public Guid UserId { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Surname { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Tel { get; set; }

        public string Address { get; set; }

        public RoleType Role { get; set; }
    }
}
