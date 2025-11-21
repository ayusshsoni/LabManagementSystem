using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using LabManagementSystem;

namespace LabManagementSystem.Forms
{
    public partial class AttendanceForm : Form
    {
        public AttendanceForm()
        {
            InitializeComponent();
            this.Load += AttendanceForm_Load_Themed; // New themed load handler
            ThemeManager.OnThemeChanged += ThemeManager_OnThemeChanged; // Subscribe to theme changes
        }

        private void AttendanceForm_Load_Themed(object sender, EventArgs e)
        {
            LoadSessionsIntoComboBox();
            ThemeManager.ApplyTheme(this); // Apply theme on load
        }

        private void LoadSessionsIntoComboBox()
        {
            using (var conn = Database.GetConnection())
            {
                string query = @"
                    SELECT
                        LS.SessionID,
                        LS.Date,
                        LS.Time,
                        P.Title AS PracticalTitle
                    FROM LabSessions LS
                    JOIN Practicals P ON LS.PracticalID = P.PracticalID
                    ORDER BY LS.Date DESC, LS.Time DESC";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    try
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Add a custom column to combine info for display BEFORE setting DataSource
                        dt.Columns.Add("DisplayInfo", typeof(string), "Date + ' ' + Time + ' - ' + PracticalTitle");

                        cmbSession.DisplayMember = "DisplayInfo"; // Custom property for display
                        cmbSession.ValueMember = "SessionID";
                        cmbSession.DataSource = dt;

                        if (cmbSession.Items.Count > 0)
                        {
                            cmbSession.SelectedIndex = 0;
                        }
                        else
                        {
                            dgvAttendance.DataSource = null; // Clear DGV if no sessions
                        }
                        Logger.LogInfo("Sessions loaded into combo box for attendance.");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error loading sessions for attendance dropdown.", ex);
                        MessageBox.Show("An error occurred loading sessions. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void cmbSession_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSession.SelectedValue != null)
            {
                int sessionId = Convert.ToInt32(cmbSession.SelectedValue);
                LoadAttendanceForSession(sessionId);
            }
            else
            {
                dgvAttendance.DataSource = null; // Clear DGV if no session selected
            }
        }

        private void LoadAttendanceForSession(int sessionId)
        {
            using (var conn = Database.GetConnection())
            {
                string query = @"
                    SELECT
                        A.AttendanceID,
                        A.StudentID,
                        S.Name,
                        S.RollNo,
                        A.Status
                    FROM Attendance A
                    JOIN Students S ON A.StudentID = S.StudentID
                    WHERE A.SessionID = @sessionId
                    ORDER BY S.Name";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@sessionId", sessionId);
                    try
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvAttendance.DataSource = dt;
                        Logger.LogInfo($"Attendance loaded for session ID: {sessionId}");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Error loading attendance for session ID: {sessionId}", ex);
                        MessageBox.Show("An error occurred loading attendance. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSaveAttendance_Click(object sender, EventArgs e)
        {
            if (cmbSession.SelectedValue == null)
            {
                MessageBox.Show("Please select a session first to save attendance.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int sessionId = Convert.ToInt32(cmbSession.SelectedValue);

            using (var conn = Database.GetConnection())
            {
                using (SQLiteTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (DataGridViewRow row in dgvAttendance.Rows)
                        {
                            // Ensure it's not a new row for adding, and cells are not null
                            if (!row.IsNewRow && row.Cells["colAttendanceID"].Value != DBNull.Value && row.Cells["colStudentID"].Value != DBNull.Value)
                            {
                                int attendanceId = Convert.ToInt32(row.Cells["colAttendanceID"].Value);
                                //int studentId = Convert.ToInt32(row.Cells["colStudentID"].Value); // Not strictly needed for update query
                                string status = row.Cells["colStatus"].Value?.ToString();

                                if (string.IsNullOrWhiteSpace(status))
                                {
                                    MessageBox.Show($"Attendance status for student '{row.Cells["colName"].Value}' cannot be empty. Skipping this student.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    Logger.LogWarning($"Skipped saving attendance for student {row.Cells["colName"].Value} in session {sessionId} due to empty status.");
                                    continue; // Skip this row and continue with others
                                }

                                string updateQuery = "UPDATE Attendance SET Status = @status WHERE AttendanceID = @attendanceId"; // SessionID and StudentID are unique, but we can update by ID
                                using (var cmd = new SQLiteCommand(updateQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@status", status);
                                    cmd.Parameters.AddWithValue("@attendanceId", attendanceId);
                                    //cmd.Parameters.AddWithValue("@sessionId", sessionId); // Not needed if using AttendanceID
                                    //cmd.Parameters.AddWithValue("@studentId", studentId); // Not needed if using AttendanceID
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        transaction.Commit();
                        MessageBox.Show("Attendance saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAttendanceForSession(sessionId); // Refresh after saving
                        Logger.LogInfo($"Attendance saved for session ID: {sessionId}");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Logger.LogError($"Error saving attendance for session ID: {sessionId}", ex);
                        MessageBox.Show("An error occurred saving attendance. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ThemeManager_OnThemeChanged(ThemeManager.AppTheme theme)
        {
            ThemeManager.ApplyTheme(this); // Apply theme when notified of change
        }
    }
}