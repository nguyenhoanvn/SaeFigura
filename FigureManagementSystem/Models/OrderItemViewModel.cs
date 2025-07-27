using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigureManagementSystem.Models
{
    public class OrderItemViewModel : INotifyPropertyChanged
    {
        public Product Product { get; }
        private int _quantity;
        public int Quantity { get => _quantity; set { _quantity = value; OnPropertyChanged(nameof(Quantity)); OnPropertyChanged(nameof(Total)); } }
        public decimal Total => Quantity * Product.Price;

        public OrderItemViewModel(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
