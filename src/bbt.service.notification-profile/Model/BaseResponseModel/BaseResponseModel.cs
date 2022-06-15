using Notification.Profile.Enum;

namespace Notification.Profile.Model.BaseResponseModel
{
    public class BaseResponseModel
    {
        public List<string> MessageList;
        public BaseResponseModel()
        {
            MessageList = new List<string>();
        }

        public string ErrorText
        {
            get
            {
                return string.Join(" ", MessageList);
            }
        }

        public ResultEnum Result { get; set; }

        public string StatusCode { get; set; }
    }
}
