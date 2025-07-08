using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FigureManagementSystem.Models;
using FigureManagementSystem.Views;
using Microsoft.EntityFrameworkCore;

namespace FigureManagementSystem.ViewModels
{
    public class SeriesManagementViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Series> _seriesList = new();
        private ObservableCollection<Series> _filteredSeriesList = new();
        private readonly SeriesManagementWindow _window;

        public ObservableCollection<Series> SeriesList
        {
            get => _filteredSeriesList;
            set { _filteredSeriesList = value; OnPropertyChanged(nameof(SeriesList)); }
        }

        private Series? _selectedSeries;
        public Series? SelectedSeries
        {
            get => _selectedSeries;
            set { _selectedSeries = value; OnPropertyChanged(nameof(SelectedSeries)); UpdateSelectionInfo(); }
        }

        public String SearchText { get; set; } = "";
        public String SelectedStatus { get; set; } = "All";
        public String SelectionInfo { get; set; } = "No series selected";
        public String TotalCountInfo { get; set; } = "Total: 0 series";
        public Visibility EmptyStateVisibility { get; set; } = Visibility.Collapsed;

        public ICommand AddNewCommand { get; set; }
        public ICommand EditSelectedCommand { get; set; }
        public ICommand DeleteSelectedCommand { get; set; }
        public ICommand ToggleStatusCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand ClearSearchCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public SeriesManagementViewModel(SeriesManagementWindow window)
        {
            _window = window;
            LoadSeries();
        }

        public void LoadSeries()
        {
            using var context = new FigureManagementSystemContext();
            _seriesList = new ObservableCollection<Series>(context.Series.ToList());
            ApplyFilters();
        }

        public void ApplyFilters()
        {
            IEnumerable<Series> filtered = _seriesList;

            // Search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(s => s.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            // Status filter
            if (SelectedStatus == "Active")
            {
                filtered = filtered.Where(s => s.IsActive == true);
            }
            else if (SelectedStatus == "Inactive")
            {
                filtered = filtered.Where(s => s.IsActive == false);
            }

            SeriesList = new ObservableCollection<Series>(filtered);

            // Empty state
            EmptyStateVisibility = SeriesList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged(nameof(EmptyStateVisibility));

            // Update totals
            TotalCountInfo = $"Total: {SeriesList.Count} series";
            OnPropertyChanged(nameof(TotalCountInfo));
        }

        public void OnSearchChanged()
        {
            ApplyFilters();
        }

        public void OnClearSearch()
        {
            SearchText = "";
            OnPropertyChanged(nameof(SearchText));
            ApplyFilters();
        }

        public void OnStatusFilterChanged()
        {
            ApplyFilters();
        }

        public void OnRefresh()
        {
            LoadSeries();
        }

        public void UpdateSelectionInfo()
        {
            if (SelectedSeries == null)
            {
                SelectionInfo = "No series selected";
            }
            else
            {
                SelectionInfo = $"Selected {SelectedSeries.Name} (ID: {SelectedSeries.Id})";
            }
            OnPropertyChanged(nameof(SelectionInfo));
        }

        public void OnAddNew()
        {
            var newSeries = new Series
            {
                IsActive = true
            };

            var fields = new List<Helpers.FieldDefinition>
            {
                new Helpers.FieldDefinition {Label = "Series Name", PropertyName = nameof(Series.Name), Type = typeof(string)},
                new Helpers.FieldDefinition {Label = "Is Active", PropertyName = nameof(Series.Name), Type = typeof(bool?)}
            };

            var editor = new EntityEditorWindow(newSeries, fields, "Add New Series") { Owner = Application.Current.MainWindow };
            if (editor.ShowDialog() == true && editor.IsSaved)
            {
                using (var context = new FigureManagementSystemContext())
                {
                    context.Series.Add(newSeries);
                    context.SaveChanges();
                    LoadSeries();
                }
            }
        }

        public void OnEditSelected()
        {
            if (SelectedSeries == null) return;

            var cloneSeries = new Series
            {
                Id = SelectedSeries.Id,
                Name = SelectedSeries.Name,
                Characters = SelectedSeries.Characters,
                IsActive = SelectedSeries.IsActive
            };

            var fields = new List<Helpers.FieldDefinition>
            {
                new Helpers.FieldDefinition {Label = "Series Name", PropertyName = nameof(Series.Name), Type = typeof(string)},
                new Helpers.FieldDefinition {Label = "Is Active", PropertyName = nameof(Series.Name), Type = typeof(bool?)}
            };

            var editor = new EntityEditorWindow(cloneSeries, fields, $"Edit {SelectedSeries.Name}") { Owner = Application.Current.MainWindow };
            if (editor.ShowDialog() == true && editor.IsSaved)
            {
                using (var context = new FigureManagementSystemContext())
                {
                    var seriesToUpdate = context.Series.Include(s => s.Characters).FirstOrDefault(s => s.Id == cloneSeries.Id);
                    if (seriesToUpdate != null)
                    {
                        seriesToUpdate.Name = cloneSeries.Name;
                        seriesToUpdate.IsActive = cloneSeries.IsActive;
                        seriesToUpdate.Characters.Clear();
                        foreach (var character in cloneSeries.Characters)
                        {
                            var existingCharacter = context.Characters.Find(character.Id);
                            if (existingCharacter != null)
                            {
                                seriesToUpdate.Characters.Add(existingCharacter);
                            }
                        }
                        context.SaveChanges();
                    }
                    LoadSeries();
                }
            }
        }

        public void OnDeleteSelected()
        {
            if (SelectedSeries == null) return;
            var result = MessageBox.Show($"Are you sure you want to delete {SelectedSeries.Name}?", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (var context = new FigureManagementSystemContext())
                {
                    var seriesToDelete = context.Series.Include(s => s.Characters).FirstOrDefault(s => s.Id == SelectedSeries.Id);
                    if (seriesToDelete != null)
                    {
                        seriesToDelete.Characters.Clear();
                        context.Series.Remove(seriesToDelete);
                        context.SaveChanges();
                        LoadSeries();
                    }
                }
            }
        }

        public void OnToggleStatus()
        {
            if (SelectedSeries == null) return;
            using (var context = new FigureManagementSystemContext())
            {
                var series = context.Series.FirstOrDefault(s => s.Id == SelectedSeries.Id);
                if (series != null)
                {
                    series.IsActive = !(series.IsActive ?? false);
                    context.SaveChanges();
                    LoadSeries();
                }
            }
        }
    }
}
