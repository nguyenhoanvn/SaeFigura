using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FigureManagementSystem.Helpers;
using FigureManagementSystem.Models;
using FigureManagementSystem.Views;
using Microsoft.EntityFrameworkCore;

namespace FigureManagementSystem.ViewModels
{
    public class ManagerBusinessViewModel
    {
        private readonly ManagerBusinessWindow _window;
        public ManagerBusinessViewModel(ManagerBusinessWindow window)
        {
            _window = window;
            _window.btnDiscounts.Click += btnDiscounts_Click;
/*            _window.btnPayments.Click += btnPayments_Click;*/
            _window.btnOrders.Click += btnOrders_Click;
            _window.btnDetails.Click += btnDetails_Click;
            _window.btnUsers.Click += btnUsers_Click;
            LogoutCommand = new RelayCommand(_ => Logout());
            OpenChatboxCommand = new RelayCommand(_ => OpenChatbox());
        }

        public ICommand LogoutCommand { get; }
        public ICommand OpenChatboxCommand { get; }
        private void Logout()
        {

            var loginWindow = new LoginWindow();
            Application.Current.MainWindow = loginWindow;
            loginWindow.Show();

            _window.Close();
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
                    new() {Label = "Discount ID", PropertyName = nameof(Discount.Id), Type = typeof(string)},
                    new() {Label = "Name", PropertyName = nameof(Discount.Name), Type = typeof(string)},
                    new() {Label = "Activate Date", PropertyName = nameof(Discount.ActivateDate), Type = typeof(DateOnly)},
                    new() {Label = "Expire Date", PropertyName = nameof(Discount.ExpireDate), Type = typeof(DateOnly)},
                    new() {Label = "Type", PropertyName = nameof(Discount.Type), Type = typeof(DiscountType) },
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

        /*private void btnPayments_Click(object sender, RoutedEventArgs e)
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
                    new() {Label = "Method", PropertyName = nameof(Payment.Method), Type = typeof(PaymentMethod)},
                    new() {Label = "Amount", PropertyName = nameof(Payment.Amount), Type = typeof(decimal)},
                    new() {Label = "Status", PropertyName = nameof(Payment.Status), Type = typeof(PaymentStatus)},
                    new() {Label = "PaymentDate", PropertyName = nameof(Payment.PaymentDate), Type = typeof(DateOnly)},
                    new() {Label = "IsActive", PropertyName = nameof(Payment.IsActive), Type = typeof(bool?)},
                }
            );
            viewModel.IsWriteable = false;
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
        }*/

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
                    new() {Label = "Total", PropertyName = nameof(Order.Total), Type = typeof(decimal), IsReadOnly = true},
                    new() {Label = "IsActive", PropertyName = nameof(Order.IsActive), Type = typeof(bool?)},
                }
            );
            viewModel.IsWriteable = false;
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
                    DisplayMemberPath = "Name",
                    ItemsSourceProvider = () => discountList,
                    DisplayMemberSelector = d => ((Discount)d).Id
                }
            };
            viewModel.ForeignKeyMappings["UserId"] = new ForeignKeyMapping
            {
                EntityType = typeof(User),
                DisplayProperty = "FullName"
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
            var context = new FigureManagementSystemContext();

            var orderList = context.Orders.ToList();
            var productList = context.Products.ToList();

            var viewModel = new GenericManagementViewModel<OrderDetail, int>(
                ownerWindow: Application.Current.MainWindow,
                entityName: "OrderDetail",
                idSelector: od => od.Id,
                displayNameSelector: od => string.Join(" - ", od.Id, od.Quantity),
                searchPredicate: null,
                toggleStatusAction: od => od.IsActive = !(od.IsActive ?? false),
                fieldDefinitions: new List<Helpers.FieldDefinition>
                {
                    new() {Label = "Price", PropertyName = nameof(OrderDetail.Price), Type = typeof(decimal), IsReadOnly = true},
                    new() {Label = "Quantity", PropertyName = nameof(OrderDetail.Quantity), Type = typeof(int)},
                    new() {Label = "IsActive", PropertyName = nameof(OrderDetail.IsActive), Type = typeof(bool?)},
                }
            );
            viewModel.IsWriteable = false;
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
                    DisplayMemberPath = "Name",
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
            var viewModel = new GenericManagementViewModel<User, string>(
                ownerWindow: Application.Current.MainWindow,
                entityName: "Users",
                idSelector: r => r.Id,
                displayNameSelector: r => r.FullName,
                searchPredicate: (r, text) => r.FullName.Contains(text, StringComparison.OrdinalIgnoreCase),
                toggleStatusAction: r => r.IsActive = !(r.IsActive ?? false),
                fieldDefinitions: new List<Helpers.FieldDefinition>
                {
                    new() {Label = "Full Name", PropertyName = nameof(User.FullName), Type = typeof(string)},
                    new() {Label = "IsActive", PropertyName = nameof(Role.IsActive), Type = typeof(bool?)},
                }

            );
            viewModel.IsWriteable = false;
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
            viewModel.ForeignKeyMappings["RoleId"] = new ForeignKeyMapping
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

        private void OpenChatbox()
        {
            var chatboxWindow = new ChatboxWindow();

            var chatboxViewModel = new ChatboxViewModel();
            chatboxViewModel.CloseWindowAction = chatboxWindow.Close;
            chatboxWindow.DataContext = chatboxViewModel;

            chatboxWindow.Owner = System.Windows.Application.Current.MainWindow; 
            chatboxWindow.ShowDialog();
        }
    }
}
