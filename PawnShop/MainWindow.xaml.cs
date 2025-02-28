using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PawnShop.Views;

namespace PawnShop
{
    public partial class MainWindow : Window
    {
        private string _currentUserRole;
        private TabItem _clientsTab;
        private TabItem _pledgesTab;
        private TabItem _employeesTab;
        private TabItem _transactionsTab;
        private TabItem _reportsTab;

        public MainWindow(string role)
        {
            InitializeComponent();
            _currentUserRole = role;
            ConfigureAccess();
        }

        private void ConfigureAccess()
        {
            // Ищем все вкладки по их именам
            _clientsTab = MainTabControl.Items.Cast<TabItem>().FirstOrDefault(t => t.Name == "ClientsTab");
            _pledgesTab = MainTabControl.Items.Cast<TabItem>().FirstOrDefault(t => t.Name == "PledgesTab");
            _employeesTab = MainTabControl.Items.Cast<TabItem>().FirstOrDefault(t => t.Name == "EmployeesTab");
            _transactionsTab = MainTabControl.Items.Cast<TabItem>().FirstOrDefault(t => t.Name == "TransactionsTab");
            _reportsTab = MainTabControl.Items.Cast<TabItem>().FirstOrDefault(t => t.Name == "ReportsTab");

            // Проверяем, что все вкладки найдены
            if (_clientsTab == null || _pledgesTab == null || _employeesTab == null ||
                _transactionsTab == null || _reportsTab == null)
            {
                MessageBox.Show("Ошибка: одна из вкладок не найдена в MainTabControl.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Настройка содержимого вкладок
            _clientsTab.Content = new ClientsPage(); 
            _pledgesTab.Content = new PledgesPage(); 
            _transactionsTab.Content = new TransactionsPage(); 
            _employeesTab.Content = new EmployeesPage(_currentUserRole); // Требует роль
            _reportsTab.Content = new ReportsPage();

            // Ограничение доступа
            if (_currentUserRole != "Admin")
            {
                _employeesTab.Visibility = Visibility.Collapsed;
                _reportsTab.Visibility = Visibility.Collapsed;
            }
            else
            {
                _employeesTab.Visibility = Visibility.Visible;
                _reportsTab.Visibility = Visibility.Visible;
            }
        }
    }
}