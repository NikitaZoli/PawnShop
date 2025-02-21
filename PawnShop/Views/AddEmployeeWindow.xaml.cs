using PawnShop.Models;
using System;
using System.Windows;

namespace PawnShop.Views
{
    public partial class AddEmployeeWindow : Window
    {
        public Employees NewEmployee { get; private set; }

        public AddEmployeeWindow()
        {
            InitializeComponent();
            HireDatePicker.SelectedDate = DateTime.Today;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PositionTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                NewEmployee = new Employees
                {
                    FullName = FullNameTextBox.Text.Trim(),
                    Position = PositionTextBox.Text.Trim(),
                    HireDate = HireDatePicker.SelectedDate ?? DateTime.Today
                };

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при создании сотрудника: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}