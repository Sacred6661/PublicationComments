namespace UserService.Data.Models
{
    public class UserLoginLog
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public DateTime? LoginTime { get; set; }
        public string IpAddress {  get; set; }
        public string UserAgent { get; set; }
        public bool? IsSuccess {  get; set; }
    }
}
