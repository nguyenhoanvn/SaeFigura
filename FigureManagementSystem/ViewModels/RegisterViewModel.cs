using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FigureManagementSystem.Helpers;
using FigureManagementSystem.Models;
using System.Windows.Input;
using System.Windows;
using Microsoft.IdentityModel.Tokens;

namespace FigureManagementSystem.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private FigureManagementSystemContext context = new FigureManagementSystemContext();

        // Properties (with OnPropertyChanged)
        private string _firstName;
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(nameof(FirstName)); ValidateFirstName(); } }

        private string _lastName;
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(nameof(LastName)); ValidateLastName(); } }

        private string _username;
        public string Username { get => _username; set { _username = value; OnPropertyChanged(nameof(Username)); ValidateUsername(); } }

        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(nameof(Email)); ValidateEmail(); } }

        private string _address;
        public string Address { get => _address; set { _address = value; OnPropertyChanged(nameof(Address)); ValidateAddress(); } }

        private string _phone;
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(nameof(Phone)); ValidatePhone(); } }

        private string _password;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(nameof(Password)); ValidatePassword(); } }

        private string _firstNameError;
        public string FirstNameError { get => _firstNameError; set { _firstNameError = value; OnPropertyChanged(nameof(FirstNameError)); }}
        private string _lastNameError; 
        public string LastNameError { get => _lastNameError; set { _lastNameError = value; OnPropertyChanged(nameof(LastNameError)); } }

        private string _usernameError;
        public string UsernameError { get => _usernameError; set { _usernameError = value; OnPropertyChanged(nameof(UsernameError)); } }

        private string _emailError;
        public string EmailError { get => _emailError; set { _emailError = value; OnPropertyChanged(nameof(EmailError)); } }

        private string _addressError; 
        public string AddressError { get => _addressError; set { _addressError = value; OnPropertyChanged(nameof(AddressError)); } }

        private string _phoneError; 
        public string PhoneError { get => _phoneError; set { _phoneError = value; OnPropertyChanged(nameof(PhoneError)); } }

        private string _passwordError; 
        public string PasswordError { get => _passwordError; set { _passwordError = value; OnPropertyChanged(nameof(PasswordError)); } }

        public ICommand RegisterCommand => new RelayCommand(
            _ => Register());

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Validation Methods
        private void ValidateFirstName() => FirstNameError = !IsValidName(FirstName) ? "First name must not be empty and only letters" : "";
        private void ValidateLastName() => LastNameError = !IsValidName(LastName) ? "Last name must not be empty and only letters" : "";
        private void ValidateUsername()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                UsernameError = "Username is required";
                return;
            }

            var forbidden = new[] { "admin", "support", "staff" };

            if (Username.Length < 3 || Username.Length > 16 ||
                char.IsDigit(Username[0]) ||
                Username.Any(char.IsWhiteSpace) ||
                Username.Any(c => !(char.IsLetterOrDigit(c) || c == '_')) ||
                forbidden.Any(w => Username.ToLower().Contains(w)))
            {
                UsernameError = "Username must be 3–16 chars, not start with digit, only a-z, A-Z, 0-9, or _, and not contain forbidden words";
                return;
            }

            bool exists = context.Users.Any(u => u.Id == Username);
            if (exists)
            {
                UsernameError = "Username already exists";
                return;
            }

            UsernameError = "";
        }
        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailError = "Email is required";
                return;
            }

            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                EmailError = "Invalid email format";
                return;
            }

            bool exists = context.Users.Any(u => u.Email == Email);
            if (exists)
            {
                EmailError = "Email already exists";
                return;
            }

            EmailError = "";
        }
        private void ValidateAddress() => AddressError = !IsValidAddress(Address) ? "Invalid characters in address" : "";
        private void ValidatePhone()
        {
            if (string.IsNullOrWhiteSpace(Phone))
            {
                PhoneError = "Phone is required";
                return;
            }

            if (!(Phone.Length == 10 && Phone.StartsWith("0") && Phone.All(char.IsDigit)))
            {
                PhoneError = "Phone must be 10 digits starting with 0";
                return;
            }

            bool exists = context.Users.Any(u => u.Phone == Phone);
            if (exists)
            {
                PhoneError = "Phone number already exists";
                return;
            }

            PhoneError = "";
        }
        private void ValidatePassword() => PasswordError = !IsValidPassword(Password) ? "Password must be 8-64 characters" : "";

        // Helper Validations
        private bool IsValidName(string name) => 
            !string.IsNullOrWhiteSpace(name) && name.All(char.IsLetter);

        private bool IsValidAddress(string address) =>
            !string.IsNullOrWhiteSpace(address) &&
            address.All(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '_');
        private bool IsValidPassword(string password) =>
            !string.IsNullOrWhiteSpace(password) &&
            password.Length >= 8 && password.Length <= 64;

        private void Register()
        {
            ValidateFirstName();
            ValidateLastName();
            ValidateUsername();
            ValidateEmail();
            ValidateAddress();
            ValidatePhone();
            ValidatePassword();

            bool hasErrors = new[] {
                FirstNameError, LastNameError, UsernameError,
                EmailError, AddressError, PhoneError, PasswordError
            }.Any(e => !string.IsNullOrEmpty(e));

            if (hasErrors)
            {
                MessageBox.Show("Please fix validation errors.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var context = new FigureManagementSystemContext();
                var user = new User
                {
                    Id = Username,
                    FullName = $"{FirstName} {LastName}",
                    Password = HashPassword(Password),
                    Email = Email,
                    RoleId = 2,
                    Address = Address,
                    Phone = Phone,
                    IsActive = true
                };

                context.Users.Add(user);
                context.SaveChanges();

                var result = MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                if (result == MessageBoxResult.OK)
                {
                    var registerWindow = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.DataContext == this);
                            registerWindow?.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    }
}
