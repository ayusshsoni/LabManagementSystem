using System;
using System.Data.SQLite;
using System.Windows.Forms;
using LabManagementSystem;

namespace LabManagementSystem.Forms
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
            this.Load += Dashboard_Load_Themed; // New themed load handler
            ThemeManager.OnThemeChanged += ThemeManager_OnThemeChanged; // Subscribe to theme changes
        }

        private void Dashboard_Load_Themed(object sender, EventArgs e)
        {
            notificationTimer.Start(); // Start the timer for notifications
            ApplyCurrentTheme(); // Apply theme on load
            UpdateDashboardCounts(); // Load dashboard numbers
            CheckForNotifications(); // Initial check for notifications
        }

        // Helper method to open child forms
        private void OpenChildForm(Form childForm)
        {
            // Close any existing MDI children if you want only one open at a time
            foreach (Form frm in this.MdiChildren)
            {
                frm.Close();
            }

            childForm.MdiParent = this;
            childForm.WindowState = FormWindowState.Maximized; // Maximize child forms
            childForm.Show();
        }

        private void manageStudentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new StudentForm());
        }

        private void manageComputersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ComputerForm());
        }

        private void managePracticalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new PracticalForm());
        }

        private void createSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new SessionForm());
        }

        private void markAttendanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new AttendanceForm());
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to log out of the Lab Management System?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result == DialogResult.Yes)
            {
                Logger.LogInfo("User logged out.");
                this.Close(); // Close the dashboard
                // Show the login form again
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
            }
        }

        // --- Theming related methods ---
        private void ThemeManager_OnThemeChanged(ThemeManager.AppTheme theme)
        {
            ApplyCurrentTheme();
            // Also apply to all open MDI child forms
            foreach (Form childForm in this.MdiChildren)
            {
                ThemeManager.ApplyTheme(childForm);
            }
        }

        private void ApplyCurrentTheme()
        {
            ThemeManager.ApplyTheme(this);
            // Reapply colors for the status label, as its parent's forecolor might change
            if (toolStripStatusLabel1.ForeColor == System.Drawing.Color.Red)
            {
                // Keep red for alerts
            }
            else
            {
                toolStripStatusLabel1.ForeColor = ThemeManager.CurrentTheme == ThemeManager.AppTheme.Dark ? System.Drawing.Color.WhiteSmoke : System.Drawing.SystemColors.ControlText;
            }
            // Update panel backcolors explicitly if ThemeManager doesn't handle nested panels well
            panel1.BackColor = ThemeManager.CurrentTheme == ThemeManager.AppTheme.Dark ? System.Drawing.Color.FromArgb(50, 50, 55) : System.Drawing.SystemColors.ControlLightLight;
            panel2.BackColor = ThemeManager.CurrentTheme == ThemeManager.AppTheme.Dark ? System.Drawing.Color.FromArgb(50, 50, 55) : System.Drawing.SystemColors.ControlLightLight;
            panel3.BackColor = ThemeManager.CurrentTheme == ThemeManager.AppTheme.Dark ? System.Drawing.Color.FromArgb(50, 50, 55) : System.Drawing.SystemColors.ControlLightLight;
            panel4.BackColor = ThemeManager.CurrentTheme == ThemeManager.AppTheme.Dark ? System.Drawing.Color.FromArgb(50, 50, 55) : System.Drawing.SystemColors.ControlLightLight;
            panel5.BackColor = ThemeManager.CurrentTheme == ThemeManager.AppTheme.Dark ? System.Drawing.Color.FromArgb(50, 50, 55) : System.Drawing.SystemColors.ControlLightLight;

            // Ensure dashboard numbers labels inside panels pick up forecolor
            foreach (Control control in panel1.Controls) { if (control is Label) control.ForeColor = ThemeManager.CurrentTheme == ThemeManager.AppTheme.Dark ? System.Drawing.Color.WhiteSmoke : System.Drawing.SystemColors.ControlText; }
            foreach (Control control in panel2.Controls) { if (control is Label) control.ForeColor = ThemeManager.CurrentTheme == ThemeManager.AppTheme.Dark ? System.Drawing.Color.WhiteSmoke : System.Drawing.SystemColors.ControlText; }
            foreach (Control control in panel3.Controls) { if (control is Label) control.ForeColor = ThemeManager.CurrentTheme == ThemeManager.AppTheme.Dark ? System.Drawing.Color.WhiteSmoke : System.Drawing.SystemColors.ControlText; }
            foreach (Control control in panel4.Controls) { if (control is Label) control.ForeColor = ThemeManager.CurrentTheme == ThemeManager.AppTheme.Dark ? System.Drawing.Color.WhiteSmoke : System.Drawing.SystemColors.ControlText; }
            foreach (Control control in panel5.Controls) { if (control is Label) control.ForeColor = ThemeManager.CurrentTheme == ThemeManager.AppTheme.Dark ? System.Drawing.Color.WhiteSmoke : System.Drawing.SystemColors.ControlText; }
        }

        private void lightThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThemeManager.CurrentTheme = ThemeManager.AppTheme.Light;
        }

        private void darkThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThemeManager.CurrentTheme = ThemeManager.AppTheme.Dark;
        }

        // --- Notifications/Alerts ---
        private void notificationTimer_Tick(object sender, EventArgs e)
        {
            CheckForNotifications();
        }

        private void CheckForNotifications()
        {
            string alertMessage = "";

            using (var conn = Database.GetConnection())
            {
                // 1. Check for computers under maintenance
                string maintenanceQuery = "SELECT COUNT(1) FROM Computers WHERE Status = 'Under Maintenance'";
                using (var cmd = new SQLiteCommand(maintenanceQuery, conn))
                {
                    try
                    {
                        int maintenanceCount = Convert.ToInt32(cmd.ExecuteScalar());
                        if (maintenanceCount > 0)
                        {
                            alertMessage += $"{maintenanceCount} computer(s) under maintenance. ";
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error checking for maintenance computers for notifications.", ex);
                    }
                }

                // 2. Check for upcoming sessions (e.g., today or tomorrow)
                string today = DateTime.Today.ToString("yyyy-MM-dd");
                string tomorrow = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

                string upcomingSessionQuery = @"
                    SELECT COUNT(1) FROM LabSessions
                    WHERE Date = @today OR Date = @tomorrow";
                using (var cmd = new SQLiteCommand(upcomingSessionQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@today", today);
                    cmd.Parameters.AddWithValue("@tomorrow", tomorrow);
                    try
                    {
                        int upcomingSessionCount = Convert.ToInt32(cmd.ExecuteScalar());
                        if (upcomingSessionCount > 0)
                        {
                            alertMessage += $"{upcomingSessionCount} lab session(s) scheduled for today/tomorrow. ";
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error checking for upcoming sessions for notifications.", ex);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(alertMessage))
            {
                toolStripStatusLabel1.Text = "Alert: " + alertMessage.Trim();
                toolStripStatusLabel1.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                toolStripStatusLabel1.Text = "No new alerts.";
                toolStripStatusLabel1.ForeColor = ThemeManager.CurrentTheme == ThemeManager.AppTheme.Dark ? System.Drawing.Color.WhiteSmoke : System.Drawing.SystemColors.ControlText;
            }
        }

        // --- Dashboard Numbers (Counts) ---
        private void UpdateDashboardCounts()
        {
            using (var conn = Database.GetConnection())
            {
                try
                {
                    lblTotalStudents.Text = GetCount("Students").ToString();
                    lblTotalComputers.Text = GetCount("Computers").ToString();
                    lblWorkingComputers.Text = GetCount("Computers", "Status = 'Working'").ToString();
                    lblMaintenanceComputers.Text = GetCount("Computers", "Status = 'Under Maintenance'").ToString();
                    lblTotalPracticals.Text = GetCount("Practicals").ToString();
                    lblUpcomingSessions.Text = GetCount("LabSessions", $"Date >= '{DateTime.Today.ToString("yyyy-MM-dd")}'").ToString();

                    Logger.LogInfo("Dashboard counts updated successfully.");
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error updating dashboard counts.", ex);
                    // Don't show MessageBox for this, just log silently on the dashboard
                }
            }
        }

        private int GetCount(string tableName, string whereClause = "")
        {
            int count = 0;
            using (var conn = Database.GetConnection())
            {
                string query = $"SELECT COUNT(1) FROM {tableName}";
                if (!string.IsNullOrWhiteSpace(whereClause))
                {
                    query += $" WHERE {whereClause}";
                }
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    try
                    {
                        count = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Error getting count for table '{tableName}' with where clause '{whereClause}'.", ex);
                        // Return 0 or rethrow, depending on how critical this is. For dashboard counts, 0 is acceptable.
                        return 0;
                    }
                }
            }
            return count;
        }

        private void viewReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ReportsForm());
        }
    }
}