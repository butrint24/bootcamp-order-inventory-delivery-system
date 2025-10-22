using System;
using Shared.Enums;

namespace Shared.Entities
{
    public class User
    {
        public Guid UserId { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; } = null!;
        public string Surname { get; private set; } = null!;
        public DateOnly? DateOfBirth { get; private set; }
        public string Tel { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string Address { get; private set; } = null!;
        public bool IsActive { get; private set; } = true;
        public RoleType Role { get; private set; }

        private User() { }

        public User(string name, string surname, DateOnly? dateOfBirth, string tel, string email, string address, RoleType role)
        {
            Name = name;
            Surname = surname;
            DateOfBirth = dateOfBirth;
            Tel = tel;
            Email = email;
            Address = address;
            Role = role;
        }

        public void UpdateContactInfo(string tel, string address, string email)
        {
            Tel = tel;
            Address = address;
            Email = email;
        }

        public void UpdatePersonalInfo(string? name, string? surname, DateOnly? dateOfBirth)
        {
            if (!string.IsNullOrWhiteSpace(name))
                Name = name;

            if (!string.IsNullOrWhiteSpace(surname))
                Surname = surname;

            if (dateOfBirth.HasValue)
                DateOfBirth = dateOfBirth;
        }


        public void ChangeRole(RoleType newRole)
        {
            Role = newRole;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
    }
}
