using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace LabManagementSystem.Forms
{
    public partial class AttendanceForm : Form
    {
        public AttendanceForm()
        {
            InitializeComponent();
        }

        private void AttendanceForm_Load(object sender, EventArgs e)
        {
            LoadSessionsIntoComboBox();
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

                        cmbSession.DisplayMember = "DisplayInfo"; // Custom property for display
                        cmbSession.ValueMember = "SessionID";
                        cmbSession.DataSource = dt;

                        // Add a custom column to combine info for display
                        dt.Columns.Add("DisplayInfo", typeof(string), "Date + ' ' + Time + ' - ' + PracticalTitle");

                        // Ensure that after adding the column, the DisplayMember is correctly set
                        cmbSession.DisplayMember = "DisplayInfo";

                        if (cmbSession.Items.Count > 0)
                        {
                            cmbSession.SelectedIndex = 0;
                        }
                        else
                        {
                            dgvAttendance.DataSource = null; // Clear DGV if no sessions
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading sessions for dropdown: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading attendance: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSaveAttendance_Click(object sender, EventArgs e)
        {
            if (cmbSession.SelectedValue == null)
            {
                MessageBox.Show("Please select a session first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            if (row.Cells["colAttendanceID"].Value != DBNull.Value && row.Cells["colStudentID"].Value != DBNull.Value)
                            {
                                int attendanceId = Convert.ToInt32(row.Cells["colAttendanceID"].Value);
                                int studentId = Convert.ToInt32(row.Cells["colStudentID"].Value);
                                string status = row.Cells["colStatus"].Value?.ToString();

                                string updateQuery = "UPDATE Attendance SET Status = @status WHERE AttendanceID = @attendanceId AND SessionID = @sessionId AND StudentID = @studentId";
                                using (var cmd = new SQLiteCommand(updateQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@status", status);
                                    cmd.Parameters.AddWithValue("@attendanceId", attendanceId);
                                    cmd.Parameters.AddWithValue("@sessionId", sessionId);
                                    cmd.Parameters.AddWithValue("@studentId", studentId);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        transaction.Commit();
                        MessageBox.Show("Attendance saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAttendanceForSession(sessionId); // Refresh after saving
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error saving attendance: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}