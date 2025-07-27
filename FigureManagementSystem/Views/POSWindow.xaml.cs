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

namespace FigureManagementSystem.Views
{
    /// <summary>
    /// Interaction logic for POSWindow.xaml
    /// </summary>
    public partial class POSWindow : FullScreenWindow
    {
        public event RoutedEventHandler Btn_AddToCart;
        public event RoutedEventHandler Btn_RemoveItem;

        public POSWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.POSViewModel(this);
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            Btn_AddToCart?.Invoke(sender, e);
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            Btn_RemoveItem?.Invoke(sender, e);
        }
    }
}
