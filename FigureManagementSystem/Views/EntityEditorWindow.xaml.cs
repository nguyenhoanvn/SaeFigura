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
                else
                {
                    var textBox = new TextBox();
                    textBox.Text = GetPropertyValue(field.PropertyName)?.ToString() ?? "";
                    textBox.TextChanged += (_, __) =>
                        SetPropertyValue(field.PropertyName, Convert.ChangeType(textBox.Text, field.Type));

                    if (field.PropertyName.ToLower().Contains("id") && Purpose == "Edit")
                        textBox.IsEnabled = false;

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
