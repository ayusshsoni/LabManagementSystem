using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LabManagementSystem.Utilities
{
    public static class InputValidator
    {
        public static bool RequireText(TextBox textBox, string fieldLabel, int minLength = 1)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text.Trim().Length < minLength)
            {
                MessageBox.Show($"Please enter a valid {fieldLabel} (min {minLength} characters).", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox.Focus();
                return false;
            }

            textBox.Text = textBox.Text.Trim();
            return true;
        }

        public static bool RequirePattern(TextBox textBox, string fieldLabel, string regexPattern, string friendlyMessage)
        {
            if (!Regex.IsMatch(textBox.Text.Trim(), regexPattern, RegexOptions.IgnoreCase))
            {
                MessageBox.Show(friendlyMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox.Focus();
                return false;
            }

            return true;
        }

        public static bool RequireComboSelection(ComboBox comboBox, string fieldLabel)
        {
            if (comboBox.SelectedItem == null)
            {
                MessageBox.Show($"Please select a value for {fieldLabel}.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox.Focus();
                return false;
            }

            return true;
        }

        public static bool RequireNumericRange(TextBox textBox, string fieldLabel, int minInclusive, int maxInclusive)
        {
            if (!int.TryParse(textBox.Text.Trim(), out int parsedValue) || parsedValue < minInclusive || parsedValue > maxInclusive)
            {
                MessageBox.Show($"{fieldLabel} must be a number between {minInclusive} and {maxInclusive}.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox.Focus();
                return false;
            }

            return true;
        }

        public static bool RequireFutureOrToday(DateTime value, string fieldLabel)
        {
            if (value.Date < DateTime.Today)
            {
                MessageBox.Show($"{fieldLabel} cannot be in the past.", "Validation Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
}

