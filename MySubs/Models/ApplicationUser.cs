using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MySubs.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}