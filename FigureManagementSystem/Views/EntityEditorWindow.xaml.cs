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
        public object Entity { get; set; }
        public List<Helpers.FieldDefinition> Fields { get; set; }
        public bool IsSaved { get; set; }
        public string WindowTitle { get; set; }
        public string Purpose { get; set; }
        public bool HasLinkedEntities { get; set; }
        public List<LinkedEntityDefinition> LinkedEntities { get; set; } = new();
        public EntityEditorWindow(
            object entity,
            List<Helpers.FieldDefinition> fields,
            string title = "Edit Entity",
            string purpose = "Edit",
            bool hasLinkedEntities = false,
            List<LinkedEntityDefinition> linkedEntities = null)
        {
            InitializeComponent();
            DataContext = this;
            Entity = entity;
            Fields = fields;
            WindowTitle = title;
            Purpose = purpose;
            HasLinkedEntities = hasLinkedEntities;
            LinkedEntities = linkedEntities ?? new();

            WindowStyle = WindowStyle.SingleBorderWindow;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;

            BuildForm();
        }

        private void BuildForm()
        {
            foreach (var field in Fields)
            {
                var label = new Label { Content = field.Label };
                Control input;

                if (field.Type == typeof(bool?) || field.Type == typeof(bool))
                {
                    var checkBox = new CheckBox();
                    var value = GetPropertyValue(field.PropertyName) as bool?;
                    checkBox.IsChecked = value;
                    checkBox.Checked += (_, __) => SetPropertyValue(field.PropertyName, true);
                    checkBox.Unchecked += (_, __) => SetPropertyValue(field.PropertyName, false);
                    input = checkBox;
                }
                else if (field.Type.IsEnum)
                {
                    var comboBox = new ComboBox
                    {
                        ItemsSource = Enum.GetValues(field.Type),
                        SelectedItem = GetPropertyValue(field.PropertyName)
                    };

                    var binding = new Binding(field.PropertyName)
                    {
                        Source = Entity,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                    comboBox.SetBinding(ComboBox.SelectedItemProperty, binding);

                    input = comboBox;
                }
                else if (field.Type == typeof(int) || field.Type == typeof(int?))
                {
                    var textBox = new TextBox();
                    var binding = new Binding(field.PropertyName)
                    {
                        Source = Entity,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        ValidatesOnDataErrors = true,
                        NotifyOnValidationError = true,
                        Converter = new IntConverter() 
                    };
                    textBox.SetBinding(TextBox.TextProperty, binding);

                    textBox.PreviewTextInput += (s, e) => e.Handled = !IsTextInteger(e.Text);
                    DataObject.AddPastingHandler(textBox, (s, e) =>
                    {
                        if (e.DataObject.GetDataPresent(DataFormats.Text))
                        {
                            string text = (string)e.DataObject.GetData(DataFormats.Text);
                            if (!IsTextInteger(text))
                                e.CancelCommand();
                        }
                    });

                    if ((field.PropertyName.Equals("Id", StringComparison.OrdinalIgnoreCase)
                    || field.PropertyName.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                    && Purpose == "Edit")
                        textBox.IsReadOnly = false;

                    if (field.IsReadOnly)
                    {
                        textBox.IsReadOnly = true;
                        textBox.Background = Brushes.LightGray; // optional
                    }
                    input = textBox;

                }
                else if (field.Type == typeof(decimal) || field.Type == typeof(decimal?))
                {
                    var textBox = new TextBox();
                    var binding = new Binding(field.PropertyName)
                    {
                        Source = Entity,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        ValidatesOnDataErrors = true,
                        NotifyOnValidationError = true,
                        Converter = new DecimalConverter() // Optional: custom converter
                    };
                    textBox.SetBinding(TextBox.TextProperty, binding);

                    textBox.PreviewTextInput += (s, e) => e.Handled = !IsTextDecimal(e.Text);
                    DataObject.AddPastingHandler(textBox, (s, e) =>
                    {
                        if (e.DataObject.GetDataPresent(DataFormats.Text))
                        {
                            string text = (string)e.DataObject.GetData(DataFormats.Text);
                            if (!IsTextDecimal(text))
                                e.CancelCommand();
                        }
                    });

                    if (field.IsReadOnly)
                    {
                        textBox.IsReadOnly = true;
                        textBox.Background = Brushes.LightGray; // optional
                    }

                    input = textBox;
                }
                else if (field.Type == typeof(DateOnly) || field.Type == typeof(DateOnly?))
                {
                    var datePicker = new DatePicker();

                    var binding = new Binding(field.PropertyName)
                    {
                        Source = Entity,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Converter = new DateOnlyConverter()
                    };

                    datePicker.SetBinding(DatePicker.SelectedDateProperty, binding);
                    input = datePicker;
                }
                else
                {
                    var textBox = new TextBox();
                    var binding = new Binding(field.PropertyName)
                    {
                        Source = Entity,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.Explicit
                    };
                    textBox.SetBinding(TextBox.TextProperty, binding);

                    if ((field.PropertyName.Equals("Id", StringComparison.OrdinalIgnoreCase)
                    || field.PropertyName.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                    && Purpose == "Edit"
                    && field.Type == typeof(int))
                        textBox.IsEnabled = false;

                    if (field.IsReadOnly)
                    {
                        textBox.IsReadOnly = true;
                        textBox.Background = Brushes.LightGray; // optional
                    }

                    input = textBox;
                }

                FormPanel.Children.Add(label);
                FormPanel.Children.Add(input);
            }

            // Linked entities (ComboBoxes)
            if (HasLinkedEntities)
            {
                foreach (var linked in LinkedEntities)
                {
                    var label = new Label { Content = linked.Label };
                    var comboBox = new ComboBox
                    {
                        ItemsSource = linked.ItemsSourceProvider(),
                        DisplayMemberPath = linked.DisplayMemberPath,
                        SelectedValuePath = "Id"
                    };

                    comboBox.SetBinding(ComboBox.SelectedValueProperty, new Binding
                    {
                        Source = Entity,
                        Path = new PropertyPath(linked.PropertyName),
                        Mode = BindingMode.TwoWay
                    });

                    FormPanel.Children.Add(new Label { Content = linked.Label });
                    FormPanel.Children.Add(comboBox);
                }
            }
        }
        private object? GetPropertyValue(string propName) =>
            Entity.GetType().GetProperty(propName)?.GetValue(Entity);

        private void SetPropertyValue(string propName, object? value)
        {
            var prop = Entity.GetType().GetProperty(propName);
            if (prop != null && prop.CanWrite)
                prop.SetValue(Entity, value);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in FormPanel.Children)
            {
                if (child is TextBox textBox)
                {
                    if (string.IsNullOrWhiteSpace(textBox.Text.Trim()))
                    {
                        MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    binding?.UpdateSource();

                    var prop = Entity.GetType().GetProperty(binding?.ParentBinding?.Path?.Path ?? "");
                    if (prop != null && prop.CanWrite && prop.PropertyType == typeof(string))
                    {
                        var currentValue = prop.GetValue(Entity) as string;
                        if (currentValue != null)
                        {
                            prop.SetValue(Entity, currentValue.Trim());
                        }
                    }
                }
                else if (child is CheckBox checkBox)
                {
                    BindingExpression binding = checkBox.GetBindingExpression(CheckBox.IsCheckedProperty);
                    binding?.UpdateSource();
                }
                else if (child is ComboBox comboBox)
                {
                    BindingExpression binding = comboBox.GetBindingExpression(ComboBox.SelectedValueProperty);
                    binding?.UpdateSource();
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

        private bool IsTextInteger(string text)
        {
            return int.TryParse(text, out _);
        }

        private bool IsTextDecimal(string text)
        {
            return decimal.TryParse(text, out _);
        }
    }
}
