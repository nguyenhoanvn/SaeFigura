using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FigureManagementSystem.Models;

namespace FigureManagementSystem
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {

            // Trigger LostFocus manually for validation
            FirstNameTextBox_LostFocus(FirstNameTextBox, null);
            LastNameTextBox_LostFocus(LastNameTextBox, null);
            UsernameTextBox_LostFocus(UsernameTextBox, null);
            EmailTextBox_LostFocus(EmailTextBox, null);
            AddressTextBox_LostFocus(AddressTextBox, null);
            PhoneTextBox_LostFocus(PhoneTextBox, null);
            PasswordBox_LostFocus(PasswordBox, null);


            bool hasErrors =
                !string.IsNullOrEmpty(FirstNameError.Text) ||
                !string.IsNullOrEmpty(LastNameError.Text) ||
                !string.IsNullOrEmpty(UsernameError.Text) ||
                !string.IsNullOrEmpty(PhoneError.Text) ||
                !string.IsNullOrEmpty(PasswordError.Text);

            if (hasErrors)
            {
                MessageBox.Show("Please fix all errors before signing up.", "Validation Errors",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Proceed with registration logic (example placeholder)
            try
            {
                bool isSuccess = RegisterUser(firstName: FirstNameTextBox.Text,
                        lastName: LastNameTextBox.Text,
                        username: UsernameTextBox.Text,
                        password: PasswordBox.Password,
                        email: EmailTextBox.Text,
                        address: AddressTextBox.Text,
                        phone: PhoneTextBox.Text);
                if (isSuccess)
                {
                    MessageBox.Show("Registration successful!", "Success",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                } else
                {
                    MessageBox.Show("Registration falied!", "Falied",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.DialogResult = true;
            
        }

        public bool IsValidUsername(string username)
        {
            var forbidden = new[] { "admin", "support", "staff" };
            return username.Length >= 3
                && username.Length <= 16
                && !char.IsDigit(username[0])
                && username.All(c => (char.IsLetterOrDigit(c) || c == '_'))
                && !username.Any(char.IsWhiteSpace)
                && username.All(c => c <= 127)
                && forbidden.All(word => !username.ToLower().Contains(word));
        }

        public bool IsValidPassword(string password) => password.Length >= 8 && password.Length <= 64;

        public bool IsValidName(string name) => name.All(char.IsLetter) && name.Length > 0;

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true; 
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern);
        }

        public bool IsValidAddress(string address)
        {
            return address.All(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '_');
        }

        public bool IsValidPhone(string phone)
        {
            return phone.Length == 10 && phone.StartsWith("0") && phone.All(char.IsDigit);
        }

        private void FirstNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            
            if (tb == null)
            {
                MessageBox.Show("Null in first name", "NULL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (!IsValidName(tb.Text))
            {
                FirstNameTextBox.BorderBrush = Brushes.Red;
                FirstNameError.Visibility = Visibility.Visible;
                FirstNameError.Text = "First name must not empty and cannot contain letters";
            } else
            {
                FirstNameTextBox.ClearValue(BorderBrushProperty);
                FirstNameError.Visibility = Visibility.Collapsed;
                FirstNameError.Text = "";
            }
            return;
        }

        private void LastNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (!IsValidName(tb.Text))
            {
                tb.BorderBrush = Brushes.Red;
                LastNameError.Text = "Last name cannot be empty and must contain only letters";
            }
            else
            {
                tb.ClearValue(BorderBrushProperty);
                LastNameError.Text = "";
            }
        }

        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (!IsValidUsername(tb.Text))
            {
                tb.BorderBrush = Brushes.Red;
                UsernameError.Text = "Username must be 3-16 chars, no spaces, digits, or forbidden words";
            }
            else
            {
                tb.ClearValue(BorderBrushProperty);
                UsernameError.Text = "";
            }
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (!IsValidEmail(tb.Text))
            {
                tb.BorderBrush = Brushes.Red;
                EmailError.Text = "Please enter a valid email address";
            }
            else
            {
                tb.ClearValue(BorderBrushProperty);
                EmailError.Text = "";
            }
        }

        private void AddressTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (!IsValidAddress(tb.Text))
            {
                tb.BorderBrush = Brushes.Red;
                AddressError.Text = "Address contains invalid characters";
            }
            else
            {
                tb.ClearValue(BorderBrushProperty);
                AddressError.Text = "";
            }
        }

        private void PhoneTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (!IsValidPhone(tb.Text))
            {
                tb.BorderBrush = Brushes.Red;
                PhoneError.Text = "Phone must be exactly 10 digits, start with 0";
            }
            else
            {
                tb.ClearValue(BorderBrushProperty);
                PhoneError.Text = "";
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var pb = sender as PasswordBox;

            if (pb == null)
                return;

            if (!IsValidPassword(pb.Password))
            {
                pb.BorderBrush = Brushes.Red;
                PasswordError.Text = "Password must be 8-64 characters long";
            }
            else
            {
                pb.ClearValue(BorderBrushProperty);
                PasswordError.Text = "";
            }
        }

        private bool RegisterUser(string firstName, string lastName, string username, string password, string email, string address, string phone)
        {
            using (var context = new ProjectContext())
            {
                TblUser userToAdd = new TblUser();
                userToAdd.UserId = username;
                userToAdd.FullName = string.Join(firstName, ' ', lastName);
                userToAdd.Password = password;
                userToAdd.Email = email;
                userToAdd.RoleId = 1;
                userToAdd.Address = address;
                userToAdd.Phone = phone;
                userToAdd.IsActive = true;

                context.TblUsers.Add(userToAdd);
                context.SaveChanges();
                if (context.TblUsers.Any(u => u.FullName.Equals(string.Join(firstName, ' ', lastName)) &&
                u.UserId == username))
                {
                    return true;
                } else
                {
                    return false;
                }
            } 
        }

        private string HashPassword(string password)
        {
            // Generate a bcrypt hash with default strength (10)
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
