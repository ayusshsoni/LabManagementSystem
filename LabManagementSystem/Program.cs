using LabManagementSystem.Forms; // Add this using directive
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabManagementSystem
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize the database when the application starts
            Database.InitializeDatabase();

            // Run the login form first
            Application.Run(new LoginForm());
        }
    }
}