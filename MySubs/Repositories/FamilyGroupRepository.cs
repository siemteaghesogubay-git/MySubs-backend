using Microsoft.EntityFrameworkCore;
using MySubs.Data;
using MySubs.Models;
using MySubs.Repositories.interfaces;

namespace MySubs.Repositories
{
    public class FamilyGroupRepository : IFamilyGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public FamilyGroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FamilyGroup?> GetByIdAsync(int id)
        {
            return await _context.FamilyGroups
                .Include(fg => fg.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(fg => fg.Id == id);
        }

        public async Task<List<FamilyGroup>> GetGroupsForUserAsync(string userId)
        {
            return await _context.FamilyGroups
                .Include(fg => fg.Members)
                    .ThenInclude(m => m.User)
                .Where(fg => fg.Members.Any(m => m.UserId == userId))
                .ToListAsync();
        }

        public async Task<FamilyGroup> CreateAsync(FamilyGroup group)
        {
            _context.FamilyGroups.Add(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public async Task<FamilyGroupMember> AddMemberAsync(FamilyGroupMember member)
        {
            _context.FamilyGroupMembers.Add(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<bool> RemoveMemberAsync(int familyGroupId, string userId)
        {
            return await _context.FamilyGroupMembers
                .Where(m => m.FamilyGroupId == familyGroupId && m.UserId == userId)
                .ExecuteDeleteAsync() > 0;
        }

        public async Task<bool> IsMemberAsync(int familyGroupId, string userId)
        {
            return await _context.FamilyGroupMembers
                .AnyAsync(m => m.FamilyGroupId == familyGroupId && m.UserId == userId);
        }
    }
}
