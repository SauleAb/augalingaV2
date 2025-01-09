using augalinga.Data.Access;
using augalinga.Data.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.Backend.ViewModels
{
    public class OrdersViewModel
    {
        int _projectId;
        private readonly DataContext _dbContext;
        public OrdersViewModel(int projectId, DataContext dbContext)
        {
            _projectId = projectId;
            _dbContext = dbContext;
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
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadOrders(int projectId)
        {
            var orders = _dbContext.Orders.Where(order => order.ProjectId == _projectId).ToList();

            Orders = new ObservableCollection<Order>(orders);
        }
    }
}