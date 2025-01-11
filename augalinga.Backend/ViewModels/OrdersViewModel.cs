using augalinga.Backend.Models;
using augalinga.Backend.Services;
using augalinga.Data.Access;
using augalinga.Data.Entities;
using augalinga.Data.Enums;
using Azure;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.Backend.ViewModels
{
    public class OrdersViewModel
    {
        int _projectId;
        private readonly DataContext _dbContext;
        private readonly AzureBlobStorage _blobStorage;
        private readonly string _folder = "orders";
        public OrdersViewModel(int projectId, DataContext dbContext)
        {
            _projectId = projectId;
            _dbContext = dbContext;
            _blobStorage = new AzureBlobStorage();
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

        public async Task UploadOrdersAsync(IEnumerable<IBrowserFile> selectedFiles, Project project, INotificationService notificationService)
        {
            try
            {
                foreach (var file in selectedFiles)
                {
                    await _blobStorage.UploadFile(project.Name, _folder, file);
                    string fileUrl = await _blobStorage.GetBlobUrlAsync(project.Name, _folder, file.Name);

                    var newOrder = new Order
                    {
                        ProjectId = _projectId,
                        Name = file.Name,
                        Link = fileUrl
                    };

                    await _dbContext.Orders.AddAsync(newOrder);
                    await _dbContext.SaveChangesAsync();

                    Orders.Add(newOrder);

                    notificationService.CreateNotification(newOrder.Name, project.Name, NotificationType.OrderAdded, null);
                }
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Azure Storage Error: {ex}");
                throw new Exception("Failed to upload orders to Azure Blob Storage.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex}");
                throw;
            }
        }

        public async Task DeleteOrderAsync(Order order, Project project, INotificationService notificationService)
        {
            try
            {
                string blobName = $"{project.Name}/{_folder}/{order.Name}";
                await _blobStorage.DeleteBlobAsync(blobName);

                _dbContext.Orders.Remove(order);
                await _dbContext.SaveChangesAsync();

                Orders.Remove(order);

                notificationService.CreateNotification(order.Name, project.Name, NotificationType.OrderDeleted, null);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Azure Storage Error: {ex}");
                throw new Exception("Failed to delete order from Azure Blob Storage.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex}");
                throw;
            }
        }
    }
}
