using PawnShop.Models;
using System;
using System.Linq;
using System.Windows;

namespace PawnShop.Views
{
    public partial class AddClientWindow : Window
    {
        public Client NewClient { get; private set; } // Добавляем свойство для нового клиента
        public AddClientWindow()
        {
            InitializeComponent();
        }
        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PassportNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(AddressTextBox.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new LombardContext())
            {
                // Проверяем, существует ли клиент с таким номером паспорта
                if (context.Clients.Any(c => c.PassportNumber == PassportNumberTextBox.Text))
                {
                    MessageBox.Show("Клиент с таким номером паспорта уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Создаем экземпляр клиента и заполняем его данными
                NewClient = new Client
                {
                    FullName = FullNameTextBox.Text,
                    PassportNumber = PassportNumberTextBox.Text,
                    PhoneNumber = PhoneNumberTextBox.Text,
                    Address = AddressTextBox.Text,
                    CreatedAt = DateTime.Now
                };

                // Сохраняем клиента в базе данных
                context.Clients.Add(NewClient);
                context.SaveChanges();
            }

            MessageBox.Show("Клиент успешно добавлен!");
            DialogResult = true;
            Close();
        }
    }
}
