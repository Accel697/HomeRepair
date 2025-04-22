using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
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
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Page
    {
        private List<employees> _employees;
        private List<employees> _filteredEmployees;
        private List<string> _jobTitles;

        public Admin(users user)
        {
            InitializeComponent();
            DataContext = this;
            LoadData();
        }

        private void LoadData()
        {
            var context = new home_repairEntities1();
            _employees = context.employees.ToList();
            EmployeesListView.ItemsSource = _employees;

            _jobTitles = context.job_titles.Select(j => j.titleJob).Distinct().ToList();
            _jobTitles.Insert(0, "Все должности");
            cbJobTitle.ItemsSource = _jobTitles;
            cbJobTitle.SelectedIndex = 0;
        }

        private void FilterEmployees()
        {
            string searchText = tbSearch.Text.ToLower();
            string selectedJobTitle = cbJobTitle.SelectedItem as string;

            _filteredEmployees = _employees.Where(emp =>
                (emp.lastNameEmployee + " " + emp.firstNameEmployee + " " + (emp.middleNameEmployee ?? string.Empty)).ToLower().Contains(searchText) &&
                (selectedJobTitle == "Все должности" || emp.job_titles.titleJob == selectedJobTitle)).ToList();

            EmployeesListView.ItemsSource = null;
            EmployeesListView.ItemsSource = _filteredEmployees;
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterEmployees();
        }

        private void cbJobTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterEmployees();
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditEmployee(null));
        }

        private void EmployeesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EmployeesListView.SelectedItem is employees selectedEmployee)
            {
                if (selectedEmployee != null)
                {
                    NavigationService.Navigate(new AddEditEmployee(selectedEmployee));
                }
                else
                {
                    MessageBox.Show($"Сотрудник не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = new home_repairEntities1();
            if (Visibility == Visibility.Visible)
            {
                context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                EmployeesListView.ItemsSource = context.employees.ToList();
            }
        }
    }
}
