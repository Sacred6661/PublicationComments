namespace CommentService.Data.Model
{
    public class CommentStatus
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
