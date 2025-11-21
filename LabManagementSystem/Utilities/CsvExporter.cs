using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LabManagementSystem.Utilities
{
    public static class CsvExporter
    {
        public static bool TryExport(DataGridView grid, string filePath)
        {
            if (grid.DataSource == null || grid.Rows.Count == 0)
            {
                MessageBox.Show("There is no data to export yet.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            var builder = new StringBuilder();
            var visibleColumns = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn column in grid.Columns)
            {
                if (column.Visible)
                {
                    visibleColumns.Add(column);
                }
            }

            for (int i = 0; i < visibleColumns.Count; i++)
            {
                builder.Append(visibleColumns[i].HeaderText);
                builder.Append(i == visibleColumns.Count - 1 ? '\n' : ',');
            }

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;

                for (int i = 0; i < visibleColumns.Count; i++)
                {
                    var column = visibleColumns[i];
                    string cellValue = row.Cells[column.Index].Value?.ToString()?.Replace("\"", "\"\"") ?? string.Empty;
                    builder.Append($"\"{cellValue}\"");
                    builder.Append(i == visibleColumns.Count - 1 ? '\n' : ',');
                }
            }

            File.WriteAllText(filePath, builder.ToString(), Encoding.UTF8);
            MessageBox.Show($"Exported to {filePath}", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }
    }
}

