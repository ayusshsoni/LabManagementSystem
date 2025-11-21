using System;
using System.Data;
using System.Data.SQLite;
using System.IO; // Added for CSV Export
using System.Windows.Forms;
using LabManagementSystem;
namespace LabManagementSystem.Forms
{
    public partial class ComputerForm : Form
    {
        private int selectedComputerId = 0; // To store the ID of the computer selected in the DataGridView

        public ComputerForm()
        {
            InitializeComponent();
            this.Load += ComputerForm_Load_Themed; // New themed load handler
            ThemeManager.OnThemeChanged += ThemeManager_OnThemeChanged; // Subscribe to theme changes
        }

        private void ComputerForm_Load_Themed(object sender, EventArgs e)
        {
            LoadComputers();
            cmbStatus.SelectedIndex = 0; // Set default status to "Working"
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            ThemeManager.ApplyTheme(this); // Apply theme on load
        }

        private void LoadComputers(string searchTerm = "")
        {
            using (var conn = Database.GetConnection())
            {
                string query = "SELECT ComputerID, SystemNo, Configuration, Status FROM Computers";
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query +=
                        " WHERE SystemNo LIKE @searchTerm OR Configuration LIKE @searchTerm OR Status LIKE @searchTerm";
                }
                query += " ORDER BY SystemNo ASC"; // Default sort
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");
                    }
                    try
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvComputers.DataSource = dt;
                        Logger.LogInfo("Computers loaded successfully.");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error loading computers.", ex);
                        MessageBox.Show("An error occurred loading computers. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ClearForm()
        {
            txtSystemNo.Clear();
            txtConfiguration.Clear();
            cmbStatus.SelectedIndex = 0;
            selectedComputerId = 0;
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSystemNo.Text))
            {
                MessageBox.Show("System Number cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtConfiguration.Text))
            {
                MessageBox.Show("Configuration cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a status for the computer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            using (var conn = Database.GetConnection())
            {
                string query = "INSERT INTO Computers (SystemNo, Configuration, Status) VALUES (@systemNo, @configuration, @status)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@systemNo", txtSystemNo.Text);
                    cmd.Parameters.AddWithValue("@configuration", txtConfiguration.Text);
                    cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedItem.ToString());
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Computer added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadComputers();
                        ClearForm();
                        Logger.LogInfo($"Computer added: {txtSystemNo.Text}");
                    }
                    catch (SQLiteException ex)
                    {
                        if (ex.ResultCode == SQLiteErrorCode.Constraint)
                        {
                            Logger.LogWarning($"Attempted to add computer with duplicate System No: {txtSystemNo.Text}");
                            MessageBox.Show("A computer with this System Number already exists. System numbers must be unique.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            Logger.LogError($"Error adding computer: {txtSystemNo.Text}", ex);
                            MessageBox.Show("An error occurred adding the computer. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"General error adding computer: {txtSystemNo.Text}", ex);
                        MessageBox.Show("An unexpected error occurred adding the computer. See log for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedComputerId == 0)
            {
                MessageBox.Show("Please select a computer to update from the list.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSystemNo.Text))
            {
                MessageBox.Show("System Number cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtConfiguration.Text))
            {
                MessageBox.Show("Configuration cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a status for the computer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = Database.GetConnection())
            {
                string query = "UPDATE Computers SET SystemNo = @systemNo, Configuration = @configuration, Status = @status WHERE ComputerID = @computerId";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@systemNo", txtSystemNo.Text);
                    cmd.Parameters.AddWithValue("@configuration", txtConfiguration.Text);
                    cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@computerId", selectedComputerId);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Computer updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadComputers();
                        ClearForm();
                        Logger.LogInfo($"Computer updated ID {selectedComputerId}: {txtSystemNo.Text}");
                    }
                    catch (SQLiteException ex)
                    {
                        if (ex.ResultCode == SQLiteErrorCode.Constraint)
                        {
                            Logger.LogWarning($"Attempted to update computer ID {selectedComputerId} with duplicate System No: {txtSystemNo.Text}");
                            MessageBox.Show("Another computer with this System Number already exists. System numbers must be unique.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            Logger.LogError($"Error updating computer ID {selectedComputerId}: {txtSystemNo.Text}", ex);
                            MessageBox.Show("An error occurred updating the computer. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"General error updating computer ID {selectedComputerId}: {txtSystemNo.Text}", ex);
                        MessageBox.Show("An unexpected error occurred updating the computer. See log for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedComputerId == 0)
            {
                MessageBox.Show("Please select a computer to delete from the list.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string computerInfo = $"{txtSystemNo.Text}";
            DialogResult confirm = MessageBox.Show(
                $"Are you sure you want to permanently delete computer '{computerInfo}'? This action cannot be undone. Any existing session assignments for this computer will have their computer removed, but not the student assignment itself.",
                "Confirm Computer Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm == DialogResult.Yes)
            {
                using (var conn = Database.GetConnection())
                {
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // First, remove any session assignments referring to this computer
                            // We are setting ComputerID to NULL to maintain student assignment to session.
                            // If you wanted to delete the entire SessionAssignment record, use DELETE FROM...
                            string deleteAssignmentsQuery = "UPDATE SessionAssignments SET ComputerID = NULL WHERE ComputerID = @computerId";
                            using (var cmd = new SQLiteCommand(deleteAssignmentsQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@computerId", selectedComputerId);
                                cmd.ExecuteNonQuery();
                                Logger.LogInfo($"Cleared computer assignment for {cmd.ExecuteNonQuery()} session assignments due to deletion of computer ID {selectedComputerId}.");
                            }

                            // Then delete the computer itself
                            string deleteComputerQuery = "DELETE FROM Computers WHERE ComputerID = @computerId";
                            using (var cmd = new SQLiteCommand(deleteComputerQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@computerId", selectedComputerId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            MessageBox.Show("Computer deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Logger.LogInfo($"Deleted computer ID {selectedComputerId}: {computerInfo}");
                            LoadComputers();
                            ClearForm();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Logger.LogError($"Error deleting computer ID {selectedComputerId}: {computerInfo}", ex);
                            MessageBox.Show("An error occurred while deleting the computer. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void dgvComputers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvComputers.Rows[e.RowIndex];
                selectedComputerId = Convert.ToInt32(row.Cells["ComputerID"].Value);
                txtSystemNo.Text = row.Cells["SystemNo"].Value.ToString();
                txtConfiguration.Text = row.Cells["Configuration"].Value.ToString();
                cmbStatus.SelectedItem = row.Cells["Status"].Value.ToString();

                btnAdd.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadComputers(txtSearch.Text);
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadComputers();
        }

        private void ThemeManager_OnThemeChanged(ThemeManager.AppTheme theme)
        {
            ThemeManager.ApplyTheme(this); // Apply theme when notified of change
        }

        // Generic method to export DataGridView content to CSV
        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            ExportDataGridViewToCsv(dgvComputers, "ComputersList");
        }

        private void ExportDataGridViewToCsv(DataGridView dgv, string defaultFileName)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";
            sfd.FileName = $"{defaultFileName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(sfd.FileName))
                    {
                        // Write headers
                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            if (dgv.Columns[i].Visible) // Only export visible columns
                            {
                                writer.Write((i > 0 ? "," : "") + $"\"{dgv.Columns[i].HeaderText}\"");
                            }
                        }
                        writer.WriteLine();

                        // Write data rows
                        for (int i = 0; i < dgv.Rows.Count; i++)
                        {
                            for (int j = 0; j < dgv.Columns.Count; j++)
                            {
                                if (dgv.Columns[j].Visible)
                                {
                                    writer.Write((j > 0 ? "," : "") + $"\"{dgv.Rows[i].Cells[j].Value?.ToString().Replace("\"", "\"\"")}\"");
                                }
                            }
                            writer.WriteLine();
                        }
                    }
                    MessageBox.Show("Data exported successfully to CSV!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Logger.LogInfo($"Data exported to CSV: {sfd.FileName}");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error exporting data to CSV: {sfd.FileName}", ex);
                    MessageBox.Show($"Error exporting data: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}