using System.Windows;
using System.Windows.Input;
using FigureManagementSystem.Helpers;
using FigureManagementSystem.ViewModels;

namespace FigureManagementSystem.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel(this);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

    }
}
