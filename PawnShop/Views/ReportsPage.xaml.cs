using PawnShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using OfficeOpenXml;
using Microsoft.Win32;
using System.Windows;

namespace PawnShop.Views
{
    public partial class ReportsPage : UserControl
    {
        public ReportsPage()
        {
            InitializeComponent();
            LoadReports();
            // Лицензия EPPlus (нужна для версий 5.x и выше)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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

                    // Активность сотрудников
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

        private void ExportToCsv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new LombardContext())
                {
                    var transactions = context.Transactions.ToList();
                    var dialog = new Microsoft.Win32.SaveFileDialog
                    {
                        FileName = "transactions_report.xlsx",
                        DefaultExt = ".xlsx",
                        Filter = "Excel files (*.xlsx)|*.xlsx"
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        var filePath = dialog.FileName;

                        using (var package = new ExcelPackage(new FileInfo(filePath)))
                        {
                            var worksheet = package.Workbook.Worksheets.Add("Transactions");
                            worksheet.Cells[1, 1].Value = "ID Транзакции";
                            worksheet.Cells[1, 2].Value = "ID Залога";
                            worksheet.Cells[1, 3].Value = "ID Сотрудника";
                            worksheet.Cells[1, 4].Value = "Тип транзакции";
                            worksheet.Cells[1, 5].Value = "Дата";
                            worksheet.Cells[1, 6].Value = "Сумма";

                            using (var range = worksheet.Cells[1, 1, 1, 6])
                            {
                                range.Style.Font.Bold = true;
                                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                                range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            }

                            for (int i = 0; i < transactions.Count; i++)
                            {
                                var transaction = transactions[i];
                                worksheet.Cells[i + 2, 1].Value = transaction.TransactionID;
                                worksheet.Cells[i + 2, 2].Value = transaction.PledgeID;
                                worksheet.Cells[i + 2, 3].Value = transaction.EmployeeId;
                                worksheet.Cells[i + 2, 4].Value = transaction.TransactionType;
                                worksheet.Cells[i + 2, 5].Value = transaction.TransactionDate.ToString("yyyy-MM-dd");
                                worksheet.Cells[i + 2, 6].Value = $"{transaction.Amount:N2} ₽";

                                worksheet.Cells[i + 2, 6].Style.Numberformat.Format = "#,##0.00 ₽";
                            }

                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                            package.Save();
                        }

                        MessageBox.Show($"Отчёт экспортирован в {filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}