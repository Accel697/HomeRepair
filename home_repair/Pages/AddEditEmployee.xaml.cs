using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для AddEditEmployee.xaml
    /// </summary>
    public partial class AddEditEmployee : Page
    {
        employees _employee = new employees();

        public AddEditEmployee(employees employee)
        {
            InitializeComponent();
            LoadComboBoxes();

            if (employee != null) 
            {
                _employee = employee;

                btnDelete.Visibility = Visibility.Visible;
                btnUser.Visibility = Visibility.Visible;
            }

            DataContext = _employee;
        }

        private void LoadComboBoxes()
        {
            using (var context = new home_repairEntities1())
            {
                var genders = context.genders.ToList();
                if (genders.Any())
                {
                    cmbGender.ItemsSource = genders;
                    cmbGender.DisplayMemberPath = "titleGender";
                    cmbGender.SelectedValuePath = "idGender";
                }
                else
                {
                    MessageBox.Show("Полы не найдены");
                }

                var jobs = context.job_titles.ToList();
                if (jobs.Any())
                {
                    cmbPositionAtWork.ItemsSource = jobs;
                    cmbPositionAtWork.DisplayMemberPath = "titleJob";
                    cmbPositionAtWork.SelectedValuePath = "idJob";
                }
                else
                {
                    MessageBox.Show("Должности не найдены");
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы действительно хотите удалить {_employee.lastNameEmployee} {_employee.firstNameEmployee}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new home_repairEntities1())
                    {
                        var user = context.users.FirstOrDefault(u => u.employeeData == _employee.idEmployee);
                        if (user != null)
                        {
                            context.users.Remove(user);
                        }
                        context.employees.Remove(_employee);
                        context.SaveChanges();
                        MessageBox.Show("Запись удалена!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        NavigationService.GoBack();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new home_repairEntities1())
                {
                    if (_employee.idEmployee == 0)
                    {
                        context.employees.Add(_employee);
                    }
                    else
                    {
                        var employeeInDb = context.employees.FirstOrDefault(em => em.idEmployee == _employee.idEmployee);
                        if (employeeInDb != null)
                        {
                            employeeInDb.firstNameEmployee = _employee.firstNameEmployee;
                            employeeInDb.lastNameEmployee = _employee.lastNameEmployee;
                            employeeInDb.middleNameEmployee = _employee.middleNameEmployee;
                            employeeInDb.birthDateEmployee = _employee.birthDateEmployee;
                            employeeInDb.genderEmployee = _employee.genderEmployee;
                            employeeInDb.positionAtWork = _employee.positionAtWork;
                            employeeInDb.wages = _employee.wages;
                            employeeInDb.phoneNumberEmployee = _employee.phoneNumberEmployee;
                        }
                        else
                        {
                            MessageBox.Show("Сотрудник не найден для обновления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    context.SaveChanges();
                    MessageBox.Show("Данные сохранены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUser_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
