using augalinga.Data.Access;
using augalinga.Data.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.ApplicationLayer.Components.ViewModels
{
    public class OrdersViewModel
    {
        int _projectId;
        public OrdersViewModel(int projectId)
        {
            _projectId = projectId;
            LoadOrders(_projectId);
        }

        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders));
            }
        }

        public void AddOrderToCollection(Order order)
        {
            Orders.Add(order);
            LoadOrders(_projectId);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadOrders(int projectId)
        {
            using (var context = new DataContext())
            {
                var orders = context.Orders.Where(order => order.ProjectId == _projectId).ToList();

                Orders = new ObservableCollection<Order>(orders);
            }
        }

        public void RemoveOrder(string orderLink)
        {
            //local
            var orderToRemove = Orders.FirstOrDefault(p => p.Link == orderLink);
            Orders.Remove(orderToRemove);

            //database
            using (var dbContext = new DataContext())
            {
                dbContext.Orders.Remove(orderToRemove);
                dbContext.SaveChanges();
            }

            LoadOrders(_projectId);
        }
    }
}