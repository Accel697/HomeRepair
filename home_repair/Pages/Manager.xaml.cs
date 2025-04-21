using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using home_repair.Model;

namespace home_repair.Pages
{
    /// <summary>
    /// Логика взаимодействия для Manager.xaml
    /// </summary>
    public partial class Manager : Page
    {
        private List<visits> _visits = new List<visits>();
        private List<visits> _filteredVisits = new List<visits>();
        private List<string> _visitStatuses = new List<string>();

        public Manager(users user)
        {
            InitializeComponent();
            DataContext = this;
            LoadData();
        }

        private void LoadData()
        {
            var context = new home_repairEntities1();
            _visits = context.visits.ToList();
            VisitsListView.ItemsSource = _visits;

            _visitStatuses = context.visit_statuses.Select(s => s.titleStatus).Distinct().ToList();
            _visitStatuses.Insert(0, "Все вызовы");
            cbStatusVisit.ItemsSource = _visitStatuses;
            cbStatusVisit.SelectedIndex = 0;
        }

        private void FilterVisits()
        {
            string searchText = tbSearch.Text.ToLower();
            string selectedVisitStatus = cbStatusVisit.SelectedItem as string;

            _filteredVisits = _visits.Where(vis =>
                ((vis.employees.lastNameEmployee + " " + vis.employees.firstNameEmployee + " " + vis.employees.middleNameEmployee).ToLower().Contains(searchText) ||
                (vis.clients.lastNameClient + " " + vis.clients.firstNameClient + " " + vis.clients.middleNameClient).ToLower().Contains(searchText)) &&
                (selectedVisitStatus == "Все вызовы" || vis.visit_statuses.titleStatus == selectedVisitStatus)).ToList();

            VisitsListView.ItemsSource = null;
            VisitsListView.ItemsSource = _filteredVisits;
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterVisits();
        }

        private void cbStatusVisit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterVisits();
        }

        private void AddVisitButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditVisit(null));
        }

        private void VisitsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VisitsListView.SelectedItem is visits selectedVisit)
            {
                if (selectedVisit != null)
                {
                    NavigationService.Navigate(new AddEditVisit(selectedVisit));
                }
                else
                {
                    MessageBox.Show($"Вызов не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = new home_repairEntities1();
            if (Visibility == Visibility.Visible)
            {
                context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                VisitsListView.ItemsSource = context.visits.ToList();
            }
        }
    }
}
