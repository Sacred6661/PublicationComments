using System.ComponentModel.DataAnnotations;

namespace PostService.Data.Models
{
    public class UserInfo
    {
        [Key]
        public long Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateAdded { get; set; } = DateTime.UtcNow;
        public DateTime? DateUpdated { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
