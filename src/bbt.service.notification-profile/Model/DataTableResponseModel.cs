using Notification.Profile.Enum;
using System.Data;

namespace Notification.Profile.Model
{
    public class DataTableResponseModel
    {
        public List<string> MessageList;
        public DataTableResponseModel()
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

        public DataTable DataTable;
    }
}
