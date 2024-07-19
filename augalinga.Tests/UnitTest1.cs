
namespace augalinga.Tests
{
    [TestClass]
    public class ExpensesViewModelTests
    {
        private ExpensesViewModel viewModel;
        private List<Expense> testExpenses;

        [TestInitialize]
        public void Setup()
        {
            testExpenses = new List<Expense>
            {
                new Expense { Id = 1, Amount = 100, Type = "+", Project = "TestProject", Date = DateTime.Now },
                new Expense { Id = 2, Amount = 50, Type = "-", Project = "TestProject", Date = DateTime.Now }
            };

            viewModel = new ExpensesViewModel("TestProject")
            {
                Expenses = new ObservableCollection<Expense>(testExpenses)
            };
        }

        [TestMethod]
        public void IncomeProperty_ShouldSetAndGetCorrectly()
        {
            viewModel.Income = 200;
            Assert.AreEqual(200, viewModel.Income);
        }

        [TestMethod]
        public void OutcomeProperty_ShouldSetAndGetCorrectly()
        {
            viewModel.Outcome = 100;
            Assert.AreEqual(100, viewModel.Outcome);
        }

        [TestMethod]
        public void TotalProperty_ShouldSetAndGetCorrectly()
        {
            viewModel.Total = 300;
            Assert.AreEqual(300, viewModel.Total);
        }

        [TestMethod]
        public void ExpensesProperty_ShouldSetAndGetCorrectly()
        {
            var expenses = new ObservableCollection<Expense>(testExpenses);
            viewModel.Expenses = expenses;
            CollectionAssert.AreEqual(expenses, viewModel.Expenses);
        }

        [TestMethod]
        public void GetIncome_ShouldReturnCorrectSum()
        {
            decimal income = viewModel.GetIncome();
            Assert.AreEqual(100, income);
        }

        [TestMethod]
        public void GetOutcome_ShouldReturnCorrectSum()
        {
            decimal outcome = viewModel.GetOutcome();
            Assert.AreEqual(50, outcome);
        }

        [TestMethod]
        public void GetTotal_ShouldReturnCorrectDifference()
        {
            decimal total = viewModel.GetTotal();
            Assert.AreEqual(50, total);
        }

        [TestMethod]
        public void AddExpenseToCollection_ShouldAddExpense()
        {
            var newExpense = new Expense { Id = 3, Amount = 200, Type = "+", Project = "TestProject", Date = DateTime.Now };
            viewModel.AddExpenseToCollection(newExpense);
            Assert.IsTrue(viewModel.Expenses.Contains(newExpense));
        }

        [TestMethod]
        public void RemoveExpense_ShouldRemoveExpense()
        {
            var expenseToRemove = viewModel.Expenses.First();
            viewModel.RemoveExpense(expenseToRemove.Id);
            Assert.IsFalse(viewModel.Expenses.Contains(expenseToRemove));
        }

        [TestMethod]
        public void PropertyChanged_ShouldBeRaisedForIncome()
        {
            bool wasRaised = false;
            viewModel.PropertyChanged += (sender, args) => { if (args.PropertyName == "Income") wasRaised = true; };

            viewModel.Income = 500;
            Assert.IsTrue(wasRaised);
        }

        [TestMethod]
        public void PropertyChanged_ShouldBeRaisedForOutcome()
        {
            bool wasRaised = false;
            viewModel.PropertyChanged += (sender, args) => { if (args.PropertyName == "Outcome") wasRaised = true; };

            viewModel.Outcome = 200;
            Assert.IsTrue(wasRaised);
        }

        [TestMethod]
        public void LoadExpenses_ShouldInitializeExpenses()
        {
            viewModel = new ExpensesViewModel("TestProject");
            viewModel.LoadExpenses("TestProject");

            Assert.IsTrue(viewModel.Expenses.Count > 0);
            Assert.AreEqual(100, viewModel.Income);
            Assert.AreEqual(50, viewModel.Outcome);
            Assert.AreEqual(50, viewModel.Total);
        }
    }
}