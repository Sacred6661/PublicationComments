using CommentService.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace CommentService.DTOs
{
    public class AddCommentDto
    {
        public string Text { get; set; }
        public string UserId { get; set; }
        [Required]
        public long PostId {  get; set; }
    }
}
