namespace Notification.Profile.Helper
{
    public class LogHelper : ILogHelper
    {
        public bool LogCreate(object requestModel, object responseModel, string methodName, string errorMessage)
        {
            return false;
            //using (var db = new DatabaseContext())
            //{
            //    var log = db.Consumers.Where(s =>
            //        s.Client == client &&
            //        s.User == user &&
            //        (source == null || source.HasValue || s.SourceId == source.Value))
            //    .AsNoTracking();

            //    returnValue.Consumers = consumers.Select(c =>
            //        new GetUserConsumersResponse.Consumer
            //        {
            //            Source = c.SourceId,
            //            Filter = c.Filter,
            //            IsPushEnabled = c.IsPushEnabled,
            //            DeviceKey = c.DeviceKey,
            //            IsSmsEnabled = c.IsSmsEnabled,
            //            Phone = c.Phone,
            //            IsEmailEnabled = c.IsEmailEnabled,
            //            Email = c.Email
            //        }

            //    ).ToList();
            //}
        }
    }
}
