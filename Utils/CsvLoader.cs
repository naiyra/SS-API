using System.Data;
using SS_API.Core;
using System.Text;

namespace SS_API.Utils
{
    public static class CsvLoader
    {
        // Load from file path (local file)
        public static IDataFrame LoadCsv(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return LoadCsv(stream);
        }

        // Load from uploaded file stream (IFormFile.OpenReadStream)
        public static IDataFrame LoadCsv(Stream stream)
        {
            stream.Position = 0; // 🔥 Important: reset stream to start

            var table = new DataTable();

            using var reader = new StreamReader(stream, Encoding.UTF8);
            bool isFirstRow = true;
            string[] headers = null;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var values = line.Split(',');

                if (isFirstRow)
                {
                    headers = values;
                    foreach (var header in headers)
                    {
                        table.Columns.Add(header.Trim());
                    }
                    isFirstRow = false;
                }
                else
                {
                    var row = table.NewRow();
                    for (int i = 0; i < headers.Length && i < values.Length; i++)
                    {
                        row[i] = values[i].Trim(); // trim spaces
                    }
                    table.Rows.Add(row);
                }
            }

            return new SmartDataFrame(table);
        }
    }
}
