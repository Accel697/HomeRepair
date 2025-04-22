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
    /// Логика взаимодействия для MasterReviews.xaml
    /// </summary>
    public partial class MasterReviews : Page
    {
        reviews _review = new reviews();
        private List<reviews> _reviews = new List<reviews>();
        private List<reviews> _filteredReviews = new List<reviews>();
        private string[] filterGrades = { "Все отзывы", "5", "4", "3", "2", "1" };
        private long[] grades = { 5, 4, 3, 2, 1 };

        public MasterReviews(employees employee, clients client)
        {
            InitializeComponent();
            _review.masterReview = employee.idEmployee;
            if (client != null)
            {
                _review.clientReview = client.idClient;
                tbGrade.Visibility = Visibility.Visible;
                txtGrade.Visibility = Visibility.Visible;
                tbText.Visibility = Visibility.Visible;
                txtText.Visibility = Visibility.Visible;
                btnAddReview.Visibility = Visibility.Visible;
            }
            else
            {
                tbNoCLient.Visibility = Visibility.Visible;
            }
            DataContext = _review;
            LoadData();
            cbGrade.ItemsSource = filterGrades;
            cbGrade.SelectedIndex = 0;
        }

        private void LoadData()
        {
            var context = new home_repairEntities1();
            _reviews = context.reviews.Where(r => r.masterReview == _review.masterReview).ToList();
            ReviewsListView.ItemsSource = _reviews;
        }

        private void FilterReviews()
        {
            string selectedReview = cbGrade.SelectedItem as string;

            if (selectedReview != "Все отзывы")
            {
                _filteredReviews = _reviews.Where(r => r.gradeReview.ToString() == selectedReview).ToList();
            }
            else
            {
                _filteredReviews = _reviews;
            }
            ReviewsListView.ItemsSource = null;
            ReviewsListView.ItemsSource = _filteredReviews;
        }

        private void cbGrade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterReviews();
        }

        private void btnAddReview_Click(object sender, RoutedEventArgs e)
        {
            _review.gradeReview = int.Parse(txtGrade.Text);
            _review.textReview = txtText.Text;

            var reviewValidator = new ReviewValidator();
            var (isReviewValid, reviewErrors) = reviewValidator.Validate(_review);

            if (!isReviewValid)
            {
                MessageBox.Show(string.Join("\n", reviewErrors), "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new home_repairEntities1())
                {
                    context.reviews.Add(_review);
                    context.SaveChanges();
                }
                MessageBox.Show("Отзыв успешно отправлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                txtGrade.Clear();
                txtText.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оставлении отзыва: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
