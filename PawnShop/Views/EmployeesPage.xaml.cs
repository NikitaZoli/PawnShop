using PawnShop.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PawnShop.Views
{
    public partial class EmployeesPage : UserControl
    {
        public ObservableCollection<Employees> Employees { get; set; } = new ObservableCollection<Employees>();
        private readonly string _currentUserRole;

        public EmployeesPage(string role)
        {
            InitializeComponent();
            _currentUserRole = role;
            LoadEmployees();
            EmployeesDataGrid.ItemsSource = Employees;

            if (_currentUserRole != "Admin")
            {
                AddEmployeeButton.IsEnabled = false;
                AddEmployeeButton.ToolTip = "Только администратор может добавлять сотрудников.";
            }
        }

        private void LoadEmployees()
        {
            try
            {
                using (var db = new LombardContext())
                {
                    var employees = db.Employees.ToList();
                    Employees.Clear();
                    foreach (var employee in employees)
                    {
                        Employees.Add(employee);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке сотрудников: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUserRole != "Admin")
            {
                MessageBox.Show("Только администратор может добавлять сотрудников!", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var addEmployeeWindow = new AddEmployeeWindow();
            if (addEmployeeWindow.ShowDialog() == true)
            {
                try
                {
                    using (var db = new LombardContext())
                    {
                        var newEmployee = addEmployeeWindow.NewEmployee;
                        db.Employees.Add(newEmployee);
                        db.SaveChanges();
                        Employees.Add(newEmployee);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}