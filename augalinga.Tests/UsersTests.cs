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
    public class UsersTests
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
        public void LoadUsers_ShouldLoadAllUsersFromDatabase()
        {
            // Arrange
            var context = CreateDbContext();
            var users = new List<User>
            {
                new User { Id = 1, FullName = "User 1", Email = "user1@example.com", Password = "password1", Token = "token1", Color = "#FF0000" },
                new User { Id = 2, FullName = "User 2", Email = "user2@example.com", Password = "password2", Token = "token2", Color = "#00FF00" }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            // Act
            var viewModel = new UsersViewModel(context);

            // Assert
            Assert.Equal(2, viewModel.Users.Count);
            Assert.Contains(viewModel.Users, u => u.FullName == "User 1");
            Assert.Contains(viewModel.Users, u => u.FullName == "User 2");
        }

        [Fact]
        public void Users_ShouldRaisePropertyChanged_WhenValueIsSet()
        {
            // Arrange
            var context = CreateDbContext();
            var viewModel = new UsersViewModel(context);
            bool propertyChangedRaised = false;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(UsersViewModel.Users))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            viewModel.Users = new ObservableCollection<User>
            {
                new User { Id = 1, FullName = "New User", Email = "newuser@example.com", Password = "password", Token = "token", Color = "#123456" }
            };

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void LoadUsers_ShouldLoadEmptyCollection_WhenNoUsersExist()
        {
            // Arrange
            var context = CreateDbContext();

            // Act
            var viewModel = new UsersViewModel(context);

            // Assert
            Assert.Empty(viewModel.Users);
        }
    }
}
