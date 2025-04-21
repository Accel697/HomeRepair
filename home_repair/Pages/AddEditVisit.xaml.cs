using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
    /// Логика взаимодействия для AddEditVisit.xaml
    /// </summary>
    public partial class AddEditVisit : Page
    {
        visits _visit = new visits();
        List<services> _selectedServices = new List<services>();

        public AddEditVisit(visits visit)
        {
            InitializeComponent();

            if (visit != null)
            {
                _visit = visit;
                btnDelete.Visibility = Visibility.Visible;
            }

            LoadData();
            DataContext = _visit;
        }

        private void LoadData()
        {
            using (var context = new home_repairEntities1())
            {
                var statuses = context.visit_statuses.ToList();
                cmbStatus.ItemsSource = statuses;
                cmbStatus.DisplayMemberPath = "titleStatus";
                cmbStatus.SelectedValuePath = "idStatus";

                var allServices = context.services.ToList();
                cmbService.ItemsSource = allServices;
                cmbService.DisplayMemberPath = "titleService";
                cmbService.SelectedValuePath = "idService";

                if (_visit.idVisit != 0)
                {
                    var services = context.visits_services
                        .Where(vs => vs.visit == _visit.idVisit)
                        .Select(vs => vs.service)
                        .ToList();

                    foreach (var service in services)
                    {
                        var serviceInDb = context.services.FirstOrDefault(s => s.idService == service);
                        _selectedServices.Add(serviceInDb);
                    }

                    UpdateServicesList();
                    CalculateTotalPrice();
                }
            }
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Удалить вызов №{_visit.idVisit}?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new home_repairEntities1())
                    {
                        var visitToDelete = context.visits
                            .Include(v => v.visits_services)
                            .FirstOrDefault(v => v.idVisit == _visit.idVisit);

                        if (visitToDelete != null)
                        {
                            context.visits_services.RemoveRange(visitToDelete.visits_services);
                            context.visits.Remove(visitToDelete);
                            context.SaveChanges();

                            MessageBox.Show("Вызов удален");
                            NavigationService.GoBack();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new home_repairEntities1())
                {
                    if (_visit.idVisit == 0)
                    {
                        context.visits.Add(_visit);
                    }
                    else
                    {
                        var existingVisit = context.visits
                            .Include(v => v.visits_services)
                            .FirstOrDefault(v => v.idVisit == _visit.idVisit);

                        if (existingVisit != null)
                        {
                            existingVisit.masterVisit = _visit.masterVisit;
                            existingVisit.clientVisit = _visit.clientVisit;
                            existingVisit.phoneNumberVisit = _visit.phoneNumberVisit;
                            existingVisit.adressVisit = _visit.adressVisit;
                            existingVisit.datetimeVisit = _visit.datetimeVisit;
                            existingVisit.priceVisit = _visit.priceVisit;
                            existingVisit.commentVisit = _visit.commentVisit;
                            existingVisit.statusVisit = _visit.statusVisit;

                            context.visits_services.RemoveRange(existingVisit.visits_services);
                        }
                    }

                    foreach (var service in _selectedServices)
                    {
                        context.visits_services.Add(new visits_services
                        {
                            visit = _visit.idVisit,
                            service = service.idService 
                        });
                    }

                    context.SaveChanges();
                    MessageBox.Show("Данные сохранены успешно!");
                    NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
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
