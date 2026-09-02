namespace MySubs.Models
{
    public enum FamilyRole { Parent, Child }

    public class FamilyGroupMember
    {
        public int Id { get; set; }

        public int FamilyGroupId { get; set; }
        public FamilyGroup FamilyGroup { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public FamilyRole Role { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
