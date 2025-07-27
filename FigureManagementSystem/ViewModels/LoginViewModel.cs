﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FigureManagementSystem.Helpers;
using FigureManagementSystem.Models;
using FigureManagementSystem.Views;

namespace FigureManagementSystem.ViewModels
{
    public class LoginViewModel
    {
        private readonly LoginWindow _window;
        private bool isPasswordVisible = false;

        public LoginViewModel(LoginWindow window)
        {
            _window = window;

            _window.CloseButton.Click += CloseButton_Click;
            _window.LoginButton.Click += LoginButton_Click;
            _window.RegisterButton.Click += RegisterButton_Click;
            _window.ShowPasswordButton.Click += ShowPasswordButton_Click;
            _window.KeyDown += LoginScreen_KeyDown;
        }

        private void LoginScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
            else if (e.Key == Key.Escape)
            {
                _window.Close();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _window.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = _window.UsernameTextBox.Text.Trim();
            string password = isPasswordVisible ? _window.PasswordTextBox.Text : _window.PasswordBox.Password;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter your username or email.", "Login Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                _window.UsernameTextBox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter your password.", "Login Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                if (isPasswordVisible)
                    _window.PasswordTextBox.Focus();
                else
                    _window.PasswordBox.Focus();
                return;
            }

            if (AuthenticateUser(username, password))
            {
                using (var context = new FigureManagementSystemContext())
                {
                    var user = context.Users.FirstOrDefault(u => (u.Id == username) && u.IsActive == true);

                    if (user != null || !VerifyPassword(password, user.Password))
                    {
                        SessionManager.SetCurrentUser(user);
                        var roleName = context.Roles.FirstOrDefault(r => r.Id == user.RoleId)?.Name;
                        switch (roleName)
                        {
                            case "Admin":
                                var masterDataWindow = new MasterDataWindow();
                                Application.Current.MainWindow = masterDataWindow;
                                masterDataWindow.Show();
                                Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
                                break;
                            case "Manager":
                                var managerBusinessWindow = new ManagerBusinessWindow();
                                Application.Current.MainWindow = managerBusinessWindow;
                                managerBusinessWindow.Show();
                                Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
                                break;
                            case "Staff":
                                var posWindow = new POSWindow();
                                Application.Current.MainWindow = posWindow;
                                posWindow.Show();
                                Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
                                break;
                        }
                    } else
                    {
                        MessageBox.Show("An error occurred when we try to log you in.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.", "Login Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                if (isPasswordVisible)
                    _window.PasswordTextBox.Clear();
                else
                    _window.PasswordBox.Clear();

                _window.UsernameTextBox.Focus();
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            using (var context = new FigureManagementSystemContext())
            {
                var user = context.Users.FirstOrDefault(u =>
                    (u.Id.Equals(username) || u.Email.Equals(username)) &&
                    (u.IsActive == true));

                if (user != null)
                {
                    return VerifyPassword(password, user.Password);
                }
                else
                {
                    return false;
                }
            }
        }

        private void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPasswordVisible)
            {
                _window.PasswordBox.Password = _window.PasswordTextBox.Text;
                _window.PasswordBox.Visibility = Visibility.Visible;
                _window.PasswordTextBox.Visibility = Visibility.Collapsed;
                _window.ShowPasswordButton.Content = "👁";
                isPasswordVisible = false;
                _window.PasswordBox.Focus();
            }
            else
            {
                _window.PasswordTextBox.Text = _window.PasswordBox.Password;
                _window.PasswordTextBox.Visibility = Visibility.Visible;
                _window.PasswordBox.Visibility = Visibility.Collapsed;
                _window.ShowPasswordButton.Content = "🙈";
                isPasswordVisible = true;
                _window.PasswordTextBox.Focus();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            _window.Hide();
            bool? registered = registerWindow.ShowDialog();
            if (registered == true)
            {
                MessageBox.Show("Completed", "Good");
            }
            _window.Show();
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return !string.IsNullOrEmpty(password) && BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
