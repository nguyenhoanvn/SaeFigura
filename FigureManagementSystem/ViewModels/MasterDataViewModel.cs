using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FigureManagementSystem.Views;

namespace FigureManagementSystem.ViewModels
{
    public class MasterDataViewModel
    {
        private readonly MasterDataWindow _window;

        public MasterDataViewModel(MasterDataWindow window)
        {
            _window = window;
            _window.btnSeries.Click += BtnSeries_Click;
            _window.btnCharacters.Click += BtnCharacters_Click;

        }

        private void BtnSeries_Click(object sender, RoutedEventArgs e)
        {
            SeriesManagementWindow seriesWindow = new SeriesManagementWindow();
            _window.Hide();
            bool? back = seriesWindow.ShowDialog();

            _window.Show();
        }

        private void BtnCharacters_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnBrands_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnMaterials_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
