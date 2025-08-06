using CommentService.Data.Model;

namespace CommentService.DTOs
{
    public class CommentDto
    {
        public long Id { get; set; }
        public long UserInfoId { get; set; }
        public string Text { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }
        public CommentStatusEnum Status { get; set; }
        public long PostId { get; set; }
    }
}
