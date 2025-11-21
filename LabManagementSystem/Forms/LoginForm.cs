using System;
using System.Data.SQLite;
using System.Windows.Forms;
using LabManagementSystem;

namespace LabManagementSystem.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.Load += LoginForm_Load_Themed; // New themed load handler
            ThemeManager.OnThemeChanged += ThemeManager_OnThemeChanged; // Subscribe to theme changes
        }

        private void LoginForm_Load_Themed(object sender, EventArgs e)
        {
            // Optional: Pre-fill for testing
            txtUsername.Text = "admin";
            txtPassword.Text = "admin";
            ThemeManager.ApplyTheme(this); // Apply theme on load
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Logger.LogWarning("Login attempt with empty username or password.");
                return;
            }

            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Logger.LogInfo($"User '{username}' logged in successfully.");
                this.Hide();
                Dashboard dashboard = new Dashboard();
                dashboard.ShowDialog();
                this.Close(); // Close login form once dashboard is closed
            }
            else
            {
                MessageBox.Show("Invalid Username or Password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear(); // Clear password field for security
                Logger.LogWarning($"Failed login attempt for username: {username}");
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            using (var conn = Database.GetConnection())
            {
                string query = "SELECT COUNT(1) FROM Users WHERE Username = @username AND Password = @password";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password); // In a real app, hash passwords!
                    try
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Database error during login for username: {username}", ex);
                        MessageBox.Show("An error occurred during login. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
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