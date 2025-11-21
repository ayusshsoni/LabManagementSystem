namespace LabManagementSystem.Forms
{
    partial class Dashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.studentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageStudentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.computerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageComputersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.practicalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.managePracticalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attendanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.markAttendanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip1
            //
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.studentToolStripMenuItem,
            this.computerToolStripMenuItem,
            this.practicalToolStripMenuItem,
            this.sessionToolStripMenuItem,
            this.attendanceToolStripMenuItem,
            this.logoutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1006, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            //
            // studentToolStripMenuItem
            //
            this.studentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manageStudentsToolStripMenuItem});
            this.studentToolStripMenuItem.Name = "studentToolStripMenuItem";
            this.studentToolStripMenuItem.Size = new System.Drawing.Size(74, 24);
            this.studentToolStripMenuItem.Text = "Student";
            //
            // manageStudentsToolStripMenuItem
            //
            this.manageStudentsToolStripMenuItem.Name = "manageStudentsToolStripMenuItem";
            this.manageStudentsToolStripMenuItem.Size = new System.Drawing.Size(200, 26);
            this.manageStudentsToolStripMenuItem.Text = "Manage Students";
            this.manageStudentsToolStripMenuItem.Click +=
                new System.EventHandler(this.manageStudentsToolStripMenuItem_Click);
            //
            // computerToolStripMenuItem
            //
            this.computerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manageComputersToolStripMenuItem});
            this.computerToolStripMenuItem.Name = "computerToolStripMenuItem";
            this.computerToolStripMenuItem.Size = new System.Drawing.Size(89, 24);
            this.computerToolStripMenuItem.Text = "Computer";
            //
            // manageComputersToolStripMenuItem
            //
            this.manageComputersToolStripMenuItem.Name = "manageComputersToolStripMenuItem";
            this.manageComputersToolStripMenuItem.Size = new System.Drawing.Size(215, 26);
            this.manageComputersToolStripMenuItem.Text = "Manage Computers";
            this.manageComputersToolStripMenuItem.Click +=
                new System.EventHandler(this.manageComputersToolStripMenuItem_Click);
            //
            // practicalToolStripMenuItem
            //
            this.practicalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.managePracticalsToolStripMenuItem});
            this.practicalToolStripMenuItem.Name = "practicalToolStripMenuItem";
            this.practicalToolStripMenuItem.Size = new System.Drawing.Size(79, 24);
            this.practicalToolStripMenuItem.Text = "Practical";
            //
            // managePracticalsToolStripMenuItem
            //
            this.managePracticalsToolStripMenuItem.Name = "managePracticalsToolStripMenuItem";
            this.managePracticalsToolStripMenuItem.Size = new System.Drawing.Size(205, 26);
            this.managePracticalsToolStripMenuItem.Text = "Manage Practicals";
            this.managePracticalsToolStripMenuItem.Click +=
                new System.EventHandler(this.managePracticalsToolStripMenuItem_Click);
            //
            // sessionToolStripMenuItem
            //
            this.sessionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createSessionToolStripMenuItem});
            this.sessionToolStripMenuItem.Name = "sessionToolStripMenuItem";
            this.sessionToolStripMenuItem.Size = new System.Drawing.Size(72, 24);
            this.sessionToolStripMenuItem.Text = "Session";
            //
            // createSessionToolStripMenuItem
            //
            this.createSessionToolStripMenuItem.Name = "createSessionToolStripMenuItem";
            this.createSessionToolStripMenuItem.Size = new System.Drawing.Size(186, 26);
            this.createSessionToolStripMenuItem.Text = "Create Session";
            this.createSessionToolStripMenuItem.Click +=
                new System.EventHandler(this.createSessionToolStripMenuItem_Click);
            //
            // attendanceToolStripMenuItem
            //
            this.attendanceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.markAttendanceToolStripMenuItem});
            this.attendanceToolStripMenuItem.Name = "attendanceToolStripMenuItem";
            this.attendanceToolStripMenuItem.Size = new System.Drawing.Size(98, 24);
            this.attendanceToolStripMenuItem.Text = "Attendance";
            //
            // markAttendanceToolStripMenuItem
            //
            this.markAttendanceToolStripMenuItem.Name = "markAttendanceToolStripMenuItem";
            this.markAttendanceToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.markAttendanceToolStripMenuItem.Text = "Mark Attendance";
            this.markAttendanceToolStripMenuItem.Click +=
                new System.EventHandler(this.markAttendanceToolStripMenuItem_Click);
            //
            // logoutToolStripMenuItem
            //
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            this.logoutToolStripMenuItem.Text = "Logout";
            this.logoutToolStripMenuItem.Click +=
                new System.EventHandler(this.logoutToolStripMenuItem_Click);
            //
            // Dashboard
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 597);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Dashboard";
            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lab Management System - Dashboard";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem studentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageStudentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem computerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageComputersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem practicalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem managePracticalsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem attendanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem markAttendanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
    }
}