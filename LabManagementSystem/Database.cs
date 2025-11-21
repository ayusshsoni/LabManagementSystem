using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace LabManagementSystem
{
    public static class Database
    {
        private static string databaseFileName = "lab.db";
        private static string connectionString = $"Data Source={databaseFileName};Version=3;";

        /// <summary>
        /// Provides a new, open SQLite connection.
        /// </summary>
        public static SQLiteConnection GetConnection()
        {
            try
            {
                var conn = new SQLiteConnection(connectionString);
                conn.Open();
                Logger.LogInfo("Database connection opened successfully.");
                return conn;
            }
            catch (Exception ex)
            {
                Logger.LogError("Could not establish database connection.", ex);
                // For critical connection errors, still show a minimal user message
                MessageBox.Show("Could not establish database connection. See log for details.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// Initializes the database file and creates all necessary tables if the file does not exist.
        /// </summary>
        public static void InitializeDatabase()
        {
            if (!File.Exists(databaseFileName))
            {
                try
                {
                    SQLiteConnection.CreateFile(databaseFileName);
                    Logger.LogInfo("Database file created.");
                    CreateTables();
                    SeedInitialUser();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error during initial database file creation or table seeding.", ex);
                    MessageBox.Show($"Critical error during database setup. Application may not function correctly. See log for details.", "Database Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw; // Re-throw to indicate a severe problem
                }
            }
            else
            {
                Logger.LogInfo("Database file already exists.");
            }
        }

        private static void CreateTables()
        {
            string sqlCommands = @"
                -- 1. Students Table
                CREATE TABLE IF NOT EXISTS Students(
                    StudentID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    RollNo TEXT UNIQUE,
                    Course TEXT,
                    Year TEXT
                );

                -- 2. Computers Table
                CREATE TABLE IF NOT EXISTS Computers(
                    ComputerID INTEGER PRIMARY KEY AUTOINCREMENT,
                    SystemNo TEXT UNIQUE,
                    Configuration TEXT,
                    Status TEXT -- e.g., Working, Under Maintenance
                );

                -- 3. Practicals Table
                CREATE TABLE IF NOT EXISTS Practicals(
                    PracticalID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT,
                    Description TEXT
                );

                -- 4. LabSessions Table
                CREATE TABLE IF NOT EXISTS LabSessions(
                    SessionID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT,
                    Time TEXT,
                    PracticalID INT,
                    FOREIGN KEY(PracticalID) REFERENCES Practicals(PracticalID)
                );

                -- 5. SessionAssignments Table (Which student used which computer in a session)
                CREATE TABLE IF NOT EXISTS SessionAssignments(
                    AssignID INTEGER PRIMARY KEY AUTOINCREMENT,
                    SessionID INT,
                    StudentID INT,
                    ComputerID INT,
                    UNIQUE(SessionID, StudentID),
                    FOREIGN KEY(SessionID) REFERENCES LabSessions(SessionID),
                    FOREIGN KEY(StudentID) REFERENCES Students(StudentID),
                    FOREIGN KEY(ComputerID) REFERENCES Computers(ComputerID)
                );

                -- 6. Attendance Table
                CREATE TABLE IF NOT EXISTS Attendance(
                    AttendanceID INTEGER PRIMARY KEY AUTOINCREMENT,
                    SessionID INT,
                    StudentID INT,
                    Status TEXT, -- e.g., Present, Absent
                    UNIQUE(SessionID, StudentID),
                    FOREIGN KEY(SessionID) REFERENCES LabSessions(SessionID),
                    FOREIGN KEY(StudentID) REFERENCES Students(StudentID)
                );

                -- 7. Users (Login) Table
                CREATE TABLE IF NOT EXISTS Users(
                    UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT UNIQUE,
                    Password TEXT
                );
            ";

            using (var conn = GetConnection()) // Use GetConnection which has logging
            {
                try
                {
                    using (var cmd = new SQLiteCommand(sqlCommands, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    Logger.LogInfo("Database tables created or verified.");
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error creating tables during database initialization.", ex);
                    MessageBox.Show("Error creating database tables. See log for details.", "Database Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static void SeedInitialUser()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    // Check if 'admin' user already exists before inserting
                    string checkUser = "SELECT COUNT(1) FROM Users WHERE Username = 'admin'";
                    using (var checkCmd = new SQLiteCommand(checkUser, conn))
                    {
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) == 0)
                        {
                            // Default Admin User: Username: admin, Password: admin
                            string insertUser = "INSERT INTO Users (Username, Password) VALUES ('admin', 'admin');"; // Changed password from 'password' to 'admin'
                            using (var cmd = new SQLiteCommand(insertUser, conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                            Logger.LogInfo("Initial 'admin' user seeded.");
                        }
                        else
                        {
                            Logger.LogInfo("Initial 'admin' user already exists.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Initial admin user seeding failed.", ex);
                MessageBox.Show($"Initial admin user seeding failed. See log for details.", "DB Seed Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}