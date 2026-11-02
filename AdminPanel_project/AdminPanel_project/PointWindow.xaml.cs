using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AdminPanel_project.Models;
using System.Windows;

namespace AdminPanel_project.Models
{
    public partial class PointWindow : Window
    {
        private readonly string apiUrl = "http://localhost:5050/api/Point";
        private readonly HttpClient _httpClient;
        private User _currentUser;

        public PointWindow(User u)
        {
            InitializeComponent();

            _currentUser = u;
            _httpClient = new HttpClient();

            _ = LoadPointsAsync();
        }

        private async Task LoadPointsAsync()
        {
            try
            {
                var points = await _httpClient.GetFromJsonAsync<List<Point>>($"{apiUrl}/AllPoints");
                if (points != null)
                {
                    PickUpPointsGrid.ItemsSource = points;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка");
            }
        }

        private async void AddPointBt_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CityTb.Text) ||
                string.IsNullOrWhiteSpace(StreetTb.Text) ||
                string.IsNullOrWhiteSpace(HomeNumberTb.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Внимание");
                return;
            }

            if (!int.TryParse(HomeNumberTb.Text, out int homeNumber))
            {
                MessageBox.Show("Номер дома должен быть числом.", "Внимание");
                return;
            }

            var newPoint = new Point
            {
                City = CityTb.Text,
                Street = StreetTb.Text,
                HomeNumber = homeNumber
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{apiUrl}/AddPoint", newPoint);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Пункт выдачи успешно добавлен!", "Успех");

                    CityTb.Clear();
                    StreetTb.Clear();
                    HomeNumberTb.Clear();

                    await LoadPointsAsync();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка при добавлении: {errorMessage}", "Ошибка"  );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка соединения с сервером: {ex.Message}", "Ошибка");
            }
        }

        private void BackBt_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow(_currentUser);
            adminWindow.Show();
            this.Close();
        }
    }
}