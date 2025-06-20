using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FigureManagementSystem.Views;

namespace FigureManagementSystem.ViewModels
{
    public class SeriesViewModel
    {
        private readonly SeriesManagementWindow _window;

        public SeriesViewModel(SeriesManagementWindow window)
        {
            _window = window;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
