using PawnShop.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PawnShop.Views
{
    public partial class TransactionsPage : UserControl
    {
        private ObservableCollection<Transactions> _transactions; // Приватное поле
        public ObservableCollection<Transactions> Transactions
        {
            get
            {
                if (_transactions == null)
                {
                    _transactions = new ObservableCollection<Transactions>();
                }
                return _transactions;
            }
            set
            {
                _transactions = value;
            }
        }

        private ObservableCollection<Transactions> allTransactions;
        private string searchText = "";

        public TransactionsPage()
        {
            InitializeComponent();
            Transactions = new ObservableCollection<Transactions>(); // Явная инициализация
            allTransactions = new ObservableCollection<Transactions>();
            TransactionsDataGrid.ItemsSource = Transactions;
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            try
            {
                using (var db = new LombardContext())
                {
                    var transactions = db.Transactions
                        .Include("Pledge")
                        .Include("Employee")
                        .ToList(); 

                    if (transactions != null)
                    {
                        Transactions.Clear();
                        allTransactions.Clear();
                        foreach (var transaction in transactions)
                        {
                            if (transaction != null)
                            {
                                Transactions.Add(transaction);
                                allTransactions.Add(transaction);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Данные транзакций не найдены в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                var pledges = new ObservableCollection<Pledge>(db.Pledges.ToList());

                if (!pledges.Any())
                {
                    MessageBox.Show("Залог не найден. Пожалуйста, добавьте первый залог.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

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

                    if (!db.Pledges.Any(p => p.PledgeID == newTransaction.PledgeID))
                    {
                        MessageBox.Show("Указанный залог не существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (newTransaction.EmployeeId == 0 || !db.Employees.Any(emp => emp.EmployeeID == newTransaction.EmployeeId))
                    {
                        MessageBox.Show("Указанный сотрудник не существует или не выбран.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    db.Transactions.Add(newTransaction);

                    try
                    {
                        db.SaveChanges();
                        Console.WriteLine($"Транзакция добавлена: TransactionID={newTransaction.TransactionID}, EmployeeId={newTransaction.EmployeeId}");

                        if (!Transactions.Contains(newTransaction) && newTransaction != null)
                        {
                            Transactions.Add(newTransaction);
                            allTransactions.Add(newTransaction);
                        }
                        UpdateSearch();
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

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Поиск...")
            {
                textBox.Text = "";
                textBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Поиск...";
                textBox.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchText = SearchTextBox.Text;
            UpdateSearch();
        }

        private void SearchRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateSearch();
        }

        private void UpdateSearch()
        {
            if (Transactions == null)
            {
                Transactions = new ObservableCollection<Transactions>(); 
                TransactionsDataGrid.ItemsSource = Transactions;
            }

            Transactions.Clear();

            if (allTransactions == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(searchText) || searchText == "Поиск...")
            {
                foreach (var transaction in allTransactions)
                {
                    if (transaction != null)
                    {
                        Transactions.Add(transaction);
                    }
                }
                return;
            }

            var filtered = allTransactions.Where(t =>
            {
                if (t == null) return false;
                if (SearchByTransactionIdRadio.IsChecked == true)
                    return t.TransactionID.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                if (SearchByEmployeeIdRadio.IsChecked == true)
                    return t.EmployeeId.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                if (SearchByPledgeIdRadio.IsChecked == true)
                    return t.PledgeID.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                if (SearchByTypeRadio.IsChecked == true)
                    return !string.IsNullOrEmpty(t.TransactionType) && t.TransactionType.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                return false;
            });

            foreach (var transaction in filtered)
            {
                if (transaction != null)
                {
                    Transactions.Add(transaction);
                }
            }
        }
    }
}