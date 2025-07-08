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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FigureManagementSystem.Fragments
{
    /// <summary>
    /// Interaction logic for FooterControl.xaml
    /// </summary>
    public partial class FooterControl : UserControl
    {
        public FooterControl()
        {
            InitializeComponent();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); 
        }
    }
}
