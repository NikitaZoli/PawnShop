using PawnShop.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PawnShop.Views
{
    public partial class TransactionsPage : UserControl
    {
        public ObservableCollection<Transactions> Transactions { get; set; }

        public TransactionsPage()
        {
            InitializeComponent();
            Transactions = new ObservableCollection<Transactions>();
            TransactionsDataGrid.ItemsSource = Transactions;
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            try
            {
                using (var db = new LombardContext())
                {
                    var transactions = db.Transactions.ToList();
                    foreach (var transaction in transactions)
                    {
                        Transactions.Add(transaction);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке транзакций: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LombardContext())
            {
                // Получить список доступных залогов
                var pledges = new ObservableCollection<Pledge>(db.Pledges.ToList());

                if (!pledges.Any())
                {
                    MessageBox.Show("Залог не найден. Пожалуйста, добавьте первый залог.");
                    return;
                }

                var addTransactionWindow = new AddTransactionWindow(pledges);

                if (addTransactionWindow.ShowDialog() == true)
                {
                    var newTransaction = addTransactionWindow.NewTransaction;

                    db.Transactions.Add(newTransaction);

                    try
                    {
                        db.SaveChanges();

                        if (!Transactions.Contains(newTransaction))
                        {
                            Transactions.Add(newTransaction);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка сохранения транзакции: {ex.Message}");
                    }
                }
            }
        }
    }
}
