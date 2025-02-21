using PawnShop.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PawnShop.Views
{
    public partial class EmployeesPage : UserControl
    {
        public ObservableCollection<Employees> Employees { get; set; } = new ObservableCollection<Employees>();

        public EmployeesPage()
        {
            InitializeComponent();
            LoadEmployees();
            EmployeesDataGrid.ItemsSource = Employees;
        }

        private void LoadEmployees()
        {
            using (var db = new LombardContext())
            {
                var employees = db.Employees.ToList();
                foreach (var employee in employees)
                {
                    Employees.Add(employee);
                }
            }
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            var addEmployeeWindow = new AddEmployeeWindow();
            if (addEmployeeWindow.ShowDialog() == true)
            {
                using (var db = new LombardContext())
                {
                    db.Employees.Add(addEmployeeWindow.NewEmployee);
                    db.SaveChanges();
                    Employees.Add(addEmployeeWindow.NewEmployee);
                }
            }
        }
    }
}