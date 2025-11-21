    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.IO; // Added for CSV Export
    using System.Windows.Forms;
    using LabManagementSystem;

    namespace LabManagementSystem.Forms
    {
        public partial class StudentForm : Form
        {
            private int selectedStudentId = 0; // To store the ID of the student selected in the DataGridView

            public StudentForm()
            {
                InitializeComponent();
                this.Load += StudentForm_Load_Themed; // New themed load handler
                ThemeManager.OnThemeChanged += ThemeManager_OnThemeChanged; // Subscribe to theme changes
            }

            //private void StudentForm_Load_Themed(object sender, EventArgs e)
            //{
            //    LoadStudents();
            //    ThemeManager.ApplyTheme(this); // Apply theme on load
            //}

            private void LoadStudents(string searchTerm = "")
            {
                using (var conn = Database.GetConnection())
                {
                    string query = "SELECT StudentID, Name, RollNo, Course, Year FROM Students";
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        query +=
                            " WHERE Name LIKE @searchTerm OR RollNo LIKE @searchTerm OR Course LIKE @searchTerm OR Year LIKE @searchTerm";
                    }
                    query += " ORDER BY Name ASC"; // Default sort

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
                            dgvStudents.DataSource = dt;
                            Logger.LogInfo("Students loaded successfully.");
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("Error loading students.", ex);
                            MessageBox.Show("An error occurred loading students. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }

            private void ClearForm()
            {
                txtName.Clear();
                txtRollNo.Clear();
                txtCourse.Clear();
                txtYear.Clear();
                selectedStudentId = 0;
                btnAdd.Enabled = true;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // --- Basic Empty Validation ---
            if (!InputValidator.IsNotEmpty(txtName.Text))
            {
                MessageBox.Show("Student Name cannot be empty.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!InputValidator.IsNotEmpty(txtRollNo.Text))
            {
                MessageBox.Show("Roll Number cannot be empty.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!InputValidator.IsNotEmpty(txtCourse.Text))
            {
                MessageBox.Show("Course cannot be empty.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!InputValidator.IsNotEmpty(txtYear.Text))
            {
                MessageBox.Show("Year cannot be empty.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- Format Validation ---
            if (!InputValidator.IsAlphabetic(txtName.Text))
            {
                MessageBox.Show("Name can contain only alphabets and spaces.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!InputValidator.IsNumeric(txtRollNo.Text))
            {
                MessageBox.Show("Roll Number must contain only digits.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = Database.GetConnection())
            {
                string query = "INSERT INTO Students (Name, RollNo, Course, Year) VALUES (@name, @rollNo, @course, @year)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@rollNo", txtRollNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@course", txtCourse.Text.Trim());
                    cmd.Parameters.AddWithValue("@year", txtYear.Text.Trim());

                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Student added successfully.", "Success",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadStudents();
                        ClearForm();

                        Logger.LogInfo($"Added student: {txtName.Text} ({txtRollNo.Text})");
                    }
                    catch (SQLiteException ex)
                    {
                        if (ex.ResultCode == SQLiteErrorCode.Constraint)
                        {
                            Logger.LogWarning($"Duplicate Roll No attempted: {txtRollNo.Text}");
                            MessageBox.Show("A student with this Roll Number already exists.",
                                            "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            Logger.LogError($"Database error adding student: {txtName.Text}", ex);
                            MessageBox.Show("An error occurred. Check logs for details.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"General error adding student: {txtName.Text}", ex);
                        MessageBox.Show("Unexpected error occurred.", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedStudentId == 0)
            {
                MessageBox.Show("Please select a student to update.", "Selection Required",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // --- Basic Empty Validation ---
            if (!InputValidator.IsNotEmpty(txtName.Text) ||
    !InputValidator.IsNotEmpty(txtRollNo.Text) ||
    !InputValidator.IsNotEmpty(txtCourse.Text) ||
    !InputValidator.IsNotEmpty(txtYear.Text))
            {
                MessageBox.Show("All fields are required.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Name: alphabets only
            if (!InputValidator.IsAlphabetic(txtName.Text))
            {
                MessageBox.Show("Name must contain only alphabets.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Roll No: numbers only
            if (!InputValidator.IsNumeric(txtRollNo.Text))
            {
                MessageBox.Show("Roll Number must contain only digits.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Year: numbers only (1–5)
            if (!InputValidator.IsValidYear(txtYear.Text))
            {
                MessageBox.Show("Year must be a valid number (1 to 5).", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            using (var conn = Database.GetConnection())
            {
                string query = "UPDATE Students SET Name = @name, RollNo = @rollNo, Course = @course, Year = @year WHERE StudentID = @id";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@rollNo", txtRollNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@course", txtCourse.Text.Trim());
                    cmd.Parameters.AddWithValue("@year", txtYear.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", selectedStudentId);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Student updated successfully.", "Success",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadStudents();
                        ClearForm();

                        Logger.LogInfo($"Updated student ID {selectedStudentId}: {txtName.Text}");
                    }
                    catch (SQLiteException ex)
                    {
                        if (ex.ResultCode == SQLiteErrorCode.Constraint)
                        {
                            Logger.LogWarning($"Duplicate Roll No on update: {txtRollNo.Text}");
                            MessageBox.Show("Another student already has this Roll Number.",
                                            "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            Logger.LogError($"Database error updating student {selectedStudentId}", ex);
                            MessageBox.Show("Database error occurred. Check logs.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"General error updating student {selectedStudentId}", ex);
                        MessageBox.Show("Unexpected error occurred.", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }



        private void btnDelete_Click(object sender, EventArgs e)
            {
                if (selectedStudentId == 0)
                {
                    MessageBox.Show("Please select a student to delete from the list.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string studentInfo = $"{txtName.Text} (Roll No: {txtRollNo.Text})";
                DialogResult confirm = MessageBox.Show(
                    $"Are you sure you want to permanently delete the student '{studentInfo}'? This action cannot be undone and all associated session assignments and attendance records will also be removed.",
                    "Confirm Student Deletion",
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
                                // Delete related session assignments first
                                string deleteAssignmentsQuery = "DELETE FROM SessionAssignments WHERE StudentID = @studentId";
                                using (var cmd = new SQLiteCommand(deleteAssignmentsQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@studentId", selectedStudentId);
                                    cmd.ExecuteNonQuery();
                                    Logger.LogInfo($"Deleted {cmd.ExecuteNonQuery()} session assignments for student ID {selectedStudentId}.");
                                }

                                // Delete related attendance records
                                string deleteAttendanceQuery = "DELETE FROM Attendance WHERE StudentID = @studentId";
                                using (var cmd = new SQLiteCommand(deleteAttendanceQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@studentId", selectedStudentId);
                                    cmd.ExecuteNonQuery();
                                    Logger.LogInfo($"Deleted {cmd.ExecuteNonQuery()} attendance records for student ID {selectedStudentId}.");
                                }

                                // Then delete the student itself
                                string deleteStudentQuery = "DELETE FROM Students WHERE StudentID = @studentId";
                                using (var cmd = new SQLiteCommand(deleteStudentQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@studentId", selectedStudentId);
                                    cmd.ExecuteNonQuery();
                                }

                                transaction.Commit();
                                MessageBox.Show("Student deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Logger.LogInfo($"Deleted student ID {selectedStudentId}: {studentInfo}");
                                LoadStudents();
                                ClearForm();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                Logger.LogError($"Error deleting student ID {selectedStudentId}: {studentInfo}", ex);
                                MessageBox.Show("An error occurred while deleting the student. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }

            private void btnClear_Click(object sender, EventArgs e)
            {
                ClearForm();
            }

            private void dgvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvStudents.Rows[e.RowIndex];
                    selectedStudentId = Convert.ToInt32(row.Cells["StudentID"].Value);
                    txtName.Text = row.Cells["Name"].Value?.ToString();
                    txtRollNo.Text = row.Cells["RollNo"].Value?.ToString();
                    txtCourse.Text = row.Cells["Course"].Value?.ToString();
                    txtYear.Text = row.Cells["Year"].Value?.ToString();

                    btnAdd.Enabled = false;
                    btnUpdate.Enabled = true;
                    btnDelete.Enabled = true;
                }
            }

            private void txtSearch_TextChanged(object sender, EventArgs e)
            {
                LoadStudents(txtSearch.Text);
            }

            private void btnClearSearch_Click(object sender, EventArgs e)
            {
                txtSearch.Clear();
                LoadStudents();
            }

            private void ThemeManager_OnThemeChanged(ThemeManager.AppTheme theme)
            {
                ThemeManager.ApplyTheme(this); // Apply theme when notified of change
            }

            // Generic method to export DataGridView content to CSV
            private void btnExportCsv_Click(object sender, EventArgs e)
            {
                ExportDataGridViewToCsv(dgvStudents, "StudentsList");
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