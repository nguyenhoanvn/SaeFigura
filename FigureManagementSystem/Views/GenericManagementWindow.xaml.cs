﻿using FigureManagementSystem.Helpers;
using FigureManagementSystem.Models;
using FigureManagementSystem.ViewModels;
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

        private bool detailsColumnAdded = false;

        private DataTemplate CreateDetailsButtonTemplate()
        {
            string xaml = @"
            <DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
                <Button Content='Details' Padding='6,2' Margin='4,0' Command='{Binding DataContext.ShowOrderDetailsCommand, RelativeSource={RelativeSource AncestorType=DataGrid}}' CommandParameter='{Binding}'/>
            </DataTemplate>";
            return (DataTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
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
                e.Column.Header = "Staff Issued";
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

            /*if (DataContext is GenericManagementViewModel<Order, int>) // or another suitable last property
            {
                dataGrid.AutoGeneratedColumns += (s2, e2) =>
                {
                    if (dataGrid.Columns
                        .FirstOrDefault(c => c.Header != null && c.Header.ToString() == "Details") == null)
                    {
                        var detailsColumn = new DataGridTemplateColumn
                        {
                            Header = "Details",
                            CellTemplate = CreateDetailsButtonTemplate()
                        };
                        dataGrid.Columns.Add(detailsColumn);
                        detailsColumnAdded = true;
                    }
                };
            }*/
        }
    }
}
