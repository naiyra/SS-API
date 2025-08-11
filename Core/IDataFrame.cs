using System.Collections.Generic;
using System.Data;

namespace SS_API.Core
{
    public interface IDataFrame
    {
        List<string> GetColumnNames();
        string GetColumnType(string columnName); // "numeric" or "categorical"
        IEnumerable<object> GetColumnValues(string columnName);
        void SetColumnValues(string columnName, IEnumerable<object> values);
        IDataFrame Clone();
        int RowCount { get; }

        List<object> GetRow(int index);
        object GetValue(int rowIndex, string columnName);

        IDataFrame Take(int rowCount);
        DataTable GetInternalTable();

        void DropColumn(string columnName);
        bool HasColumn(string columnName);
        void AddColumn(string columnName, IEnumerable<object> values);
    }
}
