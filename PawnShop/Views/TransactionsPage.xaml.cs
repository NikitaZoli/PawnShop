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
                    MessageBox.Show("Залог не найден. Пожалуйста, добавьте первый залог.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Получить список сотрудников
                var employees = new ObservableCollection<Employees>(db.Employees.ToList());

                if (!employees.Any())
                {
                    MessageBox.Show("Сотрудник не найден. Пожалуйста, добавьте первого сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var addTransactionWindow = new AddTransactionWindow(pledges, employees);

                if (addTransactionWindow.ShowDialog() == true)
                {
                    var newTransaction = addTransactionWindow.NewTransaction;

                    // Проверка существования Pledge и Employee
                    if (!db.Pledges.Any(p => p.PledgeID == newTransaction.PledgeID))
                    {
                        MessageBox.Show("Указанный залог не существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (newTransaction.EmployeeId == 0 || !db.Employees.Any(emp => emp.EmployeeID == newTransaction.EmployeeId)) // Изменено на "emp"
                    {
                        MessageBox.Show("Указанный сотрудник не существует или не выбран.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    db.Transactions.Add(newTransaction);

                    try
                    {
                        db.SaveChanges();
                        Console.WriteLine($"Транзакция добавлена: TransactionID={newTransaction.TransactionID}, EmployeeId={newTransaction.EmployeeId}");

                        if (!Transactions.Contains(newTransaction))
                        {
                            Transactions.Add(newTransaction);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка сохранения транзакции: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Внутреннее исключение: {ex.InnerException.Message}");
                            if (ex.InnerException.InnerException != null)
                            {
                                Console.WriteLine($"Внутреннее исключение 2: {ex.InnerException.InnerException.Message}");
                            }
                        }
                        MessageBox.Show($"Ошибка сохранения транзакции: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}