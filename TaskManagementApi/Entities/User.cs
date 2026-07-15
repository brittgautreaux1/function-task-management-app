using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.Entities
{
    public class User : IdentityUser
    {
        // [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        // [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}