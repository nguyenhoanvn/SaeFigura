using FigureManagementSystem.Models;
using FigureManagementSystem.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FigureManagementSystem.Helpers;
using System.Windows;
using System.Windows.Input;

namespace FigureManagementSystem.ViewModels
{
    public class GenericManagementViewModel<TEntity> : INotifyPropertyChanged where TEntity : class, new()
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public Action? CloseAction { get; set; }
        protected void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        public ObservableCollection<TEntity> Entities { get; set; } = new();
        public ObservableCollection<TEntity> FilteredEntities { get; set; } = new();
        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string> { "All", "Active", "Inactive" };
        public TEntity? SelectedEntity { get; set; }
        public string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    ApplyFilters();
                }
            }
        }

        public string _selectedStatus = "All";
        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    OnPropertyChanged(nameof(SelectedStatus));
                    ApplyFilters();
                }
            }
        }
        public string TotalCountInfo { get; set; } = "";
        public string WindowTitle { get; set; } = "";
        public string WindowSubtitle { get; set; } = "";
        public string EmptyStateTitle => $"No {_entityName} found";
        public string EmptyStateSubtitle => $"Click 'Add New {_entityName}' to get started";
        public Visibility EmptyStateVisibility { get; set; } = Visibility.Collapsed;
        
        private readonly Func<TEntity, int> _idSelector;
        private readonly Func<TEntity, string> _displayNameSelector;
        private readonly Func<TEntity, string, bool> _searchPredicate;
        private readonly Action<TEntity> _toggleStatusAction;
        private readonly List<Helpers.FieldDefinition> _fieldDefinitions;
        private readonly string _entityName;

        private readonly Window _ownerWindow;

        public ICommand BackCommand { get; set; }
        public ICommand ClearSearchCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand AddNewCommand { get; set; }
        public ICommand EditSelectedCommand { get; set; }
        public ICommand DeleteSelectedCommand { get; set; }
        public ICommand ToggleStatusCommand { get; set; }


        public GenericManagementViewModel (
            Window ownerWindow,
            string entityName,
            Func<TEntity, int> idSelector,
            Func<TEntity, string> displayNameSelector,
            Func<TEntity, string, bool> searchPredicate,
            Action<TEntity> toggleStatusAction,
            List<Helpers.FieldDefinition> fieldDefinitions)
        {
            _ownerWindow = ownerWindow;
            _entityName = entityName;
            _idSelector = idSelector;
            _displayNameSelector = displayNameSelector;
            _searchPredicate = searchPredicate;
            _toggleStatusAction = toggleStatusAction;
            _fieldDefinitions = fieldDefinitions;

            BackCommand = new RelayCommand(_ =>
            {
                CloseAction?.Invoke();
            });
            ClearSearchCommand = new RelayCommand(_ =>
            {
                SearchText = "";
            });
            RefreshCommand = new RelayCommand(_ => LoadEntities());
            AddNewCommand = new RelayCommand(_ => OnAddNew());
            EditSelectedCommand = new RelayCommand(_ => OnEditSelected());
            DeleteSelectedCommand = new RelayCommand(_ => OnDeleteSelected());
            ToggleStatusCommand = new RelayCommand(_ => OnToggleStatus());

            LoadEntities();
        }
        public void LoadEntities()
        {
            using (var context = new FigureManagementSystemContext())
            {
                Entities = new ObservableCollection<TEntity>(context.Set<TEntity>().ToList());
                ApplyFilters();
            }
        }

        public void ApplyFilters()
        {
            IEnumerable<TEntity> filtered = Entities;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(e => _searchPredicate(e, SearchText));
            }

            if (SelectedStatus == "Active")
            {
                filtered = filtered.Where(e =>
                {
                    var prop = typeof(TEntity).GetProperty("IsActive");
                    return prop != null && (prop.GetValue(e) as bool?) == true;
                });
            }
            else if (SelectedStatus == "Inactive")
            {
                filtered = filtered.Where(e =>
                {
                    var prop = typeof(TEntity).GetProperty("IsActive");
                    return prop != null && (prop.GetValue(e) as bool?) == false;
                });
            }

            FilteredEntities = new ObservableCollection<TEntity>(filtered);

            EmptyStateVisibility = FilteredEntities.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;
            TotalCountInfo = $"Total: {FilteredEntities.Count} {_entityName}";
            OnPropertyChanged(nameof(FilteredEntities));
            OnPropertyChanged(nameof(TotalCountInfo));
            OnPropertyChanged(nameof(EmptyStateVisibility));
        }

        public void OnAddNew()
        {
            var newEntity = new TEntity();
            var editor = new EntityEditorWindow(newEntity, _fieldDefinitions, $"Add New {_entityName}", "Add", false, null)
            {
                Owner = _ownerWindow
            };

            if (editor.ShowDialog() == true && editor.IsSaved)
            {
                using (var context = new FigureManagementSystemContext())
                {
                    context.Set<TEntity>().Add(newEntity);
                    context.SaveChanges();
                    LoadEntities();
                }
            }
        }

        public void OnEditSelected()
        {
            if (SelectedEntity == null)
            {
                return;
            }
            var clone = new TEntity();
            foreach (var field in _fieldDefinitions)
            {
                var prop = typeof(TEntity).GetProperty(field.PropertyName);
                if (prop != null)
                {
                    var value = prop.GetValue(SelectedEntity);
                    prop.SetValue(clone, value);
                }
            }

            var editor = new EntityEditorWindow(clone, _fieldDefinitions, $"Edit {_displayNameSelector(SelectedEntity)}", "Edit")
            {
                Owner = _ownerWindow
            };
            if (editor.ShowDialog() == true && editor.IsSaved)
            {
                using var context = new FigureManagementSystemContext();
                var entity = context.Set<TEntity>().Find(_idSelector(SelectedEntity));
                if (entity != null)
                {
                    foreach (var field in _fieldDefinitions)
                    {
                        var prop = typeof(TEntity).GetProperty(field.PropertyName);
                        if (prop != null)
                        {
                            var value = prop.GetValue(clone);
                            prop.SetValue(entity, value);
                        } 
                    }
                    context.SaveChanges();
                    LoadEntities();
                }
            }
        }

        public void OnDeleteSelected()
        {
            if (SelectedEntity == null)
            {
                return;
            }
            var result = MessageBox.Show($"Delete {_displayNameSelector(SelectedEntity)}?", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using var context = new FigureManagementSystemContext();
                var entity = context.Set<TEntity>().Find(_idSelector(SelectedEntity));
                if (entity != null)
                {
                    context.Set<TEntity>().Remove(entity);
                    context.SaveChanges();
                    LoadEntities();
                }
            }
        }

        public void OnToggleStatus()
        {
            if (SelectedEntity == null)
            {
                return;
            }
            using var context = new FigureManagementSystemContext();
            var entity = context.Set<TEntity>().Find(_idSelector(SelectedEntity));
            if (entity != null)
            {
                _toggleStatusAction(entity);
                context.SaveChanges();
                LoadEntities();
            }
        }

    }
}
