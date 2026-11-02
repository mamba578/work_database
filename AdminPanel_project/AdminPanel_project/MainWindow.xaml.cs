using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AdminPanel_project.Models;

namespace AdminPanel_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Autotb_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTb.Text.Trim();

            string password = (PasswordPb.Visibility == Visibility.Visible)
                ? PasswordPb.Password.Trim()
                : PasswordVisibleTb.Text.Trim();

            if (string.IsNullOrEmpty(login) || login == "Введите логин" || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string url = $"http://localhost:5050/api/User/Authorization?login={login}&password={password}";

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    User currentUser = JsonConvert.DeserializeObject<User>(jsonResponse);
                    if (currentUser != null)
                    {
                        if(currentUser.Role == "Администратор")
                        {
                            MessageBox.Show("Добро пожаловать");
                            AdminWindow adminWindow = new AdminWindow(currentUser);
                            adminWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Отказано в доступе");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void loginTb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (LoginTb.Text == "Введите логин")
            {
                LoginTb.Text = "";
            }
        }

        private void loginTb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTb.Text))
            {
                LoginTb.Text = "Введите логин";
            }
        }

        private void passwordPb_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordVisibleTb.Visibility == Visibility.Visible)
            {
                PasswordVisibleTb.Text = PasswordPb.Password;
            }

            if (PasswordPb.Password.Length > 0)
            {
                PasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PasswordVisibleTb.Text = PasswordPb.Password;

            PasswordPb.Visibility = Visibility.Collapsed;
            PasswordVisibleTb.Visibility = Visibility.Visible;
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordPb.Password = PasswordVisibleTb.Text;

            PasswordVisibleTb.Visibility = Visibility.Collapsed;
            PasswordPb.Visibility = Visibility.Visible;
        }

        private void passwordVisibleTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PasswordPb.Visibility == Visibility.Visible)
            {
                PasswordPb.Password = PasswordVisibleTb.Text;
            }

            if (PasswordVisibleTb.Text.Length > 0)
            {
                PasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void LoginTb_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTb.Text))
            {
                LoginTb.Text = "Логин";
                LoginTb.Foreground = Brushes.LightGray;
            }
        }

        private void LoginTb_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(LoginTb.Text == "Логин")
            {
                LoginTb.Text = "";
                LoginTb.Foreground = Brushes.Black;
            }
        }
    }
}