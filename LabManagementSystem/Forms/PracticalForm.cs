using System;
using System.Data;
using System.Data.SQLite;
using System.IO; // Added for CSV Export
using System.Windows.Forms;
using LabManagementSystem;

namespace LabManagementSystem.Forms
{
    public partial class PracticalForm : Form
    {
        private int selectedPracticalId = 0; // To store the ID of the practical selected in the DataGridView

        public PracticalForm()
        {
            InitializeComponent();
            this.Load += PracticalForm_Load_Themed; // New themed load handler
            ThemeManager.OnThemeChanged += ThemeManager_OnThemeChanged; // Subscribe to theme changes
        }

        private void PracticalForm_Load_Themed(object sender, EventArgs e)
        {
            LoadPracticals();
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            ThemeManager.ApplyTheme(this); // Apply theme on load
        }

        private void LoadPracticals(string searchTerm = "")
        {
            using (var conn = Database.GetConnection())
            {
                string query = "SELECT PracticalID, Title, Description FROM Practicals";
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query += " WHERE Title LIKE @searchTerm OR Description LIKE @searchTerm";
                }
                query += " ORDER BY Title ASC"; // Default sort

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
                        dgvPracticals.DataSource = dt;
                        Logger.LogInfo("Practicals loaded successfully.");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error loading practicals.", ex);
                        MessageBox.Show("An error occurred loading practicals. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ClearForm()
        {
            txtTitle.Clear();
            txtDescription.Clear();
            selectedPracticalId = 0;
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Practical Title cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Practical Description cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = Database.GetConnection())
            {
                string query = "INSERT INTO Practicals (Title, Description) VALUES (@title, @description)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text);
                    cmd.Parameters.AddWithValue("@description", txtDescription.Text);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Practical added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPracticals();
                        ClearForm();
                        Logger.LogInfo($"Practical added: {txtTitle.Text}");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Error adding practical: {txtTitle.Text}", ex);
                        MessageBox.Show("An error occurred adding the practical. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedPracticalId == 0)
            {
                MessageBox.Show("Please select a practical to update from the list.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Practical Title cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Practical Description cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = Database.GetConnection())
            {
                string query = "UPDATE Practicals SET Title = @title, Description = @description WHERE PracticalID = @practicalId";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text);
                    cmd.Parameters.AddWithValue("@description", txtDescription.Text);
                    cmd.Parameters.AddWithValue("@practicalId", selectedPracticalId);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Practical updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPracticals();
                        ClearForm();
                        Logger.LogInfo($"Practical updated ID {selectedPracticalId}: {txtTitle.Text}");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Error updating practical ID {selectedPracticalId}: {txtTitle.Text}", ex);
                        MessageBox.Show("An error occurred updating the practical. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedPracticalId == 0)
            {
                MessageBox.Show("Please select a practical to delete from the list.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string practicalInfo = $"{txtTitle.Text}";
            DialogResult confirm = MessageBox.Show(
                $"Are you sure you want to permanently delete practical '{practicalInfo}'? This action cannot be undone and any lab sessions associated with this practical will also be deleted, along with their assignments and attendance.",
                "Confirm Practical Deletion",
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
                            // Get all SessionIDs linked to this practical
                            string getSessionIDsQuery = "SELECT SessionID FROM LabSessions WHERE PracticalID = @practicalId";
                            System.Collections.Generic.List<int> sessionIdsToDelete = new System.Collections.Generic.List<int>();

                            using (var sessionCmd = new SQLiteCommand(getSessionIDsQuery, conn, transaction))
                            {
                                sessionCmd.Parameters.AddWithValue("@practicalId", selectedPracticalId);
                                using (var reader = sessionCmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        sessionIdsToDelete.Add(reader.GetInt32(0));
                                    }
                                }
                            }

                            foreach (int sessionId in sessionIdsToDelete)
                            {
                                // Delete attendance for these sessions
                                string deleteAttendanceQuery = "DELETE FROM Attendance WHERE SessionID = @sessionId";
                                using (var attCmd = new SQLiteCommand(deleteAttendanceQuery, conn, transaction))
                                {
                                    attCmd.Parameters.AddWithValue("@sessionId", sessionId);
                                    attCmd.ExecuteNonQuery();
                                    Logger.LogInfo($"Deleted attendance for session ID {sessionId} related to practical ID {selectedPracticalId}.");
                                }
                                // Delete session assignments for these sessions
                                string deleteAssignmentsQuery = "DELETE FROM SessionAssignments WHERE SessionID = @sessionId";
                                using (var assignCmd = new SQLiteCommand(deleteAssignmentsQuery, conn, transaction))
                                {
                                    assignCmd.Parameters.AddWithValue("@sessionId", sessionId);
                                    assignCmd.ExecuteNonQuery();
                                    Logger.LogInfo($"Deleted session assignments for session ID {sessionId} related to practical ID {selectedPracticalId}.");
                                }
                            }

                            // Now delete the LabSessions themselves
                            string deleteSessionsQuery = "DELETE FROM LabSessions WHERE PracticalID = @practicalId";
                            using (var sessionCmd = new SQLiteCommand(deleteSessionsQuery, conn, transaction))
                            {
                                sessionCmd.Parameters.AddWithValue("@practicalId", selectedPracticalId);
                                sessionCmd.ExecuteNonQuery();
                                Logger.LogInfo($"Deleted lab sessions related to practical ID {selectedPracticalId}.");
                            }

                            // Finally, delete the practical
                            string deletePracticalQuery = "DELETE FROM Practicals WHERE PracticalID = @practicalId";
                            using (var cmd = new SQLiteCommand(deletePracticalQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@practicalId", selectedPracticalId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            MessageBox.Show("Practical deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadPracticals();
                            ClearForm();
                            Logger.LogInfo($"Deleted practical ID {selectedPracticalId}: {practicalInfo}");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Logger.LogError($"Error deleting practical ID {selectedPracticalId}: {practicalInfo}", ex);
                            MessageBox.Show("An error occurred while deleting the practical and related data. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void dgvPracticals_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPracticals.Rows[e.RowIndex];
                selectedPracticalId = Convert.ToInt32(row.Cells["PracticalID"].Value);
                txtTitle.Text = row.Cells["Title"].Value.ToString();
                txtDescription.Text = row.Cells["Description"].Value.ToString();

                btnAdd.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadPracticals(txtSearch.Text);
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadPracticals();
        }

        private void ThemeManager_OnThemeChanged(ThemeManager.AppTheme theme)
        {
            ThemeManager.ApplyTheme(this); // Apply theme when notified of change
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            ExportDataGridViewToCsv(dgvPracticals, "PracticalsList");
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