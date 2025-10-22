using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Services.Interfaces;
using Shared.DTOs;
using AutoMapper;
using Shared.Entities;
using UserService.Infrastructure.Repositories.Interfaces;

namespace Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> CreateAsync(UserCreateDto dto)
        {
            var user = _mapper.Map<User>(dto);

            if(await _repo.ExistsAsync(user.Tel, user.Email))
                throw new InvalidOperationException("A user with the same telephone or email already exists.");
            
            await _repo.AddAsync(user);
            await _repo.SaveChangesAsync();

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto?> GetByIdAsync(Guid id)
        {
            var user = await _repo.GetByIdAsync(id);
            return user is null ? null : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var users = await _repo.GetAllAsync(pageNumber, pageSize) ?? Array.Empty<User>();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto?> UpdateAsync(Guid id, UserUpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return null;

            if (await _repo.ExistsAsync(
                    string.IsNullOrWhiteSpace(dto.Tel) ? existing.Tel : dto.Tel,
                    string.IsNullOrWhiteSpace(dto.Email) ? existing.Email : dto.Email,
                    existing.UserId))
            {
                throw new InvalidOperationException("A user with the same telephone or email already exists.");
            }

            existing.UpdatePersonalInfo(dto.Name, dto.Surname, dto.DateOfBirth);

            existing.UpdateContactInfo(
                string.IsNullOrWhiteSpace(dto.Tel) ? existing.Tel : dto.Tel,
                string.IsNullOrWhiteSpace(dto.Address) ? existing.Address : dto.Address,
                string.IsNullOrWhiteSpace(dto.Email) ? existing.Email : dto.Email
            );

            if (!Equals(existing.Role, dto.Role))
                existing.ChangeRole(dto.Role);

            await _repo.SaveChangesAsync();

            return _mapper.Map<UserResponseDto>(existing);
        }



        public async Task<bool> DeleteAsync(Guid id)
        {
            var ok = await _repo.SoftDeleteAsync(id);
            if (!ok) return false;

            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreAsync(Guid id)
        {
            var ok = await _repo.RestoreAsync(id);
            if (!ok) return false;

            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
