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
using home_repair.Pages;

namespace home_repair.Pages
{
    /// <summary>
    /// Логика взаимодействия для Client.xaml
    /// </summary>
    public partial class Client : Page
    {
        clients _client = new clients();
        private List<employees> _employees;
        private List<employees> _filteredEmployees;
        private List<services> _services;
        private List<services> _filteredServices;
        private List<string> _jobTitles;

        public Client(clients client)
        {
            InitializeComponent();
            _client = client;
            DataContext = this;
            LoadData();
        }

        private void LoadData()
        {
            var context = new home_repairEntities6();
            _employees = context.employees.Where(emp => emp.positionAtWork != 1 && emp.positionAtWork != 2).ToList();
            EmployeesListView.ItemsSource = _employees;

            _services = context.services.ToList();
            ServicesListView.ItemsSource = _services;

            _jobTitles = context.job_titles.Where(j => j.idJob != 1 && j.idJob != 2).Select(j => j.titleJob).Distinct().ToList();
            _jobTitles.Insert(0, "Все должности");
            cbJobTitle.ItemsSource = _jobTitles;
            cbJobTitle.SelectedIndex = 0;
        }

        private void FilterEmployees()
        {
            string searchMasterText = tbSearchMaster.Text.ToLower();
            string selectedJobTitle = cbJobTitle.SelectedItem as string;

            _filteredEmployees = _employees.Where(emp =>
                (emp.lastNameEmployee + " " + emp.firstNameEmployee + " " + emp.middleNameEmployee).ToLower().Contains(searchMasterText) &&
                (selectedJobTitle == "Все должности" || emp.job_titles.titleJob == selectedJobTitle)).ToList();

            EmployeesListView.ItemsSource = null;
            EmployeesListView.ItemsSource = _filteredEmployees;
        }

        private void FilterServices()
        {
            string searchServiceText = tbSearchService.Text.ToLower();

            _filteredServices = _services.Where(s => s.titleService.ToLower().Contains(searchServiceText)).ToList();

            ServicesListView.ItemsSource = null;
            ServicesListView.ItemsSource = _filteredServices;
        }

        private void tbSearchMaster_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterEmployees();
        }

        private void cbJobTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterEmployees();
        }

        private void EmployeesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EmployeesListView.SelectedItem is employees selectedEmployee)
            {
                if (selectedEmployee != null)
                {
                    NavigationService.Navigate(new MasterReviews(selectedEmployee, _client));
                }
                else
                {
                    MessageBox.Show($"Мастер не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void tbSearchService_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterServices();
        }

        private void AddVisitButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new MakeVisit(_client));
        }
    }
}
