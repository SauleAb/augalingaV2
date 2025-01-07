using augalinga.Backend.ViewModels;
using augalinga.Data.Access;
using augalinga.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace augalinga.Tests
{
    public class ExpensesTests
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
        public void LoadExpenses_ShouldLoadExpensesForSpecificProject()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            var expenses = new List<Expense>
            {
                new Expense { Id = 1, ProjectId = 1, Amount = 100, Type = "+", Transaction = "Test", Date = DateOnly.FromDateTime(DateTime.Today) },
                new Expense { Id = 2, ProjectId = 1, Amount = 50, Type = "-", Transaction="Test", Date = DateOnly.FromDateTime(DateTime.Today) },
                new Expense { Id = 3, ProjectId = 2, Amount = 200, Type = "+", Transaction="Test", Date = DateOnly.FromDateTime(DateTime.Today) }
            };

            context.Expenses.AddRange(expenses);
            context.SaveChanges();

            // Act
            var viewModel = new ExpensesViewModel(projectId, context);

            // Assert
            Assert.Equal(2, viewModel.Expenses.Count);
            Assert.Contains(viewModel.Expenses, e => e.Id == 1);
            Assert.Contains(viewModel.Expenses, e => e.Id == 2);
        }

        [Fact]
        public void GetIncome_ShouldReturnSumOfIncomeExpenses()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            var expenses = new List<Expense>
            {
                new Expense { Id = 1, ProjectId = 1, Amount = 100, Type = "+", Transaction = "Test", Date = DateOnly.FromDateTime(DateTime.Today )},
                new Expense { Id = 2, ProjectId = 1, Amount = 200, Type = "+", Transaction="Test", Date = DateOnly.FromDateTime(DateTime.Today) },
                new Expense { Id = 3, ProjectId = 1, Amount = 50, Type = "-", Transaction = "Test", Date = DateOnly.FromDateTime(DateTime.Today) }
            };

            context.Expenses.AddRange(expenses);
            context.SaveChanges();

            var viewModel = new ExpensesViewModel(projectId, context);

            // Act
            var income = viewModel.GetIncome();

            // Assert
            Assert.Equal(300, income);
        }

        [Fact]
        public void GetOutcome_ShouldReturnSumOfOutcomeExpenses()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            var expenses = new List<Expense>
            {
                new Expense { Id = 1, ProjectId = 1, Amount = 100, Type = "-", Transaction="Test", Date = DateOnly.FromDateTime(DateTime.Today) },
                new Expense { Id = 2, ProjectId = 1, Amount = 50, Type = "-", Transaction = "Test", Date = DateOnly.FromDateTime(DateTime.Today) },
                new Expense { Id = 3, ProjectId = 1, Amount = 200, Type = "+", Transaction="Test", Date = DateOnly.FromDateTime(DateTime.Today) }
            };

            context.Expenses.AddRange(expenses);
            context.SaveChanges();

            var viewModel = new ExpensesViewModel(projectId, context);

            // Act
            var outcome = viewModel.GetOutcome();

            // Assert
            Assert.Equal(150, outcome);
        }

        [Fact]
        public void GetTotal_ShouldReturnNetIncome()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            var expenses = new List<Expense>
            {
                new Expense { Id = 1, ProjectId = 1, Amount = 300, Type = "+", Transaction="Test", Date = DateOnly.FromDateTime(DateTime.Today) },
                new Expense { Id = 2, ProjectId = 1, Amount = 150, Type = "-", Transaction = "Test", Date = DateOnly.FromDateTime(DateTime.Today) }
            };

            context.Expenses.AddRange(expenses);
            context.SaveChanges();

            var viewModel = new ExpensesViewModel(projectId, context);

            // Act
            var total = viewModel.GetTotal();

            // Assert
            Assert.Equal(150, total);
        }

        [Fact]
        public void LoadExpenses_ShouldUpdateIncomeOutcomeAndTotal()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            var expenses = new List<Expense>
            {
                new Expense { Id = 1, ProjectId = 1, Amount = 100, Type = "+", Transaction = "Test", Date = DateOnly.FromDateTime(DateTime.Today) },
                new Expense { Id = 2, ProjectId = 1, Amount = 50, Type = "-", Transaction="Test", Date = DateOnly.FromDateTime(DateTime.Today) }
            };

            context.Expenses.AddRange(expenses);
            context.SaveChanges();

            // Act
            var viewModel = new ExpensesViewModel(projectId, context);

            // Assert
            Assert.Equal(100, viewModel.Income);
            Assert.Equal(50, viewModel.Outcome);
            Assert.Equal(50, viewModel.Total);
        }

        [Fact]
        public void LoadExpenses_ShouldOrderByDateDescending()
        {
            // Arrange
            var context = CreateDbContext();
            var projectId = 1;

            var expenses = new List<Expense>
            {
                new Expense { Id = 1, ProjectId = 1, Amount = 100, Type = "+", Transaction="Test", Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)) },
                new Expense { Id = 2, ProjectId = 1, Amount = 50, Type = "-", Transaction="Test", Date = DateOnly.FromDateTime(DateTime.Today )},
                new Expense { Id = 3, ProjectId = 1, Amount = 200, Type = "+", Transaction = "Test", Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-2)) }
            };

            context.Expenses.AddRange(expenses);
            context.SaveChanges();

            // Act
            var viewModel = new ExpensesViewModel(projectId, context);

            // Assert
            Assert.Equal(2, viewModel.Expenses.First().Id); 
        }
    }
}
