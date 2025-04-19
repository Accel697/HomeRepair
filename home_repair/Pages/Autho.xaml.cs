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
using System.Windows.Threading;
using home_repair.Model;

namespace home_repair.Pages
{
    /// <summary>
    /// Логика взаимодействия для Autho.xaml
    /// </summary>
    public partial class Autho : Page
    {

        public Autho()
        {
            InitializeComponent();
        }

        private void btnEnterGuests_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(null, null, null);
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = txtbLogin.Text.Trim();
            string password = pswbPassword.Password.Trim();

            using (var context = new home_repairEntities1())
            {
                var user = context.users.Where(x => x.loginUser == login && x.passwordUser == password).FirstOrDefault();
                var client = context.clients.Where(x => x.loginClient == login && x.passwordClient == password).FirstOrDefault();

                if (user != null)
                {
                    txtbLogin.Clear();
                    pswbPassword.Clear();
                    LoadPage(null, user, user.employees.positionAtWork.ToString());
                }
                else if (client != null)
                {
                    txtbLogin.Clear();
                    pswbPassword.Clear();
                    LoadPage(client, null, null);
                }
                else
                {
                    MessageBox.Show("Вы ввели логин или пароль неверно!");

                    pswbPassword.Clear();
                }
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Register());
        }

        private void LoadPage(clients client, users user, string role)
        {
            switch (role)
            {
                case "1":
                    NavigationService.Navigate(new Admin(user));
                    break;
                case "2":
                    NavigationService.Navigate(new Manager(user));
                    break;
                default:
                    NavigationService.Navigate(new Client(client));
                    break;
            }
        }
    }
}
