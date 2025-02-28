using PawnShop.Models;
using System;
using System.Windows;
using BCrypt.Net;
using System.Linq;
using System.Windows.Controls;

namespace PawnShop.Views
{
    public partial class AddEmployeeWindow : Window
    {
        public Employees NewEmployee { get; private set; }

        public AddEmployeeWindow()
        {
            InitializeComponent();
            HireDatePicker.SelectedDate = DateTime.Today;
            RoleComboBox.SelectedIndex = 1; // По умолчанию "User"
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PositionTextBox.Text) ||
                string.IsNullOrWhiteSpace(LoginTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                RoleComboBox.SelectedItem == null)
            {
                ShowError("Пожалуйста, заполните все поля.");
                return;
            }

            try
            {
                string fullName = FullNameTextBox.Text.Trim();
                string position = PositionTextBox.Text.Trim();
                string login = LoginTextBox.Text.Trim();
                string password = PasswordBox.Password;
                string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                Console.WriteLine($"Попытка добавить сотрудника: Логин={login}, Роль={role}");

                using (var context = new LombardContext())
                {
                    if (context.Employees.Any(emp => emp.Login == login))
                    {
                        ShowError("Пользователь с таким логином уже существует!");
                        return;
                    }

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                    Console.WriteLine($"Хэш для пароля {password}: {hashedPassword}");

                    NewEmployee = new Employees
                    {
                        FullName = fullName,
                        Position = position,
                        HireDate = HireDatePicker.SelectedDate ?? DateTime.Today,
                        Login = login,
                        PasswordHash = hashedPassword,
                        Role = role
                    };

                    context.Employees.Add(NewEmployee);
                    context.SaveChanges();
                    Console.WriteLine("Сотрудник успешно добавлен в базу.");

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении сотрудника: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Внутреннее исключение: {ex.InnerException.Message}");
                }
                ShowError($"Произошла ошибка при создании сотрудника: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }
    }
}