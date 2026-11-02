using AdminPanel_project.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static AdminPanel_project.App;

namespace AdminPanel_project
{
    public partial class AddEditTovarWindow : Window
    {
        private Tovar editProduct = null;
        private string selectedLocalPhotoPath = null;

        public AddEditTovarWindow(Tovar? t)
        {
            InitializeComponent();
            editProduct = t;
            InitializeDataAsync();
        }

        private async void InitializeDataAsync()
        {
            await LoadDataCb();
            LoadProduct();
        }

        private async Task LoadDataCb()
        {
            try
            {
                string categoryUrl = "http://localhost:5050/api/Category/AllCategories";
                string categoryJson = await client.GetStringAsync(categoryUrl);
                var allCategories = JsonConvert.DeserializeObject<List<Category>>(categoryJson);

                string brandUrl = "http://localhost:5050/api/Brand/AllBrands";
                string brandJson = await client.GetStringAsync(brandUrl);
                var allBrands = JsonConvert.DeserializeObject<List<Brand>>(brandJson);

                BrandCb.ItemsSource = allBrands;
                BrandCb.DisplayMemberPath = "Title";
                BrandCb.SelectedValuePath = "Id";

                CategoryCb.ItemsSource = allCategories;
                CategoryCb.DisplayMemberPath = "Title";
                CategoryCb.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void LoadProduct()
        {
            if (editProduct == null)
            {
                if (CategoryCb.Items.Count > 0) CategoryCb.SelectedIndex = 0;
                if (BrandCb.Items.Count > 0) BrandCb.SelectedIndex = 0;
                return;
            }

            TitleTb.Text = "Редактирование товара";
            NameTb.Text = editProduct.Title;
            PriceTb.Text = editProduct.Price.ToString();
            MaterialTb.Text = editProduct.Description;
            QuantityTb.Text = editProduct.Quantity.ToString();

            CategoryCb.SelectedValue = editProduct.IdCategory;
            BrandCb.SelectedValue = editProduct.IdBrand;

            if (!string.IsNullOrWhiteSpace(editProduct.Photo))
            {
                try
                {
                    ProductImage.Source = new BitmapImage(new Uri(editProduct.Photo, UriKind.Absolute));
                }
                catch { }
            }
        }

        private void ShowAddCategoryBt_Click(object sender, RoutedEventArgs e)
        {
            CategoryCb.Visibility = Visibility.Collapsed;
            ShowAddCategoryBt.Visibility = Visibility.Collapsed;

            NewCategoryTb.Visibility = Visibility.Visible;
            SaveCategoryBt.Visibility = Visibility.Visible;
            CancelCategoryBt.Visibility = Visibility.Visible;
        }

        private void CancelCategoryBt_Click(object sender, RoutedEventArgs e)
        {
            NewCategoryTb.Text = "";
            NewCategoryTb.Visibility = Visibility.Collapsed;
            SaveCategoryBt.Visibility = Visibility.Collapsed;
            CancelCategoryBt.Visibility = Visibility.Collapsed;

            CategoryCb.Visibility = Visibility.Visible;
            ShowAddCategoryBt.Visibility = Visibility.Visible;
        }

        private async void SaveCategoryBt_Click(object sender, RoutedEventArgs e)
        {
            string newTitle = NewCategoryTb.Text.Trim();
            if (string.IsNullOrWhiteSpace(newTitle)) return;

            Category c = new Category { Title = newTitle };
            var json = JsonConvert.SerializeObject(c);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:5050/api/Category/AddCategory", content);
                if (response.IsSuccessStatusCode)
                {
                    CancelCategoryBt_Click(null, null);
                    await LoadDataCb();

                    foreach (Category item in CategoryCb.Items)
                    {
                        if (item.Title == newTitle)
                        {
                            CategoryCb.SelectedItem = item;
                            break;
                        }
                    }
                }
                else
                {
                    string err = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(err);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowAddBrandBt_Click(object sender, RoutedEventArgs e)
        {
            BrandCb.Visibility = Visibility.Collapsed;
            ShowAddBrandBt.Visibility = Visibility.Collapsed;

            NewBrandTb.Visibility = Visibility.Visible;
            SaveBrandBt.Visibility = Visibility.Visible;
            CancelBrandBt.Visibility = Visibility.Visible;
        }

        private void CancelBrandBt_Click(object sender, RoutedEventArgs e)
        {
            NewBrandTb.Text = "";
            NewBrandTb.Visibility = Visibility.Collapsed;
            SaveBrandBt.Visibility = Visibility.Collapsed;
            CancelBrandBt.Visibility = Visibility.Collapsed;

            BrandCb.Visibility = Visibility.Visible;
            ShowAddBrandBt.Visibility = Visibility.Visible;
        }

        private async void SaveBrandBt_Click(object sender, RoutedEventArgs e)
        {
            string newTitle = NewBrandTb.Text.Trim();
            if (string.IsNullOrWhiteSpace(newTitle)) return;

            Brand b = new Brand { Title = newTitle };
            var json = JsonConvert.SerializeObject(b);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:5050/api/Brand/AddBrand", content);
                if (response.IsSuccessStatusCode)
                {
                    CancelBrandBt_Click(null, null);
                    await LoadDataCb();

                    foreach (Brand item in BrandCb.Items)
                    {
                        if (item.Title == newTitle)
                        {
                            BrandCb.SelectedItem = item;
                            break;
                        }
                    }
                }
                else
                {
                    string err = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(err);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task<string> UploadPhotoToServer(string localPath)
        {
            using (var formData = new MultipartFormDataContent())
            {
                var fileBytes = File.ReadAllBytes(localPath);
                var imageContent = new ByteArrayContent(fileBytes);

                formData.Add(imageContent, "file", Path.GetFileName(localPath));

                var response = await client.PostAsync("http://localhost:5050/api/Tovar/UploadPhoto", formData);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                    return result.fileName.ToString();
                }
                else
                {
                    throw new Exception("Не удалось загрузить фотографию на сервер API.");
                }
            }
        }

        private void SelectImageBt_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files|*.png;*.jpg;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                selectedLocalPhotoPath = dialog.FileName;
                ProductImage.Source = new BitmapImage(new Uri(selectedLocalPhotoPath, UriKind.Absolute));
            }
        }

        private async void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTb.Text))
            {
                MessageBox.Show("Введите название товара");
                return;
            }
            if (CategoryCb.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию");
                return;
            }
            if (!int.TryParse(PriceTb.Text.Trim(), out int price))
            {
                MessageBox.Show("Цена должна быть числом");
                return;
            }
            if (!int.TryParse(QuantityTb.Text.Trim(), out int quantity))
            {
                MessageBox.Show("Количество должно быть числом");
                return;
            }

            string finalPhotoName = null;

            if (!string.IsNullOrEmpty(selectedLocalPhotoPath))
            {
                try
                {
                    finalPhotoName = await UploadPhotoToServer(selectedLocalPhotoPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            else if (editProduct != null && !string.IsNullOrEmpty(editProduct.Photo))
            {
                finalPhotoName = editProduct.Photo.Split('/').Last();
            }

            Tovar product = new Tovar
            {
                Article = "ART-" + new Random().Next(1000, 9999).ToString(),

                Title = NameTb.Text.Trim(),
                Price = price,
                IdCategory = (int)CategoryCb.SelectedValue,
                IdBrand = (int)BrandCb.SelectedValue,
                Description = MaterialTb.Text.Trim(),
                Quantity = quantity,
                Status =  "Активный",
                Photo = finalPhotoName,

                IdBrandNavigation = new Brand { Title = "temp" },
                IdCategoryNavigation = new Category { Title = "temp" }
            };

            if (editProduct != null)
            {
                product.Id = editProduct.Id;
            }

            var settings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() };
            string json = JsonConvert.SerializeObject(product, settings);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response;

                if (editProduct == null)
                {
                    response = await client.PostAsync("http://localhost:5050/api/Tovar/AddProduct", content);
                }
                else
                {
                    response = await client.PutAsync($"http://localhost:5050/api/Tovar/UpdateProduct/{product.Id}", content);
                }

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Товар успешно сохранен");
                    DialogResult = true;
                    Close();
                }
                else
                {
                    string serverError = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка сервера:\n{serverError}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения:\n{ex.Message}");
            }
        }
    }
}