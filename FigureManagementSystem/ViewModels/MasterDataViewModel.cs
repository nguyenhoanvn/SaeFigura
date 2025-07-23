using FigureManagementSystem.Helpers;
using FigureManagementSystem.Models;
using FigureManagementSystem.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            _window.btnBrands.Click += BtnBrands_Click;
            _window.btnCategories.Click += BtnCategories_Click;
            _window.btnRoles.Click += BtnRoles_Click;
        }

        private void BtnSeries_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = new GenericManagementViewModel<Series, int>(
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
                Owner = _window,
                ShowInTaskbar = false
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
            var seriesList = new FigureManagementSystemContext().Series.ToList();

            var viewModel = new GenericManagementViewModel<Character, int>(
                ownerWindow: Application.Current.MainWindow,
                entityName: "Characters",
                idSelector: c => c.Id,
                displayNameSelector: c => c.Name,
                searchPredicate: (c, text) => c.Name.Contains(text, StringComparison.OrdinalIgnoreCase),
                toggleStatusAction: c => c.IsActive = !(c.IsActive ?? false),
                fieldDefinitions: new List<Helpers.FieldDefinition>
                {
                    new() {Label = "Name", PropertyName = nameof(Character.Name), Type = typeof(string)},
                    new() {Label = "Main Color", PropertyName = nameof(Character.MainColor), Type = typeof(string)},
                    new() {Label = "IsActive", PropertyName = nameof(Character.IsActive), Type = typeof(bool?)},
                    new() {Label = "SeriesId", PropertyName = nameof(Character.SeriesId), Type = typeof(int) },
                }
            );
            viewModel.WindowTitle = "Characters Management Window";
            viewModel.WindowSubtitle = "Manage your Characters in database";
            viewModel.LinkedEntities = new List<LinkedEntityDefinition>
            {
                new()
                {
                    Label = "Series",
                    PropertyName = nameof(Character.SeriesId),
                    LinkedEntityType = typeof(Series),
                    ItemsSourceProvider = () => seriesList,
                    DisplayMemberSelector = s => ((Series)s).Name
                }
            };
            viewModel.ForeignKeyMappings["SeriesId"] = new ForeignKeyMapping
            {
                EntityType = typeof(Series),
                DisplayProperty = "Name"
            };

            var window = new GenericManagementWindow
            {
                DataContext = viewModel,
                Owner = _window,
                ShowInTaskbar = false
            };

            viewModel.CloseAction = () =>
            {
                window.DialogResult = true;
                window.Close();
            };

            window.ShowDialog();
        }

        private void BtnBrands_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = new GenericManagementViewModel<Brand, int>(
                ownerWindow: Application.Current.MainWindow,
                entityName: "Brands",
                idSelector: b => b.Id,
                displayNameSelector: b => b.Name,
                searchPredicate: (b, text) => b.Name.Contains(text, StringComparison.OrdinalIgnoreCase),
                toggleStatusAction: b => b.IsActive = !(b.IsActive ?? false),
                fieldDefinitions: new List<Helpers.FieldDefinition>
                {
                    new() {Label = "Name", PropertyName = nameof(Brand.Name), Type = typeof(string)},
                    new() {Label = "Average Rating", PropertyName = nameof(Brand.AverageRating), Type = typeof(decimal?)},
                    new() {Label = "Active Since", PropertyName = nameof(Brand.ActiveSince), Type = typeof(DateOnly?)},
                    new() {Label = "IsActive", PropertyName = nameof(Brand.IsActive), Type = typeof(bool?)},
                }
            );
            viewModel.WindowTitle = "Brands Management Window";
            viewModel.WindowSubtitle = "Manage your Brands in database";

            var window = new GenericManagementWindow
            {
                DataContext = viewModel,
                Owner = _window,
                ShowInTaskbar = false
            };

            viewModel.CloseAction = () =>
            {
                window.DialogResult = true;
                window.Close();
            };

            window.ShowDialog();
        }

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = new GenericManagementViewModel<Category, int>(
                ownerWindow: Application.Current.MainWindow,
                entityName: "Categories",
                idSelector: ct => ct.Id,
                displayNameSelector: ct => ct.Name,
                searchPredicate: (ct, text) => ct.Name.Contains(text, StringComparison.OrdinalIgnoreCase),
                toggleStatusAction: ct => ct.IsActive = !(ct.IsActive ?? false),
                fieldDefinitions: new List<Helpers.FieldDefinition>
                {
                    new() {Label = "Name", PropertyName = nameof(Category.Name), Type = typeof(string)},
                    new() {Label = "Average Rating", PropertyName = nameof(Category.Description), Type = typeof(string)},
                    new() {Label = "IsActive", PropertyName = nameof(Category.IsActive), Type = typeof(bool?)},
                }
            );
            viewModel.WindowTitle = "Categories Management Window";
            viewModel.WindowSubtitle = "Manage your Categories in database";

            var window = new GenericManagementWindow
            {
                DataContext = viewModel,
                Owner = _window,
                ShowInTaskbar = false
            };

            viewModel.CloseAction = () =>
            {
                window.DialogResult = true;
                window.Close();
            };

            window.ShowDialog();
        }

        private void BtnRoles_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = new GenericManagementViewModel<Role, int>(
                ownerWindow: Application.Current.MainWindow,
                entityName: "Roles",
                idSelector: r => r.Id,
                displayNameSelector: r => r.Name,
                searchPredicate: (r, text) => r.Name.Contains(text, StringComparison.OrdinalIgnoreCase),
                toggleStatusAction: r => r.IsActive = !(r.IsActive ?? false),
                fieldDefinitions: new List<Helpers.FieldDefinition>
                {
                    new() {Label = "Name", PropertyName = nameof(Role.Name), Type = typeof(string)},
                    new() {Label = "IsActive", PropertyName = nameof(Role.IsActive), Type = typeof(bool?)},
                }

            );
            viewModel.WindowTitle = "Roles Management Window";
            viewModel.WindowSubtitle = "Manage your Roles in database";

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

    }
}
