using CommentService.Data.Model;

namespace CommentService.DTOs
{
    public class CommentFullInfoDto
    {
        public long CommentId { get; set; }
        public long UserInfoId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Text { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }
        public CommentStatusEnum Status { get; set; }
    }
}
