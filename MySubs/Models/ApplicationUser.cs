using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MySubs.Models
{
    public class ApplicationUser : IdentityUser

    {
        [Required]
        [MaxLength(15)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string LastName { get; set; } = string.Empty;
            

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}