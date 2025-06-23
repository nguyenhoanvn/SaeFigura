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
        private ObservableCollection<TblSeries> _seriesList = new();
        private ObservableCollection<TblSeries> _filteredSeriesList = new();
        private readonly SeriesManagementWindow _window;

        public ObservableCollection<TblSeries> SeriesList
        {
            get => _filteredSeriesList;
            set { _filteredSeriesList = value; OnPropertyChanged(nameof(SeriesList)); }
        }

        private TblSeries? _selectedSeries;
        public TblSeries? SelectedSeries
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
            using var context = new ProjectContext();
            _seriesList = new ObservableCollection<TblSeries>(context.TblSeries.ToList());
            ApplyFilters();
        }

        public void ApplyFilters()
        {
            IEnumerable<TblSeries> filtered = _seriesList;

            // Search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(s => s.SeriesName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
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

            SeriesList = new ObservableCollection<TblSeries>(filtered);

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
                SelectionInfo = $"Selected {SelectedSeries.SeriesName} (ID: {SelectedSeries.SeriesId})";
            }
            OnPropertyChanged(nameof(SelectionInfo));
        }

        public void OnAddNew()
        {
            var newSeries = new TblSeries
            {
                IsActive = true
            };

            var fields = new List<Helpers.FieldDefinition>
            {
                new Helpers.FieldDefinition {Label = "Series Name", PropertyName = nameof(TblSeries.SeriesName), Type = typeof(string)},
                new Helpers.FieldDefinition {Label = "Is Active", PropertyName = nameof(TblSeries.SeriesName), Type = typeof(bool?)}
            };

            var editor = new EntityEditorWindow(newSeries, fields, "Add New Series") { Owner = Application.Current.MainWindow };
            if (editor.ShowDialog() == true && editor.IsSaved)
            {
                using (var context = new ProjectContext())
                {
                    context.TblSeries.Add(newSeries);
                    context.SaveChanges();
                    LoadSeries();
                }
            }
        }

        public void OnEditSelected()
        {
            if (SelectedSeries == null) return;

            var cloneSeries = new TblSeries
            {
                SeriesId = SelectedSeries.SeriesId,
                SeriesName = SelectedSeries.SeriesName,
                TblCharacters = SelectedSeries.TblCharacters,
                IsActive = SelectedSeries.IsActive
            };

            var fields = new List<Helpers.FieldDefinition>
            {
                new Helpers.FieldDefinition {Label = "Series Name", PropertyName = nameof(TblSeries.SeriesName), Type = typeof(string)},
                new Helpers.FieldDefinition {Label = "Is Active", PropertyName = nameof(TblSeries.SeriesName), Type = typeof(bool?)}
            };

            var editor = new EntityEditorWindow(cloneSeries, fields, $"Edit {SelectedSeries.SeriesName}") { Owner = Application.Current.MainWindow };
            if (editor.ShowDialog() == true && editor.IsSaved)
            {
                using (var context = new ProjectContext())
                {
                    var seriesToUpdate = context.TblSeries.Include(s => s.TblCharacters).FirstOrDefault(s => s.SeriesId == cloneSeries.SeriesId);
                    if (seriesToUpdate != null)
                    {
                        seriesToUpdate.SeriesName = cloneSeries.SeriesName;
                        seriesToUpdate.IsActive = cloneSeries.IsActive;
                        seriesToUpdate.TblCharacters.Clear();
                        foreach (var character in cloneSeries.TblCharacters)
                        {
                            var existingCharacter = context.TblCharacters.Find(character.CharacterId);
                            if (existingCharacter != null)
                            {
                                seriesToUpdate.TblCharacters.Add(existingCharacter);
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
            var result = MessageBox.Show($"Are you sure you want to delete {SelectedSeries.SeriesName}?", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (var context = new ProjectContext())
                {
                    var seriesToDelete = context.TblSeries.Include(s => s.TblCharacters).FirstOrDefault(s => s.SeriesId == SelectedSeries.SeriesId);
                    if (seriesToDelete != null)
                    {
                        seriesToDelete.TblCharacters.Clear();
                        context.TblSeries.Remove(seriesToDelete);
                        context.SaveChanges();
                        LoadSeries();
                    }
                }
            }
        }

        public void OnToggleStatus()
        {
            if (SelectedSeries == null) return;
            using (var context = new ProjectContext())
            {
                var series = context.TblSeries.FirstOrDefault(s => s.SeriesId == SelectedSeries.SeriesId);
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
