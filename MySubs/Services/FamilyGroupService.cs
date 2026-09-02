using Microsoft.AspNetCore.Identity;
using MySubs.Dtos;
using MySubs.Models;
using MySubs.Repositories.interfaces;
using MySubs.Services.IServices;

namespace MySubs.Services
{
    public class FamilyGroupService : IFamilyGroupService
    {
        private readonly IFamilyGroupRepository _familyGroupRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public FamilyGroupService(
            IFamilyGroupRepository familyGroupRepository,
            UserManager<ApplicationUser> userManager)
        {
            _familyGroupRepository = familyGroupRepository;
            _userManager = userManager;
        }

        public async Task<List<FamilyGroupResponseDto>> GetMyGroupsAsync(string userId)
        {
            var groups = await _familyGroupRepository.GetGroupsForUserAsync(userId);
            return groups.Select(MapToResponseDto).ToList();
        }

        public async Task<FamilyGroupResponseDto?> GetByIdAsync(int id, string requestingUserId)
        {
            var isMember = await _familyGroupRepository.IsMemberAsync(id, requestingUserId);
            if (!isMember) return null; // säkerhet: bara medlemmar kan se gruppen

            var group = await _familyGroupRepository.GetByIdAsync(id);
            return group is null ? null : MapToResponseDto(group);
        }

        public async Task<FamilyGroupResponseDto> CreateAsync(FamilyGroupCreateDto dto, string creatorUserId)
        {
            var group = new FamilyGroup
            {
                Name = dto.Name,
                CreatedByUserId = creatorUserId
            };

            var created = await _familyGroupRepository.CreateAsync(group);

            // Skaparen blir automatiskt medlem med rollen Parent
            await _familyGroupRepository.AddMemberAsync(new FamilyGroupMember
            {
                FamilyGroupId = created.Id,
                UserId = creatorUserId,
                Role = FamilyRole.Parent
            });

            var fullGroup = await _familyGroupRepository.GetByIdAsync(created.Id);
            return MapToResponseDto(fullGroup!);
        }

        public async Task<FamilyGroupResponseDto?> AddMemberAsync(int familyGroupId, AddMemberDto dto, string requestingUserId)
        {
            // Säkerhet: bara befintliga medlemmar (typiskt föräldrar) får lägga till nya
            var isMember = await _familyGroupRepository.IsMemberAsync(familyGroupId, requestingUserId);
            if (!isMember) return null;

            var userToAdd = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new ArgumentException("Användaren hittades inte.");

            var alreadyMember = await _familyGroupRepository.IsMemberAsync(familyGroupId, userToAdd.Id);
            if (alreadyMember)
                throw new InvalidOperationException("Användaren är redan medlem i gruppen.");

            await _familyGroupRepository.AddMemberAsync(new FamilyGroupMember
            {
                FamilyGroupId = familyGroupId,
                UserId = userToAdd.Id,
                Role = dto.Role
            });

            var group = await _familyGroupRepository.GetByIdAsync(familyGroupId);
            return MapToResponseDto(group!);
        }

        public async Task<bool> RemoveMemberAsync(int familyGroupId, string userIdToRemove, string requestingUserId)
        {
            var isMember = await _familyGroupRepository.IsMemberAsync(familyGroupId, requestingUserId);
            if (!isMember) return false;

            return await _familyGroupRepository.RemoveMemberAsync(familyGroupId, userIdToRemove);
        }

        private static FamilyGroupResponseDto MapToResponseDto(FamilyGroup group)
        {
            return new FamilyGroupResponseDto
            {
                Id = group.Id,
                Name = group.Name,
                CreatedByUserId = group.CreatedByUserId,
                Members = group.Members.Select(m => new FamilyMemberResponseDto
                {
                    UserId = m.UserId,
                    Email = m.User.Email ?? string.Empty,
                    FirstName = m.User.FirstName,
                    LastName = m.User.LastName,
                    Role = m.Role,
                    JoinedAt = m.JoinedAt
                }).ToList()
            };
        }
    }
}