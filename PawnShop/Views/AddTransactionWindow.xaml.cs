using PawnShop.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace PawnShop.Views
{
    public partial class AddTransactionWindow : Window
    {
        public Transactions NewTransaction { get; private set; }
        private readonly ObservableCollection<Pledge> Pledges;
        private readonly ObservableCollection<Employees> Employees;
        private static readonly Regex _numberRegex = new Regex("[^0-9,.]"); // Разрешаем только цифры и разделители

        public AddTransactionWindow(ObservableCollection<Pledge> pledges, ObservableCollection<Employees> employees)
        {
            InitializeComponent();
            Pledges = pledges;
            Employees = employees;

            // Инициализация значений
            TransactionDatePicker.SelectedDate = DateTime.Today;
            PledgeComboBox.ItemsSource = Pledges;
            EmployeeComboBox.ItemsSource = Employees;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                var selectedPledge = PledgeComboBox.SelectedItem as Pledge;
                var selectedEmployee = EmployeeComboBox.SelectedItem as Employees;
                var transactionType = (TransactionTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (decimal.TryParse(AmountTextBox.Text.Replace(".", ","), out decimal amount))
                {
                    NewTransaction = new Transactions
                    {
                        TransactionType = transactionType,
                        TransactionDate = TransactionDatePicker.SelectedDate ?? DateTime.Today,
                        Amount = amount,
                        PledgeID = selectedPledge.PledgeID,
                        EmployeeId = selectedEmployee.EmployeeID // Указываем EmployeeId
                    };

                    // Если тип транзакции - "Погашение", меняем статус залога
                    if (transactionType == "Погашение займа" && selectedPledge != null)
                    {
                        try
                        {
                            using (var context = new LombardContext())
                            {
                                var pledgeToUpdate = context.Pledges
                                    .FirstOrDefault(p => p.PledgeID == selectedPledge.PledgeID);

                                if (pledgeToUpdate != null)
                                {
                                    context.Pledges.Attach(pledgeToUpdate);
                                    pledgeToUpdate.Status = "Погашен";
                                    context.Entry(pledgeToUpdate).Property(x => x.Status).IsModified = true;

                                    var changes = context.SaveChanges();

                                    if (changes > 0)
                                    {
                                        selectedPledge.Status = "Погашен";
                                        var pledgeInCollection = Pledges.FirstOrDefault(p => p.PledgeID == selectedPledge.PledgeID);
                                        if (pledgeInCollection != null)
                                        {
                                            pledgeInCollection.Status = "Погашен";
                                        }
                                        MessageBox.Show("Статус залога успешно обновлен на 'Погашен'");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Не удалось обновить статус залога!");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при обновлении статуса: {ex.Message}");
                        }
                    }

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении транзакции: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Внутреннее исключение: {ex.InnerException.Message}");
                    if (ex.InnerException.InnerException != null)
                    {
                        Console.WriteLine($"Внутреннее исключение 2: {ex.InnerException.InnerException.Message}");
                    }
                }
                MessageBox.Show($"Произошла ошибка при создании транзакции: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (TransactionTypeComboBox.SelectedItem == null)
            {
                ShowError("Пожалуйста, выберите тип транзакции");
                return false;
            }

            if (string.IsNullOrWhiteSpace(AmountTextBox.Text))
            {
                ShowError("Пожалуйста, введите сумму транзакции");
                return false;
            }

            if (PledgeComboBox.SelectedItem == null)
            {
                ShowError("Пожалуйста, выберите залог");
                return false;
            }

            if (EmployeeComboBox.SelectedItem == null)
            {
                ShowError("Пожалуйста, выберите сотрудника");
                return false;
            }

            if (!decimal.TryParse(AmountTextBox.Text.Replace(".", ","), out decimal _))
            {
                ShowError("Пожалуйста, введите корректную сумму");
                return false;
            }
            return true;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _numberRegex.IsMatch(e.Text);
        }
    }
}