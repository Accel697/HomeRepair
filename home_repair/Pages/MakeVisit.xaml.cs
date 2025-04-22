using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
using static home_repair.Services.Validation;

namespace home_repair.Pages
{
    /// <summary>
    /// Логика взаимодействия для MakeVisit.xaml
    /// </summary>
    public partial class MakeVisit : Page
    {
        visits _visit = new visits();
        List<services> _selectedServices = new List<services>();

        public MakeVisit(clients client)
        {
            InitializeComponent();

            if (client != null)
            {
                _visit.clientVisit = client.idClient;
                _visit.clients = client;
                if (client.phoneNumberClient != null) 
                    _visit.phoneNumberVisit = client.phoneNumberClient;
            }
            _visit.statusVisit = 1;

            using (var context = new home_repairEntities1())
            {
                var allServices = context.services.ToList();
                cmbService.ItemsSource = allServices;
                cmbService.DisplayMemberPath = "titleService";
                cmbService.SelectedValuePath = "idService";
            }

            DataContext = _visit;
        }

        private void UpdateServicesList()
        {
            ServicesListView.ItemsSource = null;
            ServicesListView.ItemsSource = _selectedServices;
        }

        private void CalculateTotalPrice()
        {
            decimal total = _selectedServices.Sum(s => s.priceService);
            txtPrice.Text = total.ToString("N2") + " руб";
            _visit.priceVisit = total;
        }

        private void btnSearchMaster_Click(object sender, RoutedEventArgs e)
        {
            string searchText = tbSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                MessageBox.Show("Введите ФИО для поиска");
                return;
            }

            using (var context = new home_repairEntities1())
            {
                var master = context.employees
                    .FirstOrDefault(emp => emp.positionAtWork == 3 &&
                        (emp.lastNameEmployee.Contains(searchText) ||
                        (emp.firstNameEmployee.Contains(searchText) ||
                        (emp.middleNameEmployee != null && emp.middleNameEmployee.Contains(searchText)))));

                if (master != null)
                {
                    var result = MessageBox.Show(
                        $"Найден мастер: {master.lastNameEmployee} {master.firstNameEmployee} {master.middleNameEmployee}\n\n" +
                        "Добавить этого мастера в вызов?",
                        "Результат поиска", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _visit.masterVisit = master.idEmployee;
                        _visit.employees = master;
                        DataContext = null;
                        DataContext = _visit;
                    }
                }
                else
                {
                    MessageBox.Show("Мастер с таким ФИО не найден");
                }
            }
        }

        private void btnAddVisit_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new home_repairEntities1())
            {
                var visitValidator = new VisitValidator();
                var (isVisitValid, visitErrors) = visitValidator.Visit(_visit);

                if (!isVisitValid)
                {
                    MessageBox.Show(string.Join("\n", visitErrors), "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                context.visits.Add(_visit);
                foreach (var service in _selectedServices)
                {
                    context.visits_services.Add(new visits_services
                    {
                        visit = _visit.idVisit,
                        service = service.idService
                    });
                }
                try
                {
                    context.SaveChanges();
                    NavigationService.GoBack();
                    MessageBox.Show("Данные сохранены успешно!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnDeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (ServicesListView.SelectedItem is services selectedService)
            {
                _selectedServices.Remove(selectedService);
                UpdateServicesList();
                CalculateTotalPrice();
            }
            else
            {
                MessageBox.Show("Выберите услугу для удаления");
            }
        }

        private void btnAddService_Click(object sender, RoutedEventArgs e)
        {
            if (cmbService.SelectedItem is services selectedService)
            {
                if (_selectedServices.Any(s => s.idService == selectedService.idService))
                {
                    MessageBox.Show("Эта услуга уже добавлена");
                    return;
                }

                _selectedServices.Add(selectedService);
                UpdateServicesList();
                CalculateTotalPrice();
            }
            else
            {
                MessageBox.Show("Выберите услугу из списка");
            }
        }
    }
}
