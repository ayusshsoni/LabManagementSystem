using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace LabManagementSystem.Forms
{
    public partial class StudentForm : Form
    {
        private int selectedStudentId = 0; // To store the ID of the student selected in the DataGridView

        public StudentForm()
        {
            InitializeComponent();
        }

        private void StudentForm_Load(object sender, EventArgs e)
        {
            LoadStudents();
        }

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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading students: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtRollNo.Text) ||
                string.IsNullOrWhiteSpace(txtCourse.Text) || string.IsNullOrWhiteSpace(txtYear.Text))
            {
                MessageBox.Show("Please fill all student details.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = Database.GetConnection())
            {
                string query = "INSERT INTO Students (Name, RollNo, Course, Year) VALUES (@name, @rollNo, @course, @year)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@rollNo", txtRollNo.Text);
                    cmd.Parameters.AddWithValue("@course", txtCourse.Text);
                    cmd.Parameters.AddWithValue("@year", txtYear.Text);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Student added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadStudents();
                        ClearForm();
                    }
                    catch (SQLiteException ex)
                    {
                        if (ex.ResultCode == SQLiteErrorCode.Constraint)
                        {
                            MessageBox.Show("A student with this Roll Number already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Error adding student: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding student: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedStudentId == 0)
            {
                MessageBox.Show("Please select a student to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtRollNo.Text) ||
                string.IsNullOrWhiteSpace(txtCourse.Text) || string.IsNullOrWhiteSpace(txtYear.Text))
            {
                MessageBox.Show("Please fill all student details.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = Database.GetConnection())
            {
                string query = "UPDATE Students SET Name = @name, RollNo = @rollNo, Course = @course, Year = @year WHERE StudentID = @studentId";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@rollNo", txtRollNo.Text);
                    cmd.Parameters.AddWithValue("@course", txtCourse.Text);
                    cmd.Parameters.AddWithValue("@year", txtYear.Text);
                    cmd.Parameters.AddWithValue("@studentId", selectedStudentId);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Student updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadStudents();
                        ClearForm();
                    }
                    catch (SQLiteException ex)
                    {
                        if (ex.ResultCode == SQLiteErrorCode.Constraint)
                        {
                            MessageBox.Show("Another student with this Roll Number already exists. Roll numbers must be unique.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Error updating student: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating student: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedStudentId == 0)
            {
                MessageBox.Show("Please select a student to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show($"Are you sure you want to delete student '{txtName.Text}' (Roll No: {txtRollNo.Text})?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                using (var conn = Database.GetConnection())
                {
                    string query = "DELETE FROM Students WHERE StudentID = @studentId";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentId", selectedStudentId);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Student deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadStudents();
                            ClearForm();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error deleting student: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                txtName.Text = row.Cells["Name"].Value.ToString();
                txtRollNo.Text = row.Cells["RollNo"].Value.ToString();
                txtCourse.Text = row.Cells["Course"].Value.ToString();
                txtYear.Text = row.Cells["Year"].Value.ToString();

                btnAdd.Enabled = false;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadStudents(txtSearch.Text);
        }
    }
}