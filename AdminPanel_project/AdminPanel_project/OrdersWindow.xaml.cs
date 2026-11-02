using AdminPanel_project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static AdminPanel_project.App;
using AdminPanel_project.Models;

namespace AdminPanel_project
{
    /// <summary>
    /// Логика взаимодействия для OrdersWindow.xaml
    /// </summary>
    public partial class OrdersWindow : Window
    {
        private List<Order> orders = new List<Order>();
        private User currentUser;

        public OrdersWindow(User u)
        {
            InitializeComponent();
            currentUser = u;
            LoadOrders();
        }

        private async void LoadOrders()
        {
            try
            {
                string json =
                    await client.GetStringAsync(
                        "http://localhost:5050/api/Order/AllOrders");

                orders =
                    JsonConvert.DeserializeObject<List<Order>>(json);

                OrdersGrid.ItemsSource =
                    orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void UpdateStatusBt_Click(object sender, RoutedEventArgs e)
        {
            Order order = OrdersGrid.SelectedItem as Order;

            if (order == null)
            {
                MessageBox.Show("Выберите заказ");
                return;
            }

            if (StatusCb.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус");
                return;
            }

            ComboBoxItem item = StatusCb.SelectedItem as ComboBoxItem;
            string status = item.Content.ToString();

            int newStatusId = order.IdStatus;

            switch (status)
            {
                case "Новый":
                    newStatusId = 1;
                    break;
                case "В обработке":
                    newStatusId = 2;
                    break;
                case "Передан в доставку":
                    newStatusId = 3;
                    break;
                case "Завершен":
                    newStatusId = 4;
                    break;
                case "Отменен":
                    newStatusId = 5;
                    break;
            }

            StringContent content = new StringContent("", Encoding.UTF8, "application/json");

            string url = $"http://localhost:5050/api/Order/UpdateStatus/{order.Id}?newStatusId={newStatusId}&idPoint={order.IdPoint}&paymentType={order.PaymentType}";

            try
            {
                HttpResponseMessage response = await client.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Статус обновлен");
                    LoadOrders();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка сервера: {error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сетевая ошибка: {ex.Message}");
            }
        }

        private async void DeleteBt_Click(object sender, RoutedEventArgs e)
        {
            Order order = OrdersGrid.SelectedItem as Order;

            if (order == null)
            {
                MessageBox.Show("Выберите заказ");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Отменить заказ?", "Подтверждение", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;

            if (order.IdStatus == 4)
            {
                MessageBox.Show("Нельзя отменить уже завершенный заказ");
                return;
            }
            if (order.IdStatus == 5)
            {
                MessageBox.Show("Заказ уже отменен");
                return;
            }

            int cancelStatusId = 5;

            string url = $"http://localhost:5050/api/Order/UpdateStatus/{order.Id}?newStatusId={cancelStatusId}&idPoint={order.IdPoint}&paymentType={order.PaymentType}";
            StringContent content = new StringContent("", Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Заказ отменен");
                    LoadOrders();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка сервера: {error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сетевая ошибка: {ex.Message}");
            }
        }


        private void BackBt_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow admin =
                new AdminWindow(currentUser);

            admin.Show();

            Close();
        }
    }
}