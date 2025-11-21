using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using LabManagementSystem;
using LabManagementSystem;

namespace LabManagementSystem.Forms
{
    public partial class SessionForm : Form
    {
        private int selectedSessionId = 0;

        public SessionForm()
        {
            InitializeComponent();
            this.Load += SessionForm_Load_Themed; // New themed load handler
            ThemeManager.OnThemeChanged += ThemeManager_OnThemeChanged; // Subscribe to theme changes
        }

        private void SessionForm_Load_Themed(object sender, EventArgs e)
        {
            LoadPracticalsIntoComboBox();
            LoadSessions();
            LoadAvailableStudents();
            LoadAvailableComputers();
            UpdateAssignmentButtons();
            dtpDate.Value = DateTime.Today; // Set default date to today
            ThemeManager.ApplyTheme(this); // Apply theme on load
        }

        private void LoadPracticalsIntoComboBox()
        {
            using (var conn = Database.GetConnection())
            {
                string query = "SELECT PracticalID, Title FROM Practicals ORDER BY Title";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    try
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        cmbPractical.DisplayMember = "Title";
                        cmbPractical.ValueMember = "PracticalID";
                        cmbPractical.DataSource = dt;
                        Logger.LogInfo("Practicals loaded into combo box for sessions.");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error loading practicals for session dropdown.", ex);
                        MessageBox.Show("An error occurred loading practicals. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadSessions()
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
                    ORDER BY LS.Date DESC, LS.Time DESC"; // Default sort
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    try
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvSessions.DataSource = dt;
                        dgvSessions.Columns["SessionID"].Visible = false; // Hide ID column
                        Logger.LogInfo("Lab sessions loaded successfully.");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error loading lab sessions.", ex);
                        MessageBox.Show("An error occurred loading lab sessions. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadAvailableStudents()
        {
            using (var conn = Database.GetConnection())
            {
                // Select students not yet assigned to the current selected session
                string query = @"
                    SELECT
                        S.StudentID,
                        S.Name,
                        S.RollNo
                    FROM Students S
                    WHERE S.StudentID NOT IN (
                        SELECT SA.StudentID FROM SessionAssignments SA WHERE SA.SessionID = @sessionId
                    )
                    ORDER BY S.Name";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@sessionId", selectedSessionId);
                    try
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvAvailableStudents.DataSource = dt;
                        dgvAvailableStudents.Columns["StudentID"].Visible = false;
                        Logger.LogInfo("Available students loaded for current session.");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error loading available students for current session.", ex);
                        MessageBox.Show("An error occurred loading available students. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadAvailableComputers()
        {
            using (var conn = Database.GetConnection())
            {
                // Select computers not yet assigned to the current selected session
                string query = @"
                    SELECT
                        C.ComputerID,
                        C.SystemNo,
                        C.Status
                    FROM Computers C
                    WHERE C.ComputerID NOT IN (
                        SELECT SA.ComputerID FROM SessionAssignments SA WHERE SA.SessionID = @sessionId AND SA.ComputerID IS NOT NULL
                    ) AND C.Status = 'Working' -- Only show working computers
                    ORDER BY C.SystemNo";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@sessionId", selectedSessionId);
                    try
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvAvailableComputers.DataSource = dt;
                        dgvAvailableComputers.Columns["ComputerID"].Visible = false;
                        Logger.LogInfo("Available computers loaded for current session.");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error loading available computers for current session.", ex);
                        MessageBox.Show("An error occurred loading available computers. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadSessionAssignments()
        {
            using (var conn = Database.GetConnection())
            {
                string query = @"
                    SELECT
                        SA.AssignID,
                        S.Name AS StudentName,
                        S.RollNo,
                        C.SystemNo AS ComputerSystemNo
                    FROM SessionAssignments SA
                    JOIN Students S ON SA.StudentID = S.StudentID
                    LEFT JOIN Computers C ON SA.ComputerID = C.ComputerID
                    WHERE SA.SessionID = @sessionId
                    ORDER BY S.Name";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@sessionId", selectedSessionId);
                    try
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvSessionAssignments.DataSource = dt;
                        dgvSessionAssignments.Columns["AssignID"].Visible = false;
                        Logger.LogInfo("Session assignments loaded for current session.");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error loading session assignments for current session.", ex);
                        MessageBox.Show("An error occurred loading session assignments. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ClearSessionForm()
        {
            dtpDate.Value = DateTime.Today;
            txtTime.Text = "HH:MM (e.g., 10:00)";
            if (cmbPractical.Items.Count > 0)
            {
                cmbPractical.SelectedIndex = 0;
            }
            selectedSessionId = 0;
            btnAddSession.Enabled = true;
            btnDeleteSession.Enabled = false;

            dgvAvailableStudents.DataSource = null;
            dgvAvailableComputers.DataSource = null;
            dgvSessionAssignments.DataSource = null;
            UpdateAssignmentButtons();
        }

        private void UpdateAssignmentButtons()
        {
            bool sessionSelected = selectedSessionId > 0;
            btnAssignStudent.Enabled = sessionSelected && (dgvAvailableStudents.Rows.Count > 0);
            btnAssignComputer.Enabled = sessionSelected && (dgvAvailableComputers.Rows.Count > 0);
        }

        private void btnAddSession_Click(object sender, EventArgs e)
        {
            if (cmbPractical.SelectedValue == null)
            {
                MessageBox.Show("Please select a practical.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Logger.LogWarning("Attempted to add session without selecting a practical.");
                return;
            }

            if (!TimeSpan.TryParse(txtTime.Text, out TimeSpan sessionTime))
            {
                MessageBox.Show("Please enter a valid time in HH:MM format (e.g., 10:00).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Logger.LogWarning($"Attempted to add session with invalid time format: {txtTime.Text}");
                return;
            }

            using (var conn = Database.GetConnection())
            {
                string query = "INSERT INTO LabSessions (Date, Time, PracticalID) VALUES (@date, @time, @practicalId)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@date", dtpDate.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@time", txtTime.Text);
                    cmd.Parameters.AddWithValue("@practicalId", Convert.ToInt32(cmbPractical.SelectedValue));
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Lab session added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadSessions();
                        ClearSessionForm(); // Clear form and reset selected session
                        Logger.LogInfo($"Lab session added for {dtpDate.Value.ToShortDateString()} at {txtTime.Text}. Practical: {cmbPractical.Text}");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Error adding lab session: {dtpDate.Value.ToShortDateString()} at {txtTime.Text}.", ex);
                        MessageBox.Show("An error occurred adding the lab session. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDeleteSession_Click(object sender, EventArgs e)
        {
            if (selectedSessionId == 0)
            {
                MessageBox.Show("Please select a session to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Logger.LogWarning("Attempted to delete session without selection.");
                return;
            }

            string sessionInfo = $"{dtpDate.Value.ToShortDateString()} {txtTime.Text} - {cmbPractical.Text}";
            DialogResult confirm = MessageBox.Show(
                $"Deleting the session '{sessionInfo}' will permanently delete all its associated student and computer assignments, and attendance records. This action cannot be undone. Are you sure you want to proceed?",
                "Confirm Session Deletion",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                using (var conn = Database.GetConnection())
                {
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Delete related attendance records first
                            string deleteAttendanceQuery = "DELETE FROM Attendance WHERE SessionID = @sessionId";
                            using (var cmd = new SQLiteCommand(deleteAttendanceQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@sessionId", selectedSessionId);
                                cmd.ExecuteNonQuery();
                                Logger.LogInfo($"Deleted attendance for session ID {selectedSessionId}.");
                            }

                            // Delete related session assignments
                            string deleteAssignmentsQuery = "DELETE FROM SessionAssignments WHERE SessionID = @sessionId";
                            using (var cmd = new SQLiteCommand(deleteAssignmentsQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@sessionId", selectedSessionId);
                                cmd.ExecuteNonQuery();
                                Logger.LogInfo($"Deleted session assignments for session ID {selectedSessionId}.");
                            }

                            // Delete the session itself
                            string deleteSessionQuery = "DELETE FROM LabSessions WHERE SessionID = @sessionId";
                            using (var cmd = new SQLiteCommand(deleteSessionQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@sessionId", selectedSessionId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            MessageBox.Show("Lab session and all related data deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Logger.LogInfo($"Lab session ID {selectedSessionId} and all related data deleted.");
                            LoadSessions();
                            ClearSessionForm();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Logger.LogError($"Error deleting lab session ID {selectedSessionId}.", ex);
                            MessageBox.Show("An error occurred deleting the lab session. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void btnClearSession_Click(object sender, EventArgs e)
        {
            ClearSessionForm();
        }

        private void dgvSessions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSessions.Rows[e.RowIndex];
                selectedSessionId = Convert.ToInt32(row.Cells["SessionID"].Value);

                dtpDate.Value = Convert.ToDateTime(row.Cells["Date"].Value);
                txtTime.Text = row.Cells["Time"].Value.ToString();
                cmbPractical.Text = row.Cells["PracticalTitle"].Value.ToString();

                btnAddSession.Enabled = false;
                btnDeleteSession.Enabled = true;

                LoadAvailableStudents();
                LoadAvailableComputers();
                LoadSessionAssignments();
                UpdateAssignmentButtons();
            }
        }

        private void btnAssignStudent_Click(object sender, EventArgs e)
        {
            if (selectedSessionId == 0)
            {
                MessageBox.Show("Please select a lab session first to assign a student.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Logger.LogWarning("Attempted to assign student to session without selecting a session.");
                return;
            }

            if (dgvAvailableStudents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student from the 'Available Students' list to assign.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Logger.LogWarning("Attempted to assign student to session without selecting a student.");
                return;
            }

            int studentId = Convert.ToInt32(dgvAvailableStudents.SelectedRows[0].Cells["StudentID"].Value);

            using (var conn = Database.GetConnection())
            {
                // Assign Student. ComputerID can be NULL initially.
                string query = "INSERT INTO SessionAssignments (SessionID, StudentID, ComputerID) VALUES (@sessionId, @studentId, NULL)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@sessionId", selectedSessionId);
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Student assigned to session successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAvailableStudents();
                        LoadSessionAssignments();
                        // Also add an entry to Attendance for this student in this session, default to Absent
                        InsertOrUpdateAttendance(selectedSessionId, studentId, "Absent");
                        Logger.LogInfo($"Student ID {studentId} assigned to session ID {selectedSessionId}.");
                    }
                    catch (SQLiteException ex)
                    {
                        if (ex.ResultCode == SQLiteErrorCode.Constraint)
                        {
                            Logger.LogWarning($"Attempted to assign student ID {studentId} to session ID {selectedSessionId} multiple times.");
                            MessageBox.Show("This student is already assigned to this session.", "Duplicate Assignment", MessageBoxButtons.OK, MessageBoxIcon.Information); // Changed to Info
                        }
                        else
                        {
                            Logger.LogError($"Error assigning student ID {studentId} to session ID {selectedSessionId}.", ex);
                            MessageBox.Show("An error occurred assigning the student. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"General error assigning student ID {studentId} to session ID {selectedSessionId}.", ex);
                        MessageBox.Show("An unexpected error occurred assigning the student. See log for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnAssignComputer_Click(object sender, EventArgs e)
        {
            if (selectedSessionId == 0)
            {
                MessageBox.Show("Please select a lab session first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Logger.LogWarning("Attempted to assign computer without selecting a session.");
                return;
            }

            if (dgvSessionAssignments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student assignment from 'Current Assignments' to assign a computer.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Logger.LogWarning("Attempted to assign computer to assignment without selecting an assignment.");
                return;
            }

            if (dgvAvailableComputers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an available computer from the list.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Logger.LogWarning("Attempted to assign computer without selecting an available computer.");
                return;
            }

            int assignId = Convert.ToInt32(dgvSessionAssignments.SelectedRows[0].Cells["AssignID"].Value);
            int computerId = Convert.ToInt32(dgvAvailableComputers.SelectedRows[0].Cells["ComputerID"].Value);

            using (var conn = Database.GetConnection())
            {
                // Check if the selected computer is already assigned to another student in this session (should be prevented by LoadAvailableComputers, but double-check)
                string checkComputerAssignment = "SELECT COUNT(1) FROM SessionAssignments WHERE SessionID = @sessionId AND ComputerID = @computerId AND AssignID != @assignId";
                using (var checkCmd = new SQLiteCommand(checkComputerAssignment, conn))
                {
                    checkCmd.Parameters.AddWithValue("@sessionId", selectedSessionId);
                    checkCmd.Parameters.AddWithValue("@computerId", computerId);
                    checkCmd.Parameters.AddWithValue("@assignId", assignId);
                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                    {
                        MessageBox.Show("This computer is already assigned to another student in this session. Please select a different computer.", "Duplicate Assignment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Logger.LogWarning($"Attempted to assign computer ID {computerId} to assignment ID {assignId} but it's already in use in session {selectedSessionId}.");
                        return;
                    }
                }

                string query = "UPDATE SessionAssignments SET ComputerID = @computerId WHERE AssignID = @assignId";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@computerId", computerId);
                    cmd.Parameters.AddWithValue("@assignId", assignId);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Computer assigned successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAvailableComputers();
                        LoadSessionAssignments();
                        Logger.LogInfo($"Computer ID {computerId} assigned to assignment ID {assignId} in session ID {selectedSessionId}.");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Error assigning computer ID {computerId} to assignment ID {assignId}.", ex);
                        MessageBox.Show("An error occurred assigning the computer. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Helper method to insert/update attendance when a student is assigned
        private void InsertOrUpdateAttendance(int sessionId, int studentId, string status)
        {
            using (var conn = Database.GetConnection())
            {
                string checkQuery = "SELECT COUNT(1) FROM Attendance WHERE SessionID = @sessionId AND StudentID = @studentId";
                using (var checkCmd = new SQLiteCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@sessionId", sessionId);
                    checkCmd.Parameters.AddWithValue("@studentId", studentId);
                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) == 0)
                    {
                        // Only insert if it doesn't exist
                        string insertQuery = "INSERT INTO Attendance (SessionID, StudentID, Status) VALUES (@sessionId, @studentId, @status)";
                        using (var insertCmd = new SQLiteCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@sessionId", sessionId);
                            insertCmd.Parameters.AddWithValue("@studentId", studentId);
                            insertCmd.Parameters.AddWithValue("@status", status);
                            insertCmd.ExecuteNonQuery();
                            Logger.LogInfo($"Initial attendance record added for student ID {studentId} in session ID {sessionId} with status '{status}'.");
                        }
                    }
                    // If it exists, we don't need to update it here, as AttendanceForm will manage updates
                }
            }
        }

        private void txtTime_Enter(object sender, EventArgs e)
        {
            if (txtTime.Text == "HH:MM (e.g., 10:00)")
            {
                txtTime.Text = "";
                txtTime.ForeColor = System.Drawing.SystemColors.WindowText;
            }
        }

        private void txtTime_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTime.Text))
            {
                txtTime.Text = "HH:MM (e.g., 10:00)";
                txtTime.ForeColor = System.Drawing.SystemColors.GrayText;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtTime.Text, @"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$"))
            {
                MessageBox.Show("Time must be in HH:MM format (e.g., 10:00 or 14:30).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTime.Focus();
                Logger.LogWarning($"Invalid time format entered in SessionForm: {txtTime.Text}");
            }
        }

        private void ThemeManager_OnThemeChanged(ThemeManager.AppTheme theme)
        {
            ThemeManager.ApplyTheme(this); // Apply theme when notified of change
        }
    }
}