using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace LabManagementSystem.Forms
{
    public partial class SessionForm : Form
    {
        private int selectedSessionId = 0;

        public SessionForm()
        {
            InitializeComponent();
        }

        private void SessionForm_Load(object sender, EventArgs e)
        {
            LoadPracticalsIntoComboBox();
            LoadSessions();
            LoadAvailableStudents();
            LoadAvailableComputers();
            UpdateAssignmentButtons();
            dtpDate.Value = DateTime.Today; // Set default date to today
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading practicals for dropdown: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    ORDER BY LS.Date DESC, LS.Time DESC";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    try
                    {
                        SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvSessions.DataSource = dt;
                        dgvSessions.Columns["SessionID"].Visible = false; // Hide ID column
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading lab sessions: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading available students: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading available computers: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading session assignments: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                return;
            }

            if (!TimeSpan.TryParse(txtTime.Text, out TimeSpan sessionTime))
            {
                MessageBox.Show("Please enter a valid time in HH:MM format (e.g., 10:00).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding lab session: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDeleteSession_Click(object sender, EventArgs e)
        {
            if (selectedSessionId == 0)
            {
                MessageBox.Show("Please select a session to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show("Deleting a session will also delete all its student and computer assignments. Are you sure?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                using (var conn = Database.GetConnection())
                {
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Delete related attendance records first (if any)
                            string deleteAttendanceQuery = "DELETE FROM Attendance WHERE SessionID = @sessionId";
                            using (var cmd = new SQLiteCommand(deleteAttendanceQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@sessionId", selectedSessionId);
                                cmd.ExecuteNonQuery();
                            }

                            // Delete related session assignments
                            string deleteAssignmentsQuery = "DELETE FROM SessionAssignments WHERE SessionID = @sessionId";
                            using (var cmd = new SQLiteCommand(deleteAssignmentsQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@sessionId", selectedSessionId);
                                cmd.ExecuteNonQuery();
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
                            LoadSessions();
                            ClearSessionForm();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Error deleting lab session: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Please select a lab session first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvAvailableStudents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student from the available list.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        MessageBox.Show("Student assigned to session.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAvailableStudents();
                        LoadSessionAssignments();
                        // Also add an entry to Attendance for this student in this session, default to Absent
                        InsertOrUpdateAttendance(selectedSessionId, studentId, "Absent");
                    }
                    catch (SQLiteException ex)
                    {
                        if (ex.ResultCode == SQLiteErrorCode.Constraint)
                        {
                            MessageBox.Show("This student is already assigned to this session.", "Duplicate Assignment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Error assigning student: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error assigning student: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnAssignComputer_Click(object sender, EventArgs e)
        {
            if (selectedSessionId == 0)
            {
                MessageBox.Show("Please select a lab session first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvSessionAssignments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student assignment from 'Current Assignments' to assign a computer.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvAvailableComputers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an available computer.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int assignId = Convert.ToInt32(dgvSessionAssignments.SelectedRows[0].Cells["AssignID"].Value);
            int computerId = Convert.ToInt32(dgvAvailableComputers.SelectedRows[0].Cells["ComputerID"].Value);

            using (var conn = Database.GetConnection())
            {
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error assigning computer: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        }
    }
}