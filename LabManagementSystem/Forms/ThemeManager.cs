using System;
using System.Drawing;
using System.Windows.Forms;

namespace LabManagementSystem
{
    public static class ThemeManager
    {
        public enum AppTheme
        {
            Light,
            Dark
        }

        public static AppTheme CurrentTheme { get; private set; } = AppTheme.Light;

        public static event Action<AppTheme> OnThemeChanged;

        public static void SetTheme(AppTheme theme)
        {
            CurrentTheme = theme;
            OnThemeChanged?.Invoke(theme);
        }

        public static void ApplyTheme(Control parent)
        {
            if (parent == null) return;

            if (CurrentTheme == AppTheme.Dark)
            {
                parent.BackColor = Color.FromArgb(40, 40, 43);
                parent.ForeColor = Color.WhiteSmoke;
            }
            else
            {
                parent.BackColor = SystemColors.Control;
                parent.ForeColor = SystemColors.ControlText;
            }

            foreach (Control ctrl in parent.Controls)
            {
                ApplyTheme(ctrl);
            }
        }
    }
}
