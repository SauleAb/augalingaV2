using augalinga.Backend.ViewModels;
using augalinga.Data.Access;
using augalinga.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace augalinga.Tests
{
    public class OrdersViewModelTests
    {
        private DataContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new DataContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public void LoadOrders_ShouldLoadAllOrdersForSpecificProject()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            var orders = new List<Order>
            {
                new Order { Id = 1, ProjectId = 1, Name = "Order 1", Link = "Link1" },
                new Order { Id = 2, ProjectId = 1, Name = "Order 2", Link = "Link2" },
                new Order { Id = 3, ProjectId = 2, Name = "Order 3", Link = "Link3" }
            };

            context.Orders.AddRange(orders);
            context.SaveChanges();

            // Act
            var viewModel = new OrdersViewModel(projectId, context);

            // Assert
            Assert.Equal(2, viewModel.Orders.Count);
            Assert.Contains(viewModel.Orders, o => o.Name == "Order 1");
            Assert.Contains(viewModel.Orders, o => o.Name == "Order 2");
        }

        [Fact]
        public void Orders_ShouldRaisePropertyChanged_WhenValueIsSet()
        {
            // Arrange
            var context = CreateDbContext();
            var viewModel = new OrdersViewModel(1, context);
            bool propertyChangedRaised = false;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(OrdersViewModel.Orders))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            viewModel.Orders = new ObservableCollection<Order>
            {
                new Order { Id = 1, ProjectId = 1, Name = "New Order", Link = "NewLink" }
            };

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void LoadOrders_ShouldLoadEmptyCollection_WhenNoOrdersExistForProject()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            // Act
            var viewModel = new OrdersViewModel(projectId, context);

            // Assert
            Assert.Empty(viewModel.Orders);
        }
    }
}
