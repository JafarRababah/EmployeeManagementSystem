namespace EmployeesManagment.Models
{
    public class Notification:UserActivity
    {
        public int Id { get; set; }
        public string UserId { get; set; }   // المستلم
        public string Message { get; set; }
        public string? Url { get; set; }
        public bool IsRead { get; set; } = false;
    }

}
