using PawnShop.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PawnShop.Views
{
    public partial class ClientsPage : UserControl
    {
        public ObservableCollection<Client> Clients { get; set; } = new ObservableCollection<Client>();
        public ObservableCollection<Client> FilteredClients { get; set; } = new ObservableCollection<Client>();
        public ClientsPage()
        {
            InitializeComponent();
            LoadClients();
            ClientsDataGrid.ItemsSource = Clients;
            DataContext = this;
            // Копируем данные из Clients в FilteredClients
            foreach (var client in Clients)
            {
                FilteredClients.Add(client);
            }

            // Устанавливаем FilteredClients как источник данных для DataGrid
            ClientsDataGrid.ItemsSource = FilteredClients;
        }

        private void LoadClients()
        {
            using (var db = new LombardContext())
            {
                var clients = db.Clients.ToList();
                foreach (var client in clients)
                {
                    // Проверяем, добавлен ли уже клиент в коллекцию
                    if (!Clients.Any(c => c.ClientID == client.ClientID))
                    {
                        Clients.Add(client);
                    }
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string fullNameFilter = SearchFullNameTextBox.Text?.Trim().ToLower() ?? "";
            string passportFilter = SearchPassportTextBox.Text?.Trim().ToLower() ?? "";
            string phoneFilter = SearchPhoneTextBox.Text?.Trim().ToLower() ?? "";

            // Если все фильтры пустые, показываем все записи
            if (string.IsNullOrWhiteSpace(fullNameFilter) &&
                string.IsNullOrWhiteSpace(passportFilter) &&
                string.IsNullOrWhiteSpace(phoneFilter))
            {
                ResetFilter();
                return;
            }

            // Фильтрация клиентов
            var filtered = Clients.Where(client =>
                (string.IsNullOrWhiteSpace(fullNameFilter) || client.FullName.ToLower().Contains(fullNameFilter)) &&
                (string.IsNullOrWhiteSpace(passportFilter) || client.PassportNumber.ToLower().Contains(passportFilter)) &&
                (string.IsNullOrWhiteSpace(phoneFilter) || client.PhoneNumber.ToLower().Contains(phoneFilter))
            ).ToList();

            // Обновляем коллекцию фильтрованных клиентов
            FilteredClients.Clear();
            foreach (var client in filtered)
            {
                FilteredClients.Add(client);
            }
        }

        private void ResetFilter()
        {
            FilteredClients.Clear();
            foreach (var client in Clients)
            {
                FilteredClients.Add(client);
            }

            // Устанавливаем исходный список для DataGrid
            ClientsDataGrid.ItemsSource = FilteredClients;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем все поля поиска
            SearchFullNameTextBox.Text = string.Empty;
            SearchPassportTextBox.Text = string.Empty;
            SearchPhoneTextBox.Text = string.Empty;

            ResetFilter();
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            var addClientWindow = new AddClientWindow();
            if (addClientWindow.ShowDialog() == true)
            {
                var newClient = addClientWindow.NewClient;

                if (newClient != null)
                {
                    // Обновляем коллекцию
                    Clients.Add(newClient);
                    // Если фильтр не применен, обновляем и фильтрованную коллекцию
                    if (string.IsNullOrWhiteSpace(SearchFullNameTextBox.Text))
                    {
                        FilteredClients.Add(newClient);
                    }
                    else
                    {
                        // Проверяем, соответствует ли новый клиент фильтру
                        string fullNameFilter = SearchFullNameTextBox.Text.Trim().ToLower();
                        if (newClient.FullName.ToLower().Contains(fullNameFilter))
                        {
                            FilteredClients.Add(newClient);
                        }
                    }
                }
            }
        }


        private void EditClientMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Client selectedClient)
            {
                var editClientWindow = new AddClientWindow
                {
                    Title = "Редактировать клиента"
                };

                // Заполняем окно текущими данными клиента
                editClientWindow.FullNameTextBox.Text = selectedClient.FullName;
                editClientWindow.PassportNumberTextBox.Text = selectedClient.PassportNumber;
                editClientWindow.PhoneNumberTextBox.Text = selectedClient.PhoneNumber;

                if (editClientWindow.ShowDialog() == true)
                {
                    using (var db = new LombardContext())
                    {
                        var clientToUpdate = db.Clients.Find(selectedClient.ClientID);
                        if (clientToUpdate != null)
                        {
                            clientToUpdate.FullName = editClientWindow.FullNameTextBox.Text;
                            clientToUpdate.PassportNumber = editClientWindow.PassportNumberTextBox.Text;
                            clientToUpdate.PhoneNumber = editClientWindow.PhoneNumberTextBox.Text;

                            //db.SaveChanges();

                            // Обновляем данные в коллекции
                            selectedClient.FullName = clientToUpdate.FullName;
                            selectedClient.PassportNumber = clientToUpdate.PassportNumber;
                            selectedClient.PhoneNumber = clientToUpdate.PhoneNumber;
                        }
                    }
                }
            }
        }

        private void DeleteClientMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is Client selectedClient)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить клиента {selectedClient.FullName}?",
                                             "Подтверждение удаления",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new LombardContext())
                    {
                        var clientToDelete = db.Clients.Find(selectedClient.ClientID);
                        if (clientToDelete != null)
                        {
                            db.Clients.Remove(clientToDelete);
                            db.SaveChanges();
                        }
                    }

                    // Удаляем клиента из обеих коллекций
                    Clients.Remove(selectedClient);
                    FilteredClients.Remove(selectedClient);
                }
            }
        }

    }
}

