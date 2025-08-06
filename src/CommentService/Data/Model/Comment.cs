namespace CommentService.Data.Model
{
    public class Comment
    {
        public long Id { get; set; }
        public long UserInfoId { get; set; }
        public string Text { get; set; }
        public DateTime? DateAdded { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; }
        public CommentStatusEnum Status { get; set; } = CommentStatusEnum.Pending;
        public long PostId { get; set; }
    }
    }
