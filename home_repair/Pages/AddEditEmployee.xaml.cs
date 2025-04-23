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
using static home_repair.Services.Validation;

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
            using (var context = new home_repairEntities6())
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
            if (MessageBox.Show($"Вы действительно хотите удалить сотрудника {_employee.lastNameEmployee} {_employee.firstNameEmployee}?",
        "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new home_repairEntities6())
                    {
                        var employeeToDelete = context.employees
                            .Include("users") 
                            .FirstOrDefault(emp => emp.idEmployee == _employee.idEmployee);

                        if (employeeToDelete == null)
                        {
                            MessageBox.Show("Сотрудник не найден в базе данных", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (employeeToDelete.users != null && employeeToDelete.users.Any())
                        {
                            var userToDelete = employeeToDelete.users.FirstOrDefault();
                            if (userToDelete != null)
                            {
                                context.users.Remove(userToDelete);
                            }
                        }

                        context.employees.Remove(employeeToDelete);

                        context.SaveChanges();

                        MessageBox.Show("Сотрудник успешно удален", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        NavigationService.GoBack();
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
                {
                    MessageBox.Show($"Ошибка при удалении: Возможно, сотрудник связан с другими записями.\n{dbEx.InnerException?.Message}",
                        "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new home_repairEntities6())
            {
                var employeeValidator = new EmployeeValidator();
                var (isEmployeeValid, employeeErrors) = employeeValidator.Validate(_employee);

                if (!isEmployeeValid)
                {
                    MessageBox.Show(string.Join("\n", employeeErrors), "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_employee.idEmployee == 0)
                {
                    context.employees.Add(_employee);
                    NavigationService.GoBack();
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
                try
                {
                    context.SaveChanges();
                    MessageBox.Show("Данные сохранены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnUser_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EmployeeAccount(_employee));
        }
    }
}
