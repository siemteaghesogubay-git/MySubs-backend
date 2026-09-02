using MySubs.Models;

namespace MySubs.Repositories.interfaces
{
    public interface IFamilyGroupRepository
    {
        Task<FamilyGroup?> GetByIdAsync(int id);
        Task<List<FamilyGroup>> GetGroupsForUserAsync(string userId);
        Task<FamilyGroup> CreateAsync(FamilyGroup group);
        Task<FamilyGroupMember> AddMemberAsync(FamilyGroupMember member);
        Task<bool> RemoveMemberAsync(int familyGroupId, string userId);
        Task<bool> IsMemberAsync(int familyGroupId, string userId);
    }
}