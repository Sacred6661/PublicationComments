namespace ProfileService.Data.Models
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public DateTime? DateUpdated { get; set; }
    }
}
