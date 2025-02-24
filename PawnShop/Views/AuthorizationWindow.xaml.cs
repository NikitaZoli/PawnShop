using System;
using System.Linq;
using System.Windows;
using PawnShop.Models;
using BCrypt.Net;
using PawnShop;
using System.Windows.Threading;
using System.Data.SqlClient;
using System.Data.Entity.Core;

namespace PawnShop.Views
{
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow()
        {
            InitializeComponent();
            // RegisterTestUser убран из конструктора
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = LoginTextBox.Text.Trim();
                string password = PasswordBox.Password;

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    ShowError("Заполните логин и пароль!");
                    return;
                }

                using (var context = new LombardContext())
                {
                    var employee = context.Employees.FirstOrDefault(emp => emp.Login == login);
                    if (employee == null)
                    {
                        ShowError("Пользователь не найден!");
                        return;
                    }

                    Console.WriteLine($"Введённый логин: {login}");
                    Console.WriteLine($"Введённый пароль: {password}");
                    Console.WriteLine($"Хэш в базе: {employee.PasswordHash}");

                    bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, employee.PasswordHash);
                    Console.WriteLine($"Проверка пароля: {isPasswordCorrect}");

                    if (!isPasswordCorrect)
                    {
                        ShowError("Неверный пароль!");
                        return;
                    }

                    MainWindow mainWindow = new MainWindow(employee.Role);
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }

        // Метод для создания пользователя (вызывай вручную, если нужно)
        private void RegisterTestUser()
        {
            using (var context = new LombardContext())
            {
                var existingUser = context.Employees.FirstOrDefault(emp => emp.Login == "testlogin");
                if (existingUser != null)
                {
                    Console.WriteLine($"Удаляем старого пользователя с хэшем: {existingUser.PasswordHash}");
                    context.Employees.Remove(existingUser);
                    context.SaveChanges();
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword("123456");
                Console.WriteLine($"Новый хэш для 123456: {hashedPassword}");

                var newUser = new Employees
                {
                    FullName = "Test User",
                    Position = "Manager",
                    HireDate = DateTime.Now,
                    Login = "testlogin",
                    PasswordHash = hashedPassword,
                    Role = "Admin"
                };
                context.Employees.Add(newUser);
                context.SaveChanges();

                var createdUser = context.Employees.FirstOrDefault(emp => emp.Login == "testlogin");
                Console.WriteLine($"Хэш в базе после создания: {createdUser?.PasswordHash}");
                Console.WriteLine("Тестовый пользователь создан с паролем 123456!");
            }
        }
    }
}
