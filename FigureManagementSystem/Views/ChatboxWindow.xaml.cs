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
using FigureManagementSystem.Helpers;
using FigureManagementSystem.ViewModels;

namespace FigureManagementSystem.Views
{
    /// <summary>
    /// Interaction logic for ChatboxWindow.xaml
    /// </summary>
    public partial class ChatboxWindow : FullScreenWindow
    {
        public ChatboxWindow()
        {
            InitializeComponent();
            this.DataContext = new ChatboxViewModel();
        }
    }
}
