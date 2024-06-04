using augalinga.Data.Access;
using augalinga.Data.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.ApplicationLayer.Components.ViewModels
{
    public class ExpensesViewModel
    {
        private string _projectName;
        public ExpensesViewModel(string projectName)
        {
            _projectName = projectName;
            LoadExpenses(_projectName);
        }

        private decimal _income;
        public decimal Income
        {
            get => _income;
            set
            {
                _income = value;
                OnPropertyChanged(nameof(Income));
            }
        }
        private decimal _outcome;
        public decimal Outcome
        {
            get => _outcome;
            set
            {
                _outcome = value;
                OnPropertyChanged(nameof(Outcome));
            }
        }
        private decimal _total;
        public decimal Total
        {
            get => _total;
            set
            {
                _total = value;
                OnPropertyChanged(nameof(Total));
            }
        }



        public decimal GetIncome()
        {
            var incomeExpenses = Expenses.Where(expense => expense.Type == "+");

            if (!incomeExpenses.Any())
            {
                return 0;
            }

            return incomeExpenses.Sum(expense => expense.Amount);
        }

        public decimal GetOutcome()
        {
            var outcomeExpenses = Expenses.Where(expense => expense.Type == "-");

            if (!outcomeExpenses.Any())
            {
                return 0;
            }

            return outcomeExpenses.Sum(expense => expense.Amount);
        }

        public decimal GetTotal() //add to project entity
        {
            if (Expenses.Count == 0)
            {
                return 0;
            }
            return GetIncome() - GetOutcome();
        }

        private ObservableCollection<Expense> _expenses;
        public ObservableCollection<Expense> Expenses
        {
            get => _expenses;
            set
            {
                _expenses = value;
                OnPropertyChanged(nameof(Expenses));
            }
        }

        public void AddExpenseToCollection(Expense expense)
        {
            Expenses.Add(expense);
            LoadExpenses(_projectName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadExpenses(string projectName)
        {
            using (var context = new DataContext())
            {
                var expenses = context.Expenses.Where(expense => expense.Project == projectName).OrderByDescending(expense => expense.Date).ToList();

                Expenses = new ObservableCollection<Expense>(expenses);
            }

            Income = GetIncome();
            Outcome = GetOutcome();
            Total = GetTotal();
        }

        public void RemoveExpense(int expenseId)
        {
            //local
            var expenseToRemove = Expenses.FirstOrDefault(p => p.Id == expenseId);
            Expenses.Remove(expenseToRemove);

            //database
            using (var dbContext = new DataContext())
            {
                dbContext.Expenses.Remove(expenseToRemove);
                dbContext.SaveChanges();
            }

            LoadExpenses(_projectName);
        }
    }
}
