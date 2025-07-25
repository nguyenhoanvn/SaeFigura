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
using Microsoft.Data.SqlClient;

namespace FigureManagementSystem.ViewModels
{
    public class GenericManagementViewModel<TEntity, TKey> : INotifyPropertyChanged, IGenericForeignKeyProvider where TEntity : class, new()
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public Action? CloseAction { get; set; }
        protected void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        public ObservableCollection<TEntity> Entities { get; set; } = new();
        public ObservableCollection<TEntity> FilteredEntities { get; set; } = new();
        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string> { "All", "Active", "Inactive" };
        private List<LinkedEntityDefinition>? _linkedEntities;
        public List<LinkedEntityDefinition>? LinkedEntities {
            get => _linkedEntities;
            set
            {
                _linkedEntities = value;
                InitializeFilters();
            }
        }
        public Dictionary<string, ForeignKeyMapping> ForeignKeyMappings { get; } = new();
        public Dictionary<string, IEnumerable<object>> LinkedEntitySources
            => LinkedEntities?.ToDictionary(
                x => x.PropertyName,
                x => x.ItemsSourceProvider.Invoke()) ?? new();
        public Dictionary<string, object?> SelectedFilters { get; set; } = new();
        public ObservableCollection<FilterItem> FilterItems { get; } = new();


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
        public bool HasSearch => _searchPredicate != null;
        public Visibility EmptyStateVisibility { get; set; } = Visibility.Collapsed;
        
        private readonly Func<TEntity, TKey> _idSelector;
        private readonly Func<TEntity, string> _displayNameSelector;
        private readonly Func<TEntity, string, bool>? _searchPredicate = null;
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
            Func<TEntity, TKey> idSelector,
            Func<TEntity, string> displayNameSelector,
            Action<TEntity> toggleStatusAction,
            List<Helpers.FieldDefinition> fieldDefinitions,
            Func<TEntity, string, bool>? searchPredicate = null)
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

            RefreshCommand = new RelayCommand(_ =>
            {
                LoadEntities();

                foreach (var filterItem in FilterItems)
                {
                    filterItem.SelectedFilterValue = null;
                }

                ApplyFilters();
            });
            AddNewCommand = new RelayCommand(_ => OnAddNew());
            EditSelectedCommand = new RelayCommand(_ => OnEditSelected());
            DeleteSelectedCommand = new RelayCommand(_ => OnDeleteSelected());
            ToggleStatusCommand = new RelayCommand(_ => OnToggleStatus());

            LoadEntities();
        }
        public void LoadEntities()
        {
            try
            {
                using var context = new FigureManagementSystemContext();
                Entities = new ObservableCollection<TEntity>(context.Set<TEntity>().ToList());
                ApplyFilters();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Database error occurred while loading data:\n" + sqlEx.Message, "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ApplyFilters()
        {
            IEnumerable<TEntity> filtered = Entities;

            if (!string.IsNullOrWhiteSpace(SearchText) && _searchPredicate != null)
            {
                filtered = filtered.Where(entity => _searchPredicate(entity, SearchText));
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

            foreach (var filter in SelectedFilters)
            {
                var prop = typeof(TEntity).GetProperty(filter.Key);
                if (prop != null && filter.Value != null)
                {
                    filtered = filtered.Where(e =>
                    {
                        var entityValue = prop.GetValue(e);
                        return Equals(entityValue, filter.Value);
                    });
                }
            }

            FilteredEntities = new ObservableCollection<TEntity>(filtered);

            EmptyStateVisibility = FilteredEntities.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;
            TotalCountInfo = $"Total: {FilteredEntities.Count} {_entityName}";
            OnPropertyChanged(nameof(FilteredEntities));
            OnPropertyChanged(nameof(TotalCountInfo));
            OnPropertyChanged(nameof(EmptyStateVisibility));
        }

        private void InitializeFilters()
        {
            if (LinkedEntities != null)
            {
                foreach (var linked in LinkedEntities)
                {
                    var filterItem = new FilterItem
                    {
                        PropertyName = linked.PropertyName,
                        Label = linked.Label,
                        DisplayMemberPath = linked.DisplayMemberPath,
                        SelectedFilterValue = SelectedFilters.ContainsKey(linked.PropertyName) ? SelectedFilters[linked.PropertyName] : null
                    };

                    filterItem.PropertyChanged += (_, __) =>
                    {
                        SelectedFilters[filterItem.PropertyName] = filterItem.SelectedFilterValue;
                        ApplyFilters();
                    };

                    FilterItems.Add(filterItem);
                }
            }
        }

        public void OnAddNew()
        {
            var newEntity = new TEntity();
            var editor = new EntityEditorWindow(
                newEntity,
                _fieldDefinitions,
                $"Add New {_entityName}",
                "Add",
                hasLinkedEntities: LinkedEntities != null && LinkedEntities.Any(),
                linkedEntities: LinkedEntities)
            {
                Owner = _ownerWindow
            };

            if (editor.ShowDialog() == true && editor.IsSaved)
            {
                try
                {
                    using var context = new FigureManagementSystemContext();
                    context.Set<TEntity>().Add(newEntity);
                    context.SaveChanges();
                    LoadEntities();
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show("Failed to add the entity due to a database error:\n" + sqlEx.Message, "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unexpected error occurred while adding the entity:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void OnEditSelected()
        {
            if (SelectedEntity == null)
            {
                MessageBox.Show("Please select an entity to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var clone = new TEntity();
            CopyEntityProperties(SelectedEntity, clone);

            var editor = new EntityEditorWindow(
                clone,
                _fieldDefinitions,
                $"Edit {_displayNameSelector(SelectedEntity)}",
                "Edit",
                hasLinkedEntities: LinkedEntities != null && LinkedEntities.Any(),
                linkedEntities: LinkedEntities)
            {
                Owner = _ownerWindow
            };

            if (editor.ShowDialog() == true && editor.IsSaved)
            {
                try
                {
                    using var context = new FigureManagementSystemContext();
                    var entity = context.Set<TEntity>().Find(_idSelector(SelectedEntity));
                    if (entity != null)
                    {
                        CopyEntityProperties(clone, entity);
                        context.SaveChanges();
                        LoadEntities();
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show("Failed to edit the entity due to a database error:\n" + sqlEx.Message, "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unexpected error occurred while editing the entity:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        public void OnDeleteSelected()
        {
            if (SelectedEntity == null) return;

            var result = MessageBox.Show($"Delete {_displayNameSelector(SelectedEntity)}?", "Confirm", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
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
                catch (SqlException sqlEx)
                {
                    MessageBox.Show("Failed to delete the entity due to a database error:\n" + sqlEx.Message, "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unexpected error occurred while deleting the entity:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        public void OnToggleStatus()
        {
            if (SelectedEntity == null) return;

            try
            {
                using var context = new FigureManagementSystemContext();
                var entity = context.Set<TEntity>().Find(_idSelector(SelectedEntity));
                if (entity != null)
                {
                    _toggleStatusAction(entity);
                    context.SaveChanges();
                    LoadEntities();
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Failed to update status due to a database error:\n" + sqlEx.Message, "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred while toggling status:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CopyEntityProperties(TEntity source, TEntity target)
        {
            foreach (var field in _fieldDefinitions)
            {
                var prop = typeof(TEntity).GetProperty(field.PropertyName);
                if (prop != null)
                {
                    var value = prop.GetValue(source);
                    prop.SetValue(target, value);
                }
            }

            if (LinkedEntities != null)
            {
                foreach (var linked in LinkedEntities)
                {
                    var prop = typeof(TEntity).GetProperty(linked.PropertyName);
                    if (prop != null)
                    {
                        var value = prop.GetValue(source);
                        prop.SetValue(target, value);
                    }
                }
            }

            var idProp = typeof(TEntity).GetProperty("Id");
            if (idProp != null && idProp.CanWrite && idProp.PropertyType == typeof(int))
            {
                var idValue = idProp.GetValue(source);
                idProp.SetValue(target, idValue);
            }
        }

    }
}
