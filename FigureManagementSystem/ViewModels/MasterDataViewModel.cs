using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FigureManagementSystem.Models;
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
            var viewModel = new GenericManagementViewModel<Series>(
                ownerWindow: Application.Current.MainWindow,
                entityName: "Series",
                idSelector: s => s.Id,
                displayNameSelector: s => s.Name,
                searchPredicate: (s, text) => s.Name.Contains(text, StringComparison.OrdinalIgnoreCase),
                toggleStatusAction: s => s.IsActive = !(s.IsActive ?? false),
                fieldDefinitions: new List<Helpers.FieldDefinition>
                {
                    new() {Label = "Name", PropertyName = nameof(Series.Name), Type = typeof(string)},
                    new() {Label = "IsActive", PropertyName = nameof(Series.IsActive), Type = typeof(bool?)},
                }
            );
            viewModel.WindowTitle = "Series Management Window";
            viewModel.WindowSubtitle = "Manage your Series in database";

            var window = new GenericManagementWindow
            {
                DataContext = viewModel,
                Owner = _window 
            };

            viewModel.CloseAction = () =>
            {
                window.DialogResult = true;
                window.Close();
            };

            window.ShowDialog();
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
