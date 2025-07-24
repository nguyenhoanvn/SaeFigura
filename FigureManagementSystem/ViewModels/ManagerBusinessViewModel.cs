using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FigureManagementSystem.Helpers;
using FigureManagementSystem.Models;
using FigureManagementSystem.Views;

namespace FigureManagementSystem.ViewModels
{
    public class ManagerBusinessViewModel
    {
        private readonly ManagerBusinessWindow _window;
        public ManagerBusinessViewModel(ManagerBusinessWindow window)
        {
            _window = window;
            _window.btnDiscounts.Click += btnDiscounts_Click;
            _window.btnPayments.Click += btnPayments_Click;
            _window.btnOrders.Click += btnOrders_Click;
            _window.btnDetails.Click += btnDetails_Click;
            _window.btnUsers.Click += btnUsers_Click;
        }

        private void btnDiscounts_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = new GenericManagementViewModel<Discount, string>(
                ownerWindow: Application.Current.MainWindow,
                entityName: "Discount",
                idSelector: d => d.Id,
                displayNameSelector: s => s.Name,
                searchPredicate: (s, text) => s.Name.Contains(text, StringComparison.OrdinalIgnoreCase),
                toggleStatusAction: s => s.IsActive = !(s.IsActive ?? false),
                fieldDefinitions: new List<Helpers.FieldDefinition>
                {
                    new() {Label = "Name", PropertyName = nameof(Discount.Name), Type = typeof(string)},
                    new() {Label = "Activate Date", PropertyName = nameof(Discount.ActivateDate), Type = typeof(DateOnly)},
                    new() {Label = "Expire Date", PropertyName = nameof(Discount.ExpireDate), Type = typeof(DateOnly)},
                    new() {Label = "Type", PropertyName = nameof(Discount.Type), Type = typeof(string)},
                    new() {Label = "Value", PropertyName = nameof(Discount.Value), Type = typeof(decimal)},
                    new() {Label = "IsActive", PropertyName = nameof(Discount.IsActive), Type = typeof(bool?)},
                }
            );
            viewModel.WindowTitle = "Discount Management Window";
            viewModel.WindowSubtitle = "Manage your Discounts in database";

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

        private void btnPayments_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = new GenericManagementViewModel<Payment, int>(
                ownerWindow: Application.Current.MainWindow,
                entityName: "Payment",
                idSelector: p => p.Id,
                displayNameSelector: p => string.Join(" - ", p.Id, p.OrderId),
                searchPredicate: (p, text) => string.Join(" - ", p.Id, p.OrderId).Contains(text, StringComparison.OrdinalIgnoreCase),
                toggleStatusAction: p => p.IsActive = !(p.IsActive ?? false),
                fieldDefinitions: new List<Helpers.FieldDefinition>
                {
                    new() {Label = "OrderId", PropertyName = nameof(Payment.OrderId), Type = typeof(int)},
                    new() {Label = "Method", PropertyName = nameof(Payment.Method), Type = typeof(string)},
                    new() {Label = "Amount", PropertyName = nameof(Payment.Amount), Type = typeof(decimal)},
                    new() {Label = "Status", PropertyName = nameof(Payment.Status), Type = typeof(string)},
                    new() {Label = "PaymentDate", PropertyName = nameof(Payment.PaymentDate), Type = typeof(DateOnly)},
                    new() {Label = "IsActive", PropertyName = nameof(Payment.IsActive), Type = typeof(bool?)},
                }
            );
            viewModel.WindowTitle = "Payment Management Window";
            viewModel.WindowSubtitle = "Manage your Payment in database";

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

        private void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            var context = new FigureManagementSystemContext();
            var userList = context.Users.ToList();
            var discountList = context.Discounts.ToList();

            var viewModel = new GenericManagementViewModel<Order, int>(
                ownerWindow: Application.Current.MainWindow,
                entityName: "Order",
                idSelector: o => o.Id,
                displayNameSelector: o => string.Join(" - ", o.UserId, o.OrderDate),
                searchPredicate: null,
                toggleStatusAction: o => o.IsActive = !(o.IsActive ?? false),
                fieldDefinitions: new List<Helpers.FieldDefinition>
                {
                    new() {Label = "OrderDate", PropertyName = nameof(Order.OrderDate), Type = typeof(DateOnly)},
                    new() {Label = "Total", PropertyName = nameof(Order.Total), Type = typeof(decimal)},
                    new() {Label = "Users", PropertyName = nameof(Order.UserId), Type = typeof(string)},
                    new() {Label = "DiscountId", PropertyName = nameof(Order.DiscountId), Type = typeof(string)},
                    new() {Label = "IsActive", PropertyName = nameof(Order.IsActive), Type = typeof(bool?)},
                }
            );
            viewModel.WindowTitle = "Orders Management Window";
            viewModel.WindowSubtitle = "Manage your Orders in database";
            viewModel.LinkedEntities = new List<LinkedEntityDefinition>
            {
                new()
                {
                    Label = "Users",
                    PropertyName = nameof(Order.UserId),
                    LinkedEntityType = typeof(User),
                    DisplayMemberPath = "FullName",
                    ItemsSourceProvider = () => userList,
                    DisplayMemberSelector = u => ((User)u).Id
                },

                new()
                {
                    Label = "Discount",
                    PropertyName = nameof(Order.DiscountId),
                    LinkedEntityType = typeof(Discount),
                    ItemsSourceProvider = () => discountList,
                    DisplayMemberSelector = d => ((Discount)d).Id
                }
            };
            viewModel.ForeignKeyMappings["UserId"] = new ForeignKeyMapping
            {
                EntityType = typeof(User),
                DisplayProperty = "Id"
            };
            viewModel.ForeignKeyMappings["DiscountId"] = new ForeignKeyMapping
            {
                EntityType = typeof(Discount),
                DisplayProperty = "Id"
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

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            var orderList = new FigureManagementSystemContext().Orders.ToList();
            var productList = new FigureManagementSystemContext().Products.ToList();

            var viewModel = new GenericManagementViewModel<OrderDetail, int>(
                ownerWindow: Application.Current.MainWindow,
                entityName: "OrderDetail",
                idSelector: od => od.Id,
                displayNameSelector: od => string.Join(" - ", od.Product.Name, od.Quantity),
                searchPredicate: null,
                toggleStatusAction: od => od.IsActive = !(od.IsActive ?? false),
                fieldDefinitions: new List<Helpers.FieldDefinition>
                {
                    new() {Label = "OrderId", PropertyName = nameof(OrderDetail.OrderId), Type = typeof(int)},
                    new() {Label = "ProductId", PropertyName = nameof(OrderDetail.ProductName), Type = typeof(int)},
                    new() {Label = "Price", PropertyName = nameof(OrderDetail.Price), Type = typeof(decimal)},
                    new() {Label = "Quantity", PropertyName = nameof(OrderDetail.Quantity), Type = typeof(int)},
                    new() {Label = "IsActive", PropertyName = nameof(OrderDetail.IsActive), Type = typeof(bool?)},
                }
            );
            viewModel.WindowTitle = "Order Details Management Window";
            viewModel.WindowSubtitle = "Manage your Order Details in database";
            viewModel.LinkedEntities = new List<LinkedEntityDefinition>
            {
                new()
                {
                    Label = "Order",
                    PropertyName = nameof(OrderDetail.OrderId),
                    DisplayMemberPath = "Id",
                    LinkedEntityType = typeof(Order),
                    ItemsSourceProvider = () => orderList,
                    DisplayMemberSelector = o => ((Order)o).Id.ToString()
                },

                new()
                {
                    Label = "Product",
                    PropertyName = nameof(OrderDetail.ProductId),
                    LinkedEntityType = typeof(Product),
                    ItemsSourceProvider = () => productList,
                    DisplayMemberSelector = p => ((Product)p).Name
                }
            };
            viewModel.ForeignKeyMappings["OrderId"] = new ForeignKeyMapping
            {
                EntityType = typeof(Order),
                DisplayProperty = "Id"
            };
            viewModel.ForeignKeyMappings["ProductId"] = new ForeignKeyMapping
            {
                EntityType = typeof(Product),
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

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            var rolesList = new FigureManagementSystemContext().Users.ToList();
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
            viewModel.LinkedEntities = new List<LinkedEntityDefinition>
            {
                new()
                {
                    Label = "Roles",
                    PropertyName = nameof(User.RoleId),
                    LinkedEntityType = typeof(Role),
                    ItemsSourceProvider = () => rolesList,
                    DisplayMemberSelector = r => ((Role)r).Name
                }
            };
            viewModel.ForeignKeyMappings["Role Id"] = new ForeignKeyMapping
            {
                EntityType = typeof(Role),
                DisplayProperty = "Name"
            };

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
