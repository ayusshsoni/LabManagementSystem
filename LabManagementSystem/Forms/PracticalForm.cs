using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace LabManagementSystem.Forms
{
    public partial class PracticalForm : Form
    {
        private int selectedPracticalId = 0; // To store the ID of the practical selected in the DataGridView

        public PracticalForm()
        {
            InitializeComponent();
        }

        private void PracticalForm_Load(object sender, EventArgs e)
        {
            LoadPracticals();
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading practicals: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please fill both title and description.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding practical: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedPracticalId == 0)
            {
                MessageBox.Show("Please select a practical to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please fill both title and description.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating practical: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedPracticalId == 0)
            {
                MessageBox.Show("Please select a practical to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show($"Are you sure you want to delete practical '{txtTitle.Text}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                using (var conn = Database.GetConnection())
                {
                    string query = "DELETE FROM Practicals WHERE PracticalID = @practicalId";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@practicalId", selectedPracticalId);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Practical deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadPracticals();
                            ClearForm();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error deleting practical: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}