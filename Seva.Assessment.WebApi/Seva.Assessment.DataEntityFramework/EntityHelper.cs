using System;
using System.Collections.Generic;
using System.Text;

namespace Seva.Assessment.DataEntityFramework
{
    public enum EntityCommandType
    {
        INSERT = 1, UPDATE = 2, DELETE = 3, SELECT = 4, SELECTALL = 5
    }
    public static class EntityHelper
    {
        public static string ToText(this object o)
        {
            if (o == null || o == DBNull.Value)
                return null;
            else
                return o.ToString();
        }

        public static DateTime ToDateTime(this object o)
        {
            if (o == null || o == DBNull.Value)
                return new DateTime();
            else
                return Convert.ToDateTime(o);
        }

        public static DateTime? ToDateTime(this object o, bool isNullable)
        {
            if (o == null || o == DBNull.Value)
                return null;
            else
                return Convert.ToDateTime(o);
        }
    }

}
