using PawnShop.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace PawnShop.Views
{
    public partial class AddTransactionWindow : Window
    {
        public Transactions NewTransaction { get; private set; }
        private readonly ObservableCollection<Pledge> Pledges;
        private static readonly Regex _numberRegex = new Regex("[^0-9,.]"); // Разрешаем только цифры и разделители

        public AddTransactionWindow(ObservableCollection<Pledge> pledges)
        {
            InitializeComponent();
            Pledges = pledges;

            // Инициализация значений
            TransactionDatePicker.SelectedDate = DateTime.Today;
            PledgeComboBox.ItemsSource = Pledges;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                var selectedPledge = PledgeComboBox.SelectedItem as Pledge;
                var transactionType = (TransactionTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (decimal.TryParse(AmountTextBox.Text.Replace(".", ","), out decimal amount))
                {
                    NewTransaction = new Transactions
                    {
                        TransactionType = transactionType,
                        TransactionDate = TransactionDatePicker.SelectedDate ?? DateTime.Today,
                        Amount = amount,
                        PledgeID = selectedPledge.PledgeID,
                        Pledge = selectedPledge
                    };

                    // Если тип транзакции - "Погашение", меняем статус залога
                    if (transactionType == "Погашение" && selectedPledge != null)
                    {
                        using (var context = new LombardContext())
                        {
                            var pledgeToUpdate = context.Pledges.Find(selectedPledge.PledgeID);
                            if (pledgeToUpdate != null)
                            {
                                pledgeToUpdate.Status = "Погашен";
                                context.SaveChanges();

                                // Обновляем локальную модель
                                selectedPledge.Status = "Погашен";
                            }
                        }
                    }

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при создании транзакции: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (!decimal.TryParse(AmountTextBox.Text.Replace(".", ","), out decimal _))
            {
                ShowError("Пожалуйста, введите корректную сумму");
                return false;
            }

            return true;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Предупреждение",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, является ли вводимый символ допустимым
            e.Handled = _numberRegex.IsMatch(e.Text);
        }
    }
}