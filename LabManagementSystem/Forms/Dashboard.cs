using System;
using System.Windows.Forms;

namespace LabManagementSystem.Forms
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
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
            DialogResult result = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close(); // Close the dashboard
                // Show the login form again
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
            }
        }
    }
}