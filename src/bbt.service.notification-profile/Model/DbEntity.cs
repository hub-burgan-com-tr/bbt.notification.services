using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace notification_profile.Model
{
    public class DbDataEntity
    {
        public ParameterDirection direction { get; set; }
        public DbType dbType { get; set; }
        public string parameterName { get; set; }
        public object value;
    }
    public class DbEntity
    {

    }
}