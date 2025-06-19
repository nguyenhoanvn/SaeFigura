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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool isPasswordVisible = false;

        public LoginWindow()
        {
            InitializeComponent();
            this.KeyDown += LoginScreen_KeyDown;
        }

        private void LoginScreen_KeyDown(object sender, KeyEventArgs e)
        {
            // Allow Enter key to trigger login
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
            // Allow Escape key to close window
            else if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = isPasswordVisible ? PasswordTextBox.Text : PasswordBox.Password;

            // Basic validation
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter your username or email.", "Login Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                UsernameTextBox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter your password.", "Login Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                if (isPasswordVisible)
                    PasswordTextBox.Focus();
                else
                    PasswordBox.Focus();
                return;
            }

            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Good", "Login success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.", "Login Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                if (isPasswordVisible)
                    PasswordTextBox.Clear();
                else
                    PasswordBox.Clear();

                UsernameTextBox.Focus();
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            using (var context = new ProjectContext())
            {
                var user = context.TblUsers.FirstOrDefault(u =>
                    (u.UserId.Equals(username) || u.Email.Equals(username)) &&
                    (u.IsActive == true));

                if (user != null)
                {
                    return user.Password.Equals(password);
                } else
                {
                    return false;
                }
            }
        }

        private void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPasswordVisible)
            {
                // Hide password
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                ShowPasswordButton.Content = "👁";
                isPasswordVisible = false;
                PasswordBox.Focus();
            }
            else
            {
                // Show password
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Collapsed;
                ShowPasswordButton.Content = "🙈";
                isPasswordVisible = true;
                PasswordTextBox.Focus();
            }
        }

        private void GoogleLoginButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement Google OAuth authentication
            MessageBox.Show("Google login functionality will be implemented here.",
                "Google Login", MessageBoxButton.OK, MessageBoxImage.Information);

            // Example implementation would involve:
            // 1. Redirect to Google OAuth URL
            // 2. Handle the callback
            // 3. Validate the token
            // 4. Create or update user in your database
            // 5. Proceed with login
        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open forgot password window or dialog
            MessageBox.Show("Forgot password functionality will be implemented here.",
                "Forgot Password", MessageBoxButton.OK, MessageBoxImage.Information);

            // You might want to:
            // 1. Open a new window for password reset
            // 2. Send reset email
            // 3. Provide security questions
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {

            RegisterWindow registerWindow = new RegisterWindow();
            this.Hide();
            bool? registered = registerWindow.ShowDialog();
            if (registered == true)
            {
                MessageBox.Show("Completed", "Good");
            }
            this.Show();

            // Example:
            // RegisterWindow registerWindow = new RegisterWindow();
            // registerWindow.ShowDialog();
        }


        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        // Window dragging functionality
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
