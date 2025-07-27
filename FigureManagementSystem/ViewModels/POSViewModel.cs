using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FigureManagementSystem.Helpers;
using FigureManagementSystem.Models;
using FigureManagementSystem.Views;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FigureManagementSystem.ViewModels
{
    public class POSViewModel : INotifyPropertyChanged
    {
        private readonly POSWindow _window;
        public POSViewModel(POSWindow window)
        {
            _window = window;
            _window.Btn_RemoveItem += RemoveItem_Click;
            _window.Btn_AddToCart += AddToCart_Click;

            SubmitOrderCommand = SubmitOrderCommand = new RelayCommand(
                _ => SubmitOrder(),
                _ => OrderItems.Any()
            );
            LogoutCommand = new RelayCommand(_ => Logout());

            LoadData();

        }

        public ICommand LogoutCommand { get; }
        private void Logout()
        {

            var loginWindow = new LoginWindow();
            Application.Current.MainWindow = loginWindow;
            loginWindow.Show();

            _window.Close();
        }

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<OrderItemViewModel> OrderItems { get; set; } = new();
        public ObservableCollection<Discount> Discounts { get; set; }

        public ObservableCollection<Product> FilteredProducts =>
            new ObservableCollection<Product>(Products.Where(p => string.IsNullOrEmpty(ProductFilter)
                || p.Name.Contains(ProductFilter, StringComparison.OrdinalIgnoreCase)));

        public Product? SelectedProduct { get; set; }
        public string ProductFilter { get => _productFilter; set { _productFilter = value; OnPropertyChanged(nameof(ProductFilter)); OnPropertyChanged(nameof(FilteredProducts)); } }
        private string _productFilter = string.Empty;

        public int QuantityToAdd { get; set; } = 1;
        public decimal TotalAmount { get; private set; }
        public decimal DiscountedAmount { get; private set; }
        public decimal FinalAmount { get; private set; }

        public ICommand SubmitOrderCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string? _discountCode;
        public string? DiscountCode
        {
            get => _discountCode;
            set { _discountCode = value; OnPropertyChanged(nameof(DiscountCode)); }
        }
        private Discount? _appliedDiscount;
        public Discount? AppliedDiscount
        {
            get => _appliedDiscount;
            set { _appliedDiscount = value; OnPropertyChanged(nameof(AppliedDiscount)); NotifyTotals(); }
        }

        public string AppliedDiscountText => AppliedDiscount != null
            ? $"Discount Applied: {AppliedDiscount.Name} ({(AppliedDiscount.Type == DiscountType.Percent ? $"{AppliedDiscount.Value}%" : $"${AppliedDiscount.Value}")})"
            : "";

        public ICommand ApplyDiscountCommand => new RelayCommand(_ =>
        {
            if (string.IsNullOrWhiteSpace(DiscountCode))
            {
                MessageBox.Show("Please enter a discount code.");
                return;
            }

            using var context = new FigureManagementSystemContext();
            var now = DateOnly.FromDateTime(DateTime.Now);
            var discount = context.Discounts
                .FirstOrDefault(d =>
                    d.Id == DiscountCode &&
                    d.IsActive == true &&
                    d.ActivateDate <= now && 
                    now <= d.ExpireDate);

            if (discount == null)
            {
                MessageBox.Show("Invalid or expired discount code.");
                AppliedDiscount = null;
                return;
            }
            if (discount.Type == DiscountType.Fixed && discount.Value >= TotalAmount)
            {
                MessageBox.Show($"This order is not valid for apply this discount! The value must higher than {discount.Value}$");
                AppliedDiscount = null;
                return;
            } 

            AppliedDiscount = discount;
            MessageBox.Show("Discount applied successfully!");
        });

        private void LoadData()
        {
            using var db = new FigureManagementSystemContext();
            Products = new ObservableCollection<Product>(db.Products.Where(p => p.IsActive == true && p.Quantity > 0).ToList());
            Discounts = new ObservableCollection<Discount>(db.Discounts.Where(d => d.IsActive == true).ToList());
        }

        public void AddToOrder(Product product, int quantity)
        {
            var existing = OrderItems.FirstOrDefault(i => i.Product.Id == product.Id);
            int currentCartQuantity = existing?.Quantity ?? 0;
            int availableToAdd = product.Quantity - currentCartQuantity;

            if (quantity <= 0 || quantity > availableToAdd)
            {
                MessageBox.Show($"Invalid quantity. Max you can add: {availableToAdd}");
                return;
            }

            if (existing != null)
                existing.Quantity += quantity;
            else
                OrderItems.Add(new OrderItemViewModel(product, quantity));

            NotifyTotals();
        }

        public void NotifyTotals()
        {
            TotalAmount = OrderItems.Sum(item => item.Total);

            if (AppliedDiscount != null)
            {
                if (AppliedDiscount.Type == DiscountType.Percent)
                {
                    DiscountedAmount = TotalAmount * AppliedDiscount.Value / 100;
                }
                else
                {
                    DiscountedAmount = AppliedDiscount.Value;
                }

                FinalAmount = TotalAmount - DiscountedAmount;
            }
            else
            {
                DiscountedAmount = 0;
                FinalAmount = TotalAmount;
            }
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(DiscountedAmount));
            OnPropertyChanged(nameof(FinalAmount));
            OnPropertyChanged(nameof(AppliedDiscountText));
        }


        private void SubmitOrder()
        {
            if (!OrderItems.Any())
            {
                MessageBox.Show("Cart is empty.");
                return;
            }

            using var db = new FigureManagementSystemContext();
            var currentUser = SessionManager.CurrentUser;
            if (currentUser != null)
            {

                var order = new Order
                {
                    OrderDate = DateOnly.FromDateTime(DateTime.Today),
                    UserId = currentUser.Id,
                    DiscountId = DiscountCode,
                    Total = FinalAmount,
                    IsActive = true,
                    OrderDetails = OrderItems.Select(item => new OrderDetail
                    {
                        ProductId = item.Product.Id,
                        Quantity = item.Quantity,
                        Price = item.Product.Price,
                        IsActive = true
                    }).ToList()
                };

                db.Orders.Add(order);

                foreach (var item in OrderItems)
                {
                    var product = db.Products.Find(item.Product.Id);
                    if (product != null)
                        product.Quantity -= item.Quantity;
                }

                var discount = db.Discounts.Find(DiscountCode);
                if (discount != null)
                {
                    discount.IsActive = false;
                }
                
                db.SaveChanges();
                MessageBox.Show("Order submitted!");
                OrderItems.Clear();
                TotalAmount = 0;
                DiscountCode = "";
                FinalAmount = 0;
                DiscountedAmount = 0;
                NotifyTotals();
                LoadData();
                OnPropertyChanged(nameof(FilteredProducts));
            } else
            {
                MessageBox.Show("Current User cannot found in session!");
                return;
            }
        }
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProduct == null || QuantityToAdd <= 0)
            {
                MessageBox.Show("Select product and valid quantity.");
                return;
            }
            AddToOrder(SelectedProduct, QuantityToAdd);
        }

        private void RemoveItem_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is OrderItemViewModel item)
            {
                OrderItems.Remove(item);
                NotifyTotals();
            }
        }
    }
}
