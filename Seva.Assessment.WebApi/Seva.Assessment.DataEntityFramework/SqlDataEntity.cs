using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Seva.Assessment.DataEntityFramework
{
    public interface ISqlDataEntity
    {
        Dictionary<EntityCommandType,string> SqlStatements { get; set; }
        void MapReader(IDataRecord record);
        Dictionary<string, object> InsertParameters();
        Dictionary<string, object> UpdateParameters();
        Dictionary<string, object> DeleteParameters();

    }

    public class SqlDataEntity
    {
        public List<string> CriteriaColumns { get; set; }
    }
}
