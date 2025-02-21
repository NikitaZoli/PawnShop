using PawnShop.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PawnShop.Views
{
    public partial class PledgesPage : UserControl
    {
        private ObservableCollection<Pledge> Pledges { get; set; }
        public event Action<Pledge> PledgeAdded;

        public PledgesPage()
        {
            InitializeComponent();
            LoadPledges();
            DataContext = this;
        }

        private void LoadPledges()
        {
            using (var context = new LombardContext())
            {
                var pledgesFromDb = context.Pledges.ToList();

                // Если коллекция уже инициализирована, очищаем и добавляем новые данные
                if (Pledges == null)
                {
                    Pledges = new ObservableCollection<Pledge>(pledgesFromDb);
                    PledgesDataGrid.ItemsSource = Pledges;
                }
                else
                {
                    Pledges.Clear();
                    foreach (var pledge in pledgesFromDb)
                    {
                        Pledges.Add(pledge);
                    }
                }
            }
        }


        private void SavePledge_Click(object sender, RoutedEventArgs e)
        {
            var addPledgeWindow = new AddPledgeWindow();
            if (addPledgeWindow.ShowDialog() == true)
            {
                // После успешного добавления добавляем объект в ObservableCollection
                using (var context = new LombardContext())
                {
                    var newPledge = context.Pledges.OrderByDescending(p => p.PledgeID).FirstOrDefault();
                    if (newPledge != null)
                    {
                        Pledges.Add(newPledge);
                    }
                }
            }
        }

        private void EditPledgeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedPledge = PledgesDataGrid.SelectedItem as Pledge;
            if (selectedPledge == null)
            {
                MessageBox.Show("Выберите залог для редактирования");
                return;
            }

            // Открываем окно редактирования
            var editWindow = new AddPledgeWindow(selectedPledge);
            if (editWindow.ShowDialog() == true)
            {
                using (var context = new LombardContext())
                {
                    var updatedPledge = context.Pledges.Find(selectedPledge.PledgeID);
                    if (updatedPledge != null)
                    {
                        // Обновляем свойства существующего объекта
                        selectedPledge.ClientID = updatedPledge.ClientID;
                        selectedPledge.ItemDescription = updatedPledge.ItemDescription;
                        selectedPledge.EstimatedValue = updatedPledge.EstimatedValue;
                        selectedPledge.LoanAmount = updatedPledge.LoanAmount;
                        selectedPledge.InterestRate = updatedPledge.InterestRate;
                        selectedPledge.PledgeDate = updatedPledge.PledgeDate;
                        selectedPledge.DueDate = updatedPledge.DueDate;
                        selectedPledge.Status = updatedPledge.Status;
                    }
                }
            }
        }



        private void DeletePledgeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedPledge = PledgesDataGrid.SelectedItem as Pledge;
            if (selectedPledge == null)
            {
                MessageBox.Show("Выберите залог для удаления");
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить этот залог?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new LombardContext())
                    {
                        var pledge = context.Pledges.Find(selectedPledge.PledgeID);
                        if (pledge != null)
                        {
                            context.Pledges.Remove(pledge);
                            context.SaveChanges();

                            // Удаляем залог из коллекции
                            Pledges.Remove(selectedPledge);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }


        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Поиск...")
            {
                SearchTextBox.Text = string.Empty;
                SearchTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Поиск...";
                SearchTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTextBox.Text == "Поиск...") return;

            using (var context = new LombardContext())
            {
                var query = context.Pledges.AsQueryable();

                if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
                {
                    if (SearchByIdRadio.IsChecked == true)
                    {
                        if (int.TryParse(SearchTextBox.Text, out int id))
                        {
                            query = query.Where(p => p.PledgeID == id);
                        }
                    }
                    else if (SearchByClientIdRadio.IsChecked == true)
                    {
                        if (int.TryParse(SearchTextBox.Text, out int clientId))
                        {
                            query = query.Where(p => p.ClientID == clientId);
                        }
                    }
                    else if (SearchByDescriptionRadio.IsChecked == true)
                    {
                        query = query.Where(p => p.ItemDescription.Contains(SearchTextBox.Text));
                    }
                    else if (SearchByStatusRadio.IsChecked == true)
                    {
                        query = query.Where(p => p.Status.Contains(SearchTextBox.Text));
                    }
                }

                Pledges = new ObservableCollection<Pledge>(query.ToList());
                PledgesDataGrid.ItemsSource = Pledges;
            }
        }

        private void SearchRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox != null && SearchTextBox.Text != "Поиск...")
            {
                SearchTextBox_TextChanged(null, null);
            }
        }
    }
}