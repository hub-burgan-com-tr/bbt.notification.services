namespace Notification.Profile.Model.Database
{
    public class LogDetail
    {
        public int Id { get; set; }
        public int LogId { get; set; }
        public string ResponseData { get; set; }
        public string RequestData { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime RequestDate { get; set; }

    }
}
