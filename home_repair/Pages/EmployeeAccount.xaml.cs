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
using static home_repair.Services.Validation;

namespace home_repair.Pages
{
    /// <summary>
    /// Логика взаимодействия для EmployeeAccount.xaml
    /// </summary>
    public partial class EmployeeAccount : Page
    {
        users _user = new users();

        public EmployeeAccount(employees employee)
        {
            InitializeComponent();

            using (var context = new home_repairEntities1())
            {
                _user = context.users.FirstOrDefault(u => u.employeeData == employee.idEmployee);
            }

            if (_user == null)
            {
                _user = new users();
                _user.employeeData = employee.idEmployee;
            }

            DataContext = _user;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new home_repairEntities1())
            {
                var userValidator = new UserValidator();
                var (isUserValid, userErrors) = userValidator.Validate(_user);

                if (!isUserValid)
                {
                    MessageBox.Show(string.Join("\n", userErrors), "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_user.idUser == 0)
                {
                    context.users.Add(_user);
                }
                else
                {
                    var userInDb = context.users.FirstOrDefault(u => u.idUser == _user.idUser);
                    if (userInDb != null)
                    {
                        userInDb.loginUser = _user.loginUser;
                        userInDb.passwordUser = _user.passwordUser;
                    }
                    else
                    {
                        MessageBox.Show("Аккаунт сотрудника не найден для обновления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
