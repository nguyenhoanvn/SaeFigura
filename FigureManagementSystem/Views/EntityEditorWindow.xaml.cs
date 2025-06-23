using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
    /// Interaction logic for EntityEditorWindow.xaml
    /// </summary>
    public partial class EntityEditorWindow : Window
    {
        private object _entity;
        private List<Helpers.FieldDefinition> _fields;
        private Dictionary<string, Control> _inputs = new();
        public bool IsSaved { get; set; } = false;
        public string WindowTitle { get; set; }
        public string Purpose { get; set; }
        public EntityEditorWindow(object entity, List<Helpers.FieldDefinition> fields, string title = "Edit Entity", string purpose = "Edit")
        {
            InitializeComponent();
            DataContext = this;

            _entity = entity;
            _fields = fields;
            WindowTitle = title;
            Purpose = purpose;

            BuildForm();
        }

        public void BuildForm()
        {
            FormPanel.Children.Clear();
            foreach (var field in _fields)
            {
                var label = new TextBlock
                {
                    Text = field.Label,
                    FontWeight = FontWeights.SemiBold,
                    Margin = new Thickness(0, 0, 0, 2)
                };
                FormPanel.Children.Add(label);

                Control input;
                object? value = _entity.GetType().GetProperty(field.PropertyName)?.GetValue(_entity);

                if (field.Type == typeof(bool) || field.Type == typeof(bool?))
                {
                    input = new CheckBox
                    {
                        IsChecked = (bool?)(value ?? false),
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                }
                else
                {
                    input = new TextBox
                    {
                        Text = value?.ToString() ?? "",
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                }

                _inputs[field.PropertyName] = input;
                FormPanel.Children.Add(input);
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            foreach (var field in _fields)
            {
                var prop = _entity.GetType().GetProperty(field.PropertyName);
                if (prop == null) continue;
                Control input = _inputs[field.PropertyName];

                try
                {
                    if (field.Type == typeof(bool) || field.Type == typeof(bool?))
                    {
                        prop.SetValue(_entity, ((CheckBox)input).IsChecked);
                    }
                    else if (field.Type == typeof(int))
                    {
                        if (int.TryParse(((TextBox)input).Text, out int intValue)) {
                            prop.SetValue(_entity, intValue);
                        } else
                        {
                            MessageBox.Show($"{field.Label} must be an integer");
                            return;
                        }
                    } 
                    else
                    {
                        prop.SetValue(_entity, ((TextBox)input).Text);
                    }
                } catch
                {
                    MessageBox.Show($"Invalid input for {field.Label}");
                    return;
                }
            }
            IsSaved = true;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsSaved = false;
            DialogResult = false;
            Close();
        }

        


    }
}
