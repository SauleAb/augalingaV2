﻿using augalinga.Data.Access;
using augalinga.Data.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace augalinga.Backend.ViewModels
{
    public class ExpensesViewModel
    {
        private int _projectId;
        public ExpensesViewModel(int projectId)
        {
            _projectId = projectId;
            LoadExpenses(_projectId);
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadExpenses(int projectId)
        {
            using (var context = new DataContext())
            {
                var expenses = context.Expenses.Where(expense => expense.ProjectId == projectId).OrderByDescending(expense => expense.Date).ToList();

                Expenses = new ObservableCollection<Expense>(expenses);
            }

            Income = GetIncome();
            Outcome = GetOutcome();
            Total = GetTotal();
        }
    }
}