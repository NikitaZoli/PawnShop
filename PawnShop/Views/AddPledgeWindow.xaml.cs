using PawnShop.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PawnShop.Views
{
    // Используем partial class вместо создания нового определения
    public partial class AddPledgeWindow : Window
    {
        private Pledge EditedPledge;
        private bool IsEditMode;

        public AddPledgeWindow(Pledge pledge = null)
        {
            InitializeComponent();
            LoadClients();

            if (pledge != null)
            {
                EditedPledge = pledge;
                IsEditMode = true;
                LoadUIData(pledge);
            }
            else
            {
                IsEditMode = false;
                // Устанавливаем сегодняшнюю дату для нового залога
                PledgeDatePicker.SelectedDate = DateTime.Today;
            }
        }

        private void LoadClients()
        {
            using (var context = new LombardContext())
            {
                var clients = context.Clients.ToList();
                // Устанавливаем список клиентов в ComboBox
                ClientComboBox.ItemsSource = clients;
            }
        }

        private void LoadUIData(Pledge pledge)
        {
            ClientComboBox.SelectedValue = pledge.ClientID;
            ItemDescriptionTextBox.Text = pledge.ItemDescription;
            EstimatedValueTextBox.Text = pledge.EstimatedValue.ToString();
            LoanAmountTextBox.Text = pledge.LoanAmount.ToString();
            InterestRateTextBox.Text = pledge.InterestRate.ToString();
            PledgeDatePicker.SelectedDate = pledge.PledgeDate;
            DueDatePicker.SelectedDate = pledge.DueDate;
            StatusComboBox.SelectedItem = StatusComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == pledge.Status);
        }

        private bool ValidateUIInputs()
        {
            // Проверка на пустые поля
            if (ClientComboBox.SelectedValue == null ||
                string.IsNullOrWhiteSpace(ItemDescriptionTextBox.Text) ||
                string.IsNullOrWhiteSpace(EstimatedValueTextBox.Text) ||
                string.IsNullOrWhiteSpace(LoanAmountTextBox.Text) ||
                string.IsNullOrWhiteSpace(InterestRateTextBox.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            // Проверка существования клиента в базе данных
            int clientId = (int)ClientComboBox.SelectedValue;
            using (var context = new LombardContext())
            {
                if (!context.Clients.Any(c => c.ClientID == clientId))
                {
                    MessageBox.Show("Клиент с указанным ID не найден в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            // Проверка описания предмета залога
            if (ItemDescriptionTextBox.Text.Length < 10)
            {
                MessageBox.Show("Описание предмета залога должно содержать минимум 10 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            // Проверка EstimatedValue
            if (!decimal.TryParse(EstimatedValueTextBox.Text, out decimal estimatedValue) || estimatedValue <= 0)
            {
                MessageBox.Show("Оценочная стоимость должна быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            // Проверка LoanAmount
            if (!decimal.TryParse(LoanAmountTextBox.Text, out decimal loanAmount) || loanAmount <= 0)
            {
                MessageBox.Show("Сумма займа должна быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            // Проверка, что сумма займа не превышает оценочную стоимость
            if (loanAmount > estimatedValue)
            {
                MessageBox.Show("Сумма займа не может превышать оценочную стоимость!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            // Проверка InterestRate
            if (!decimal.TryParse(InterestRateTextBox.Text, out decimal interestRate) || interestRate <= 0 || interestRate > 100)
            {
                MessageBox.Show("Процентная ставка должна быть положительным числом не более 100!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            // Проверка дат
            if (!PledgeDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату залога!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!DueDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату возврата!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (DueDatePicker.SelectedDate <= PledgeDatePicker.SelectedDate)
            {
                MessageBox.Show("Дата возврата должна быть позже даты залога!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            // Проверка выбора статуса
            if (StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус залога!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void UpdateUIData(Pledge pledge)
        {
            pledge.ClientID = (int)(ClientComboBox.SelectedValue ?? 0);
            pledge.ItemDescription = ItemDescriptionTextBox.Text;
            pledge.EstimatedValue = decimal.Parse(EstimatedValueTextBox.Text);
            pledge.LoanAmount = decimal.Parse(LoanAmountTextBox.Text);
            pledge.InterestRate = decimal.Parse(InterestRateTextBox.Text);
            pledge.PledgeDate = PledgeDatePicker.SelectedDate ?? DateTime.Now;
            pledge.DueDate = DueDatePicker.SelectedDate ?? DateTime.Now.AddMonths(1);
            pledge.Status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Активный";
        }

        private void SavePledge_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateUIInputs())
                return;
            try
            {
                using (var context = new LombardContext())
                {
                    Pledge pledgeToSave;
                    if (IsEditMode)
                    {
                        pledgeToSave = context.Pledges.Find(EditedPledge.PledgeID);
                        if (pledgeToSave == null)
                        {
                            MessageBox.Show("Залог не найден.");
                            return;
                        }
                    }
                    else
                    {
                        pledgeToSave = new Pledge();
                        context.Pledges.Add(pledgeToSave);
                    }
                    UpdateUIData(pledgeToSave);
                    context.SaveChanges();
                    MessageBox.Show(IsEditMode
                        ? "Залог успешно обновлен!"
                        : "Новый залог добавлен!");
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}