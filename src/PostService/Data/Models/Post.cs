namespace PostService.Data.Models
{
    public class Post
    {
        public long Id { get; set; }
        public long UserInfoId {  get; set; }
        public string? ImageUrl { get; set; }
        public string? Title { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
    }
}
