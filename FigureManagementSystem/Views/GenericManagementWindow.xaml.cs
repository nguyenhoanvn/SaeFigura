﻿using FigureManagementSystem.Helpers;
using FigureManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace FigureManagementSystem.Views
{
    /// <summary>
    /// Interaction logic for GenericManagementWindow.xaml
    /// </summary>
    public partial class GenericManagementWindow : FullScreenWindow
    {
        public GenericManagementWindow()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            dataGrid.IsReadOnly = true;

            if (e.PropertyName == nameof(OrderDetail.ProductId))
            {
                e.Column.Header = "Product Name";
            }

            if (e.PropertyName == nameof(Order.UserId))
            {
                e.Column.Header = "User Full Name";
            }


            var propertyType = e.PropertyType;

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string))
            {
                e.Cancel = true; 
            }
            else if (!propertyType.IsPrimitive 
                && propertyType != typeof(string) 
                && propertyType != typeof(decimal) 
                && propertyType != typeof(DateTime)
                && propertyType != typeof(DateOnly)
                && propertyType != typeof(bool) 
                && propertyType != typeof(bool?)
                && !propertyType.IsEnum)
            {
                e.Cancel = true;
            }

            if (DataContext is IGenericForeignKeyProvider viewModel)
            {
                if (viewModel.ForeignKeyMappings.TryGetValue(e.PropertyName, out var mapping))
                {
                    var binding = new Binding(e.PropertyName)
                    {
                        Converter = new GenericForeignKeyConverter(mapping.EntityType, mapping.DisplayProperty)
                    };
                    ((DataGridTextColumn)e.Column).Binding = binding;
                }
            }
        }
    }
}
