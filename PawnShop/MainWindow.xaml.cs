using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PawnShop
{
    public partial class MainWindow : Window
    {
        private string _currentUserRole;

        public MainWindow(string role)
        {
            InitializeComponent();
            _currentUserRole = role;
            ConfigureAccess();
        }

        private void ConfigureAccess()
        {
            if (_currentUserRole == "Employee")
            {
                // Поиск вкладки "Сотрудники" через содержимое заголовка
                var employeesTab = MainTabControl.Items
                    .Cast<TabItem>()
                    .FirstOrDefault(t =>
                        (t.Header is StackPanel headerPanel) &&
                        headerPanel.Children.OfType<TextBlock>().Any(tb => tb.Text == "Сотрудники"));

                if (employeesTab != null)
                {
                    MainTabControl.Items.Remove(employeesTab);
                }
            }
        }
    }
}