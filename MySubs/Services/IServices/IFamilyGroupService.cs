using MySubs.Dtos;

namespace MySubs.Services.IServices
{
    public interface IFamilyGroupService
    {
        Task<List<FamilyGroupResponseDto>> GetMyGroupsAsync(string userId);
        Task<FamilyGroupResponseDto?> GetByIdAsync(int id, string requestingUserId);
        Task<FamilyGroupResponseDto> CreateAsync(FamilyGroupCreateDto dto, string creatorUserId);
        Task<FamilyGroupResponseDto?> AddMemberAsync(int familyGroupId, AddMemberDto dto, string requestingUserId);
        Task<bool> RemoveMemberAsync(int familyGroupId, string userIdToRemove, string requestingUserId);
    }
}