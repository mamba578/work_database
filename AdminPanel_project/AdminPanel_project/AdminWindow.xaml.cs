using AdminPanel_project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AdminPanel_project
{
    public partial class AdminWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private List<Tovar> allProducts = new List<Tovar>();
        private User currentUser;

        public AdminWindow(User u)
        {
            InitializeComponent();
            currentUser = u;

            LoadCategories();
            LoadProducts();
        }

        private async void LoadCategories()
        {
            try
            {
                string json = await client.GetStringAsync("http://localhost:5050/api/Category/AllCategories");
                var categories = JsonConvert.DeserializeObject<List<Category>>(json);

                categories.Insert(0, new Category { Id = 0, Title = "Все категории" });

                FilterCategoryCb.ItemsSource = categories;
                FilterCategoryCb.DisplayMemberPath = "Title";
                FilterCategoryCb.SelectedValuePath = "Id";
                FilterCategoryCb.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}");
            }
        }

        private async void LoadProducts()
        {
            try
            {
                string url = "http://localhost:5050/api/Tovar/AllProductsAdmin";
                string json = await client.GetStringAsync(url);
                allProducts = JsonConvert.DeserializeObject<List<Tovar>>(json);
                ProductsListBox.ItemsSource = allProducts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}\nПроверьте, запущен ли бэкенд.", "Ошибка API");
            }
        }

        private async void ApplyFilters()
        {
            try
            {
                string search = SearchTb.Text.Trim().ToLower();
                int categoryId = 0;

                if (FilterCategoryCb.SelectedValue != null)
                {
                    categoryId = (int)FilterCategoryCb.SelectedValue;
                }

                if (string.IsNullOrEmpty(search) && categoryId == 0)
                {
                    LoadProducts();
                    return;
                }

                string url = "http://localhost:5050/api/Tovar/Filter?";

                if (categoryId > 0)
                    url += $"categoryId={categoryId}&";

                if (!string.IsNullOrEmpty(search))
                    url += $"search={search}";

                string json = await client.GetStringAsync(url);
                allProducts = JsonConvert.DeserializeObject<List<Tovar>>(json);
                ProductsListBox.ItemsSource = allProducts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}");
            }
        }

        private void SearchTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void FilterCategoryCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void AddBt_Click(object sender, RoutedEventArgs e)
        {
            AddEditTovarWindow window = new AddEditTovarWindow(null);
            bool? result = window.ShowDialog();

            if (result == true)
            {
                ApplyFilters();
            }
        }

        private void EditBt_Click(object sender, RoutedEventArgs e)
        {
            Tovar product = ProductsListBox.SelectedItem as Tovar;

            if (product == null)
            {
                MessageBox.Show("Выберите товар для редактирования");
                return;
            }

            AddEditTovarWindow window = new AddEditTovarWindow(product);
            bool? result = window.ShowDialog();

            if (result == true)
            {
                ApplyFilters();
            }
        }

        private async void DeleteBt_Click(object sender, RoutedEventArgs e)
        {
            Tovar product = ProductsListBox.SelectedItem as Tovar;

            if (product == null)
            {
                MessageBox.Show("Выберите товар для удаления");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Вы уверены, что хотите перевести товар '{product.Title}' в статус 'Удален'?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                string url = $"http://localhost:5050/api/Tovar/DeleteProduct/{product.Id}";
                HttpResponseMessage response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Товар успешно переведен в статус 'Удален'");
                    ApplyFilters();
                }
                else
                {
                    string serverError = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка сервера при изменении статуса:\n{serverError}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка соединения при архивации: {ex.Message}");
            }
        }

        private void ProductsBt_Click(object sender, RoutedEventArgs e)
        {
            SearchTb.Text = "";
            FilterCategoryCb.SelectedIndex = 0;
            LoadProducts();
        }

        private void OrdersBt_Click(object sender, RoutedEventArgs e)
        {
            OrdersWindow window = new OrdersWindow(currentUser);
            window.Show();
            Close();
        }

        private void ExitBt_Click(object sender, RoutedEventArgs e)
        {
            MainWindow auth = new MainWindow();
            auth.Show();
            Close();
        }

        private void AnaliticBt_Click(object sender, RoutedEventArgs e)
        {
            var anali = new AnaliticalWindow(currentUser);
            anali.Show();
            Close();
        }

        private void PointsBt_Click(object sender, RoutedEventArgs e)
        {
            var points = new PointWindow(currentUser);
            points.Show();
            Close();
        }
    }
}