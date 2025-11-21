using System.Drawing;
using System.Windows.Forms;

namespace LabManagementSystem.Utilities
{
    /// <summary>
    /// Central place for configuring WinForms styling so every screen feels cohesive.
    /// </summary>
    public static class ThemeManager
    {
        private static readonly Color PrimaryColor = Color.FromArgb(0, 120, 215);
        private static readonly Color AccentColor = Color.FromArgb(0, 153, 188);
        private static readonly Color BackgroundColor = Color.FromArgb(245, 247, 250);
        private static readonly Color PanelColor = Color.White;
        private static readonly Color HeaderTextColor = Color.White;

        public static void ApplyTheme(Form form)
        {
            form.BackColor = BackgroundColor;
            form.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            foreach (Control control in form.Controls)
            {
                ApplyControlTheme(control);
            }
        }

        private static void ApplyControlTheme(Control control)
        {
            switch (control)
            {
                case Panel panel:
                    panel.BackColor = PanelColor;
                    break;
                case Button button:
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.BackColor = PrimaryColor;
                    button.ForeColor = Color.White;
                    button.Height = 34;
                    button.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
                    button.Cursor = Cursors.Hand;
                    break;
                case TextBox textBox:
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    break;
                case ComboBox comboBox:
                    comboBox.FlatStyle = FlatStyle.Flat;
                    break;
                case MenuStrip menuStrip:
                    menuStrip.BackColor = PrimaryColor;
                    menuStrip.ForeColor = HeaderTextColor;
                    foreach (ToolStripItem item in menuStrip.Items)
                    {
                        item.ForeColor = HeaderTextColor;
                        if (item is ToolStripMenuItem menuItem)
                        {
                            ApplyMenuTheme(menuItem);
                        }
                    }
                    break;
                case DataGridView dataGridView:
                    ApplyGridTheme(dataGridView, dataGridView.ReadOnly);
                    break;
            }

            foreach (Control child in control.Controls)
            {
                ApplyControlTheme(child);
            }
        }

        private static void ApplyMenuTheme(ToolStripMenuItem menuItem)
        {
            menuItem.BackColor = PrimaryColor;
            menuItem.ForeColor = HeaderTextColor;
            foreach (ToolStripItem subItem in menuItem.DropDownItems)
            {
                subItem.BackColor = Color.White;
                subItem.ForeColor = Color.Black;
            }
        }

        public static void ApplyGridTheme(DataGridView grid, bool? readOnlyOverride = null)
        {
            bool readOnly = readOnlyOverride ?? true;
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 241, 247);
            grid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = HeaderTextColor;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            grid.DefaultCellStyle.SelectionBackColor = AccentColor;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.RowHeadersVisible = false;
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.ReadOnly = readOnly;
            grid.EditMode = readOnly ? DataGridViewEditMode.EditProgrammatically : DataGridViewEditMode.EditOnEnter;

            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }
    }
}

