using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace LabManagementSystem.Forms
{
    public partial class ComputerForm : Form
    {
        private int selectedComputerId = 0; // To store the ID of the computer selected in the DataGridView

        public ComputerForm()
        {
            InitializeComponent();
        }

        private void ComputerForm_Load(object sender, EventArgs e)
        {
            LoadComputers();
            cmbStatus.SelectedIndex = 0; // Set default status to "Working"
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading computers: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (string.IsNullOrWhiteSpace(txtSystemNo.Text) || string.IsNullOrWhiteSpace(txtConfiguration.Text) ||
                cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Please fill all computer details.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    }
                    catch (SQLiteException ex)
                    {
                        if (ex.ResultCode == SQLiteErrorCode.Constraint)
                        {
                            MessageBox.Show("A computer with this System Number already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Error adding computer: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding computer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedComputerId == 0)
            {
                MessageBox.Show("Please select a computer to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSystemNo.Text) || string.IsNullOrWhiteSpace(txtConfiguration.Text) ||
                cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Please fill all computer details.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    }
                    catch (SQLiteException ex)
                    {
                        if (ex.ResultCode == SQLiteErrorCode.Constraint)
                        {
                            MessageBox.Show("Another computer with this System Number already exists. System numbers must be unique.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Error updating computer: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating computer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedComputerId == 0)
            {
                MessageBox.Show("Please select a computer to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show($"Are you sure you want to delete computer '{txtSystemNo.Text}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                using (var conn = Database.GetConnection())
                {
                    string query = "DELETE FROM Computers WHERE ComputerID = @computerId";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@computerId", selectedComputerId);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Computer deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadComputers();
                            ClearForm();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error deleting computer: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}