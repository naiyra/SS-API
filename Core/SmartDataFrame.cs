using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SS_API.Core
{
    public class SmartDataFrame : IDataFrame
    {
        private readonly DataTable table;

        public SmartDataFrame(DataTable dataTable)
        {
            table = dataTable ?? throw new ArgumentNullException(nameof(dataTable));
        }

        public List<string> GetColumnNames()
        {
            return table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
        }

        public string GetColumnType(string columnName)
        {
            if (!table.Columns.Contains(columnName))
                throw new ArgumentException($"Column '{columnName}' does not exist.");

            var type = table.Columns[columnName].DataType;

            if (type == typeof(string))
                return "categorical";

            if (type == typeof(int) || type == typeof(long) ||
                type == typeof(float) || type == typeof(double) ||
                type == typeof(decimal))
                return "numeric";

            return "unknown";
        }

        public IEnumerable<object> GetColumnValues(string columnName)
        {
            if (!table.Columns.Contains(columnName))
                throw new ArgumentException($"Column '{columnName}' does not exist.");

            return table.AsEnumerable().Select(row => row[columnName]);
        }

        public void SetColumnValues(string columnName, IEnumerable<object> values)
        {
            if (!table.Columns.Contains(columnName))
                throw new ArgumentException($"Column '{columnName}' does not exist.");

            var valueList = values.ToList();
            var column = table.Columns[columnName];

            if (column.ReadOnly)
                column.ReadOnly = false;

            for (int i = 0; i < table.Rows.Count && i < valueList.Count; i++)
            {
                table.Rows[i][columnName] = valueList[i];
            }
        }

        public IDataFrame Clone()
        {
            return new SmartDataFrame(table.Copy());
        }

        public IDataFrame Take(int rowCount)
        {
            var limitedTable = table.Clone();
            int actualRows = Math.Min(rowCount, table.Rows.Count);

            for (int i = 0; i < actualRows; i++)
            {
                limitedTable.ImportRow(table.Rows[i]);
            }

            return new SmartDataFrame(limitedTable);
        }

        public List<object> GetRow(int index)
        {
            if (index < 0 || index >= table.Rows.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var row = table.Rows[index];
            return table.Columns.Cast<DataColumn>().Select(c => row[c]).ToList();
        }

        public object GetValue(int rowIndex, string columnName)
        {
            if (!table.Columns.Contains(columnName))
                throw new ArgumentException($"Column '{columnName}' does not exist.");

            if (rowIndex < 0 || rowIndex >= table.Rows.Count)
                throw new ArgumentOutOfRangeException(nameof(rowIndex));

            return table.Rows[rowIndex][columnName];
        }

        public int RowCount => table.Rows.Count;

        public DataTable GetInternalTable()
        {
            return table;
        }

        public void DropColumn(string columnName)
        {
            if (table.Columns.Contains(columnName))
            {
                table.Columns.Remove(columnName);
            }
        }

        public bool HasColumn(string columnName)
        {
            return table.Columns.Contains(columnName);
        }

        public void AddColumn(string columnName, IEnumerable<object> values)
        {
            if (table.Columns.Contains(columnName))
                throw new ArgumentException($"Column '{columnName}' already exists.");

            table.Columns.Add(columnName, typeof(object));
            var valueList = values.ToList();

            for (int i = 0; i < table.Rows.Count && i < valueList.Count; i++)
            {
                table.Rows[i][columnName] = valueList[i];
            }
        }
    }
}
