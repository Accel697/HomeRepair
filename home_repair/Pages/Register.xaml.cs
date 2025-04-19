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
    /// Логика взаимодействия для Register.xaml
    /// </summary>
    public partial class Register : Page
    {
        public Register()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var newCLient = new clients
            {
                lastNameClient = txtbLastName.Text,
                firstNameClient = txtbFirstName.Text,
                middleNameClient = txtbMiddleName.Text,
                loginClient = txtbLogin.Text,
                passwordClient = pswbPassword.Password,
                emailClient = txtbEmail.Text,
                phoneNumberClient = txtbPhoneNumber.Text
            };

            try
            {
                using (var context = new home_repairEntities1())
                {
                    bool isLoginExists = context.clients.Any(c => c.loginClient == newCLient.loginClient);
                    if (isLoginExists)
                    {
                        MessageBox.Show("Этот логин уже занят. Пожалуйста, выберите другой", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    isLoginExists = context.users.Any(u => u.loginUser == newCLient.loginClient);
                    if (isLoginExists)
                    {
                        MessageBox.Show("Этот логин уже занят. Пожалуйста, выберите другой", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    context.clients.Add(newCLient);
                    context.SaveChanges();
                }
                MessageBox.Show("Регистрация прошла успешно", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
