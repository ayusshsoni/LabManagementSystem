using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace LabManagementSystem
{
    // Make the class static so its methods can be called directly (e.g., Database.GetConnection())
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
                return conn;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not establish database connection: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                SQLiteConnection.CreateFile(databaseFileName);
                CreateTables();
                SeedInitialUser();
            }
        }

        private static void CreateTables()
        {
            // SQL commands for all tables based on your schema
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

            using (var conn = GetConnection())
            {
                try
                {
                    using (var cmd = new SQLiteCommand(sqlCommands, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating tables: {ex.Message}", "Database Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static void SeedInitialUser()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    // Default Admin User: Username: admin, Password: admin
                    string insertUser = "INSERT INTO Users (Username, Password) VALUES ('admin', 'admin');";
                    using (var cmd = new SQLiteCommand(insertUser, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Catching this is important if a unique constraint failure happens, 
                // which means the user was somehow created during table creation or a subsequent run.
                MessageBox.Show($"Initial admin user seeding failed: {ex.Message}", "DB Seed Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}