using PawnShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PawnShop.Views
{
    public partial class ReportsPage : UserControl
    {
        public ReportsPage()
        {
            InitializeComponent();
            LoadReports();
        }

        private void LoadReports()
        {
            UpdateReports(null, null); // Инициализация с текущей датой
        }

        private void UpdateReports_Click(object sender, RoutedEventArgs e)
        {
            UpdateReports(StartDatePicker.SelectedDate, EndDatePicker.SelectedDate);
        }

        private void UpdateReports(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                using (var context = new LombardContext())
                {
                    // Преобразование null в минимальные/максимальные даты
                    DateTime effectiveStartDate = startDate ?? DateTime.MinValue;
                    DateTime effectiveEndDate = endDate ?? DateTime.MaxValue;

                    // Общая сумма транзакций
                    decimal totalAmount = context.Transactions
                        .Where(t => t.TransactionDate >= effectiveStartDate && t.TransactionDate <= effectiveEndDate)
                        .Sum(t => (decimal?)t.Amount) ?? 0;
                    TotalTransactionsAmount.Text = $"{totalAmount:N0} ₽";

                    // Количество активных залогов
                    int activePledgesCount = context.Pledges
                        .Count(p => p.Status == "Активный" && p.PledgeDate >= effectiveStartDate && p.PledgeDate <= effectiveEndDate);
                    ActivePledgesCount.Text = activePledgesCount.ToString();

                    // Активность сотрудников (загрузка в память)
                    var transactions = context.Transactions
                        .Where(t => t.TransactionDate >= effectiveStartDate && t.TransactionDate <= effectiveEndDate)
                        .ToList();

                    var employeeActivity = (from t in transactions
                                            join e in context.Employees.ToList() on t.EmployeeId equals e.EmployeeID into empGroup
                                            from e in empGroup.DefaultIfEmpty()
                                            group t by new { EmployeeId = t.EmployeeId, FullName = e?.FullName ?? "Неизвестный сотрудник" } into g
                                            where g.Key.EmployeeId != null
                                            select new
                                            {
                                                FullName = g.Key.FullName,
                                                TransactionCount = g.Count()
                                            })
                                          .OrderByDescending(x => x.TransactionCount)
                                          .ToList();

                    EmployeeActivityGrid.ItemsSource = employeeActivity;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отчётов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
