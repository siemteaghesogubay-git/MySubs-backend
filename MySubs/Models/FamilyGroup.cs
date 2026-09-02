namespace MySubs.Models
{
    public class FamilyGroup
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty; // t.ex. "Familjen Andersson"

        public string CreatedByUserId { get; set; } = string.Empty;
        public ApplicationUser CreatedByUser { get; set; } = null!;

        public ICollection<FamilyGroupMember> Members { get; set; } = new List<FamilyGroupMember>();
    }
}