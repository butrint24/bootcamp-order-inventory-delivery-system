using System;
using Shared.Enums;

namespace Shared.Entities
{
    public class User
    {
        public Guid UserId { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; } = null!;
        public string Surname { get; private set; } = null!;
        public DateTime? DateOfBirth { get; private set; }
        public string Tel { get; private set; } = null!;
        public string Address { get; private set; } = null!;
        public RoleType Role { get; private set; }

        private User() { }

        public User(string name, string surname, DateTime? dateOfBirth, string tel, string address, RoleType role)
        {
            Name = name;
            Surname = surname;
            DateOfBirth = dateOfBirth;
            Tel = tel;
            Address = address;
            Role = role;
        }

        public void UpdateContactInfo(string tel, string address)
        {
            Tel = tel;
            Address = address;
        }

        public void ChangeRole(RoleType newRole)
        {
            Role = newRole;
        }
    }
}
