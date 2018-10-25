using NHibernate;
using NHibernate.SqlCommand;
using System.Diagnostics;

namespace Seva.Assessment.FluentNHibernetImpl
{
    public class SqlStatementInterceptor : EmptyInterceptor
    {
        public override SqlString OnPrepareStatement(SqlString sql)
        {
            Trace.WriteLine(sql.ToString());
            return sql;
        }
    }
}
