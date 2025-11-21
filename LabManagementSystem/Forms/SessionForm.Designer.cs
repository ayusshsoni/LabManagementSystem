namespace LabManagementSystem.Forms
{
    partial class SessionForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbPractical = new System.Windows.Forms.ComboBox();
            this.btnAddSession = new System.Windows.Forms.Button();
            this.dgvSessions = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.dgvAvailableStudents = new System.Windows.Forms.DataGridView();
            this.label5 = new System.Windows.Forms.Label();
            this.dgvAvailableComputers = new System.Windows.Forms.DataGridView();
            this.label6 = new System.Windows.Forms.Label();
            this.btnAssignStudent = new System.Windows.Forms.Button();
            this.btnAssignComputer = new System.Windows.Forms.Button();
            this.dgvSessionAssignments = new System.Windows.Forms.DataGridView();
            this.label7 = new System.Windows.Forms.Label();
            this.btnClearSession = new System.Windows.Forms.Button();
            this.btnDeleteSession = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSessions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAvailableStudents)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAvailableComputers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSessionAssignments)).BeginInit();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Date:";
            //
            // dtpDate
            //
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(90, 17);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(120, 22);
            this.dtpDate.TabIndex = 1;
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Time:";
            //
            // txtTime
            //
            this.txtTime.Location = new System.Drawing.Point(90, 52);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(120, 22);
            this.txtTime.TabIndex = 3;
            this.txtTime.Text = "HH:MM (e.g., 10:00)";
            this.txtTime.Enter += new System.EventHandler(this.txtTime_Enter);
            this.txtTime.Leave += new System.EventHandler(this.txtTime_Leave);
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Practical:";
            //
            // cmbPractical
            //
            this.cmbPractical.DropDownStyle =
                System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPractical.FormattingEnabled = true;
            this.cmbPractical.Location = new System.Drawing.Point(90, 87);
            this.cmbPractical.Name = "cmbPractical";
            this.cmbPractical.Size = new System.Drawing.Size(200, 24);
            this.cmbPractical.TabIndex = 5;
            //
            // btnAddSession
            //
            this.btnAddSession.Location = new System.Drawing.Point(20, 130);
            this.btnAddSession.Name = "btnAddSession";
            this.btnAddSession.Size = new System.Drawing.Size(120, 30);
            this.btnAddSession.TabIndex = 6;
            this.btnAddSession.Text = "Add Session";
            this.btnAddSession.UseVisualStyleBackColor = true;
            this.btnAddSession.Click +=
                new System.EventHandler(this.btnAddSession_Click);
            //
            // dgvSessions
            //
            this.dgvSessions.AllowUserToAddRows = false;
            this.dgvSessions.AllowUserToDeleteRows = false;
            this.dgvSessions.Anchor =
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top |
                                                      System.Windows.Forms.AnchorStyles.Left) |
                                                     System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSessions.AutoSizeColumnsMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSessions.ColumnHeadersHeightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSessions.Location = new System.Drawing.Point(20, 190);
            this.dgvSessions.MultiSelect = false;
            this.dgvSessions.Name = "dgvSessions";
            this.dgvSessions.ReadOnly = true;
            this.dgvSessions.RowHeadersWidth = 51;
            this.dgvSessions.RowTemplate.Height = 24;
            this.dgvSessions.SelectionMode =
                System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSessions.Size = new System.Drawing.Size(950, 150);
            this.dgvSessions.TabIndex = 7;
            this.dgvSessions.CellClick +=
                new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSessions_CellClick);
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F,
                System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point,
                ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(20, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 18);
            this.label4.TabIndex = 8;
            this.label4.Text = "Existing Sessions";
            //
            // dgvAvailableStudents
            //
            this.dgvAvailableStudents.AllowUserToAddRows = false;
            this.dgvAvailableStudents.AllowUserToDeleteRows = false;
            this.dgvAvailableStudents.Anchor =
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top |
                                                      System.Windows.Forms.AnchorStyles.Bottom) |
                                                     System.Windows.Forms.AnchorStyles.Left)));
            this.dgvAvailableStudents.AutoSizeColumnsMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAvailableStudents.ColumnHeadersHeightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAvailableStudents.Location = new System.Drawing.Point(20, 390);
            this.dgvAvailableStudents.MultiSelect = false;
            this.dgvAvailableStudents.Name = "dgvAvailableStudents";
            this.dgvAvailableStudents.ReadOnly = true;
            this.dgvAvailableStudents.RowHeadersWidth = 51;
            this.dgvAvailableStudents.RowTemplate.Height = 24;
            this.dgvAvailableStudents.SelectionMode =
                System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAvailableStudents.Size = new System.Drawing.Size(300, 200);
            this.dgvAvailableStudents.TabIndex = 9;
            //
            // label5
            //
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F,
                System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point,
                ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(20, 368);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(142, 18);
            this.label5.TabIndex = 10;
            this.label5.Text = "Available Students";
            //
            // dgvAvailableComputers
            //
            this.dgvAvailableComputers.AllowUserToAddRows = false;
            this.dgvAvailableComputers.AllowUserToDeleteRows = false;
            this.dgvAvailableComputers.Anchor =
                ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top |
                                                      System.Windows.Forms.AnchorStyles.Bottom)));
            this.dgvAvailableComputers.AutoSizeColumnsMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAvailableComputers.ColumnHeadersHeightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAvailableComputers.Location = new System.Drawing.Point(340, 390);
            this.dgvAvailableComputers.MultiSelect = false;
            this.dgvAvailableComputers.Name = "dgvAvailableComputers";
            this.dgvAvailableComputers.ReadOnly = true;
            this.dgvAvailableComputers.RowHeadersWidth = 51;
            this.dgvAvailableComputers.RowTemplate.Height = 24;
            this.dgvAvailableComputers.SelectionMode =
                System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAvailableComputers.Size = new System.Drawing.Size(300, 200);
            this.dgvAvailableComputers.TabIndex = 11;
            //
            // label6
            //
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F,
                System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point,
                ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(340, 368);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(152, 18);
            this.label6.TabIndex = 12;
            this.label6.Text = "Available Computers";
            //
            // btnAssignStudent
            //
            this.btnAssignStudent.Location = new System.Drawing.Point(20, 600);
            this.btnAssignStudent.Name = "btnAssignStudent";
            this.btnAssignStudent.Size = new System.Drawing.Size(140, 30);
            this.btnAssignStudent.TabIndex = 13;
            this.btnAssignStudent.Text = "Assign Student";
            this.btnAssignStudent.UseVisualStyleBackColor = true;
            this.btnAssignStudent.Click +=
                new System.EventHandler(this.btnAssignStudent_Click);
            //
            // btnAssignComputer
            //
            this.btnAssignComputer.Location = new System.Drawing.Point(340, 600);
            this.btnAssignComputer.Name = "btnAssignComputer";
            this.btnAssignComputer.Size = new System.Drawing.Size(140, 30);
            this.btnAssignComputer.TabIndex = 14;
            this.btnAssignComputer.Text = "Assign Computer";
            this.btnAssignComputer.UseVisualStyleBackColor = true;
            this.btnAssignComputer.Click +=
                new System.EventHandler(this.btnAssignComputer_Click);
            //
            // dgvSessionAssignments
            //
            this.dgvSessionAssignments.AllowUserToAddRows = false;
            this.dgvSessionAssignments.AllowUserToDeleteRows = false;
            this.dgvSessionAssignments.Anchor =
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top |
                                                      System.Windows.Forms.AnchorStyles.Bottom) |
                                                     System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSessionAssignments.AutoSizeColumnsMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSessionAssignments.ColumnHeadersHeightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSessionAssignments.Location = new System.Drawing.Point(660, 390);
            this.dgvSessionAssignments.MultiSelect = false;
            this.dgvSessionAssignments.Name = "dgvSessionAssignments";
            this.dgvSessionAssignments.ReadOnly = true;
            this.dgvSessionAssignments.RowHeadersWidth = 51;
            this.dgvSessionAssignments.RowTemplate.Height = 24;
            this.dgvSessionAssignments.SelectionMode =
                System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSessionAssignments.Size = new System.Drawing.Size(310, 200);
            this.dgvSessionAssignments.TabIndex = 15;
            //
            // label7
            //
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F,
                System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point,
                ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(660, 368);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(161, 18);
            this.label7.TabIndex = 16;
            this.label7.Text = "Current Assignments";
            //
            // btnClearSession
            //
            this.btnClearSession.Location = new System.Drawing.Point(146, 130);
            this.btnClearSession.Name = "btnClearSession";
            this.btnClearSession.Size = new System.Drawing.Size(100, 30);
            this.btnClearSession.TabIndex = 17;
            this.btnClearSession.Text = "Clear Form";
            this.btnClearSession.UseVisualStyleBackColor = true;
            this.btnClearSession.Click +=
                new System.EventHandler(this.btnClearSession_Click);
            //
            // btnDeleteSession
            //
            this.btnDeleteSession.Location = new System.Drawing.Point(252, 130);
            this.btnDeleteSession.Name = "btnDeleteSession";
            this.btnDeleteSession.Size = new System.Drawing.Size(120, 30);
            this.btnDeleteSession.TabIndex = 18;
            this.btnDeleteSession.Text = "Delete Session";
            this.btnDeleteSession.UseVisualStyleBackColor = true;
            this.btnDeleteSession.Click +=
                new System.EventHandler(this.btnDeleteSession_Click);
            //
            // SessionForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 653);
            this.Controls.Add(this.btnDeleteSession);
            this.Controls.Add(this.btnClearSession);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.dgvSessionAssignments);
            this.Controls.Add(this.btnAssignComputer);
            this.Controls.Add(this.btnAssignStudent);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dgvAvailableComputers);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dgvAvailableStudents);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dgvSessions);
            this.Controls.Add(this.btnAddSession);
            this.Controls.Add(this.cmbPractical);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.label1);
            this.Name = "SessionForm";
            this.Text = "Lab Session Management";
            this.Load += new System.EventHandler(this.SessionForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSessions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAvailableStudents)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAvailableComputers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSessionAssignments)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbPractical;
        private System.Windows.Forms.Button btnAddSession;
        private System.Windows.Forms.DataGridView dgvSessions;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dgvAvailableStudents;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dgvAvailableComputers;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnAssignStudent;
        private System.Windows.Forms.Button btnAssignComputer;
        private System.Windows.Forms.DataGridView dgvSessionAssignments;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnClearSession;
        private System.Windows.Forms.Button btnDeleteSession;
    }
}