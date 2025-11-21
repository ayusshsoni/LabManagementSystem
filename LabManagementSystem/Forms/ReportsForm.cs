using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using LabManagementSystem; // This is for ThemeManager
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting; // This is for Chart, ChartArea, Legend, Series

namespace LabManagementSystem.Forms
{
    public partial class ReportsForm : Form
    {
        public ReportsForm()
        {
            InitializeComponent();
            this.Load += ReportsForm_Load_Themed; // Use the themed load handler
            ThemeManager.OnThemeChanged += ThemeManager_OnThemeChanged; // Subscribe to theme changes
        }

        private void ReportsForm_Load_Themed(object sender, EventArgs e)
        {
            LoadComputerStatusChart();
            ThemeManager.ApplyTheme(this); // Apply theme on load
        }

        private void ThemeManager_OnThemeChanged(ThemeManager.AppTheme theme)
        {
            ThemeManager.ApplyTheme(this); // Apply theme when notified
            // Re-render charts to apply theme colors if chart elements were themed
            ApplyChartTheme(chartComputerStatus);
        }

        private void LoadComputerStatusChart()
        {
            Dictionary<string, int> statusCounts = GetComputerStatusCounts();

            // Clear previous data
            chartComputerStatus.Series["S1"].Points.Clear();
            chartComputerStatus.Titles.Clear();
            chartComputerStatus.Titles.Add("Computer Status Distribution");

            foreach (var entry in statusCounts)
            {
                DataPoint point = new DataPoint();
                point.SetValueY(entry.Value);
                point.Label = $"{entry.Key} ({entry.Value})"; // Label with status and count
                point.LegendText = entry.Key; // Text for the legend
                chartComputerStatus.Series["S1"].Points.Add(point);
            }

            // Optional: Customize chart appearance (apply theme)
            ApplyChartTheme(chartComputerStatus);

            Logger.LogInfo("Computer status chart loaded.");
        }

        private Dictionary<string, int> GetComputerStatusCounts()
        {
            Dictionary<string, int> statusCounts = new Dictionary<string, int>();
            using (var conn = Database.GetConnection())
            {
                string query = "SELECT Status, COUNT(1) AS Count FROM Computers GROUP BY Status";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string status = reader["Status"].ToString();
                                int count = Convert.ToInt32(reader["Count"]);
                                statusCounts.Add(status, count);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error getting computer status counts for chart.", ex);
                        MessageBox.Show("An error occurred while loading chart data. See log for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            return statusCounts;
        }

        private void ApplyChartTheme(Chart chart)
        {
            // Apply theme colors to chart elements
            if (ThemeManager.CurrentTheme == ThemeManager.AppTheme.Dark)
            {
                chart.BackColor = Color.FromArgb(45, 45, 48);
                chart.ForeColor = Color.WhiteSmoke;
                chart.Titles[0].ForeColor = Color.WhiteSmoke;
                chart.Legends[0].BackColor = Color.Transparent; // Or a dark color
                chart.Legends[0].ForeColor = Color.WhiteSmoke;
                chart.ChartAreas[0].BackColor = Color.FromArgb(45, 45, 48); // Plot area
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(80, 80, 80); // Darker grid lines
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(80, 80, 80);
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.WhiteSmoke;
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.WhiteSmoke;
                chart.ChartAreas[0].AxisX.TitleForeColor = Color.WhiteSmoke;
                chart.ChartAreas[0].AxisY.TitleForeColor = Color.WhiteSmoke;
                chart.ChartAreas[0].Area3DStyle.LightStyle = LightStyle.Realistic;
            }
            else
            {
                chart.BackColor = SystemColors.Control;
                chart.ForeColor = SystemColors.ControlText;
                chart.Titles[0].ForeColor = SystemColors.ControlText;
                chart.Legends[0].BackColor = Color.Transparent;
                chart.Legends[0].ForeColor = SystemColors.ControlText;
                chart.ChartAreas[0].BackColor = Color.White;
                chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = SystemColors.ControlText;
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = SystemColors.ControlText;
                chart.ChartAreas[0].AxisX.TitleForeColor = SystemColors.ControlText;
                chart.ChartAreas[0].AxisY.TitleForeColor = SystemColors.ControlText;
                chart.ChartAreas[0].Area3DStyle.LightStyle = LightStyle.Simplistic;
            }
        }
    }
}