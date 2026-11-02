using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Newtonsoft.Json;
using AdminPanel_project.Models;
using static AdminPanel_project.App;

namespace AdminPanel_project
{
    /// <summary>
    /// Логика взаимодействия для AnaliticalWindow.xaml
    /// </summary>
    public partial class AnaliticalWindow : Window
    {
        private const string ApiBaseUrl = "http://localhost:5050/api/";
        private User currentUser;

        public AnaliticalWindow(User u)
        {
            InitializeComponent();
            _ = LoadAllChartsAsync();
            currentUser = u;
        }

        private async Task LoadAllChartsAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // ====================================================================
                    // 1. ЛИНЕЙНЫЙ ГРАФИК (Динамика выручки)
                    // ====================================================================
                    string ordersJson = await client.GetStringAsync($"{ApiBaseUrl}Order/AllOrders");
                    var allOrders = JsonConvert.DeserializeObject<List<Order>>(ordersJson);

                    if (allOrders != null && allOrders.Any())
                    {
                        var revenueData = allOrders
                            .Where(o => o.TotalPrice > 0)
                            .GroupBy(o => o.OrderDate.ToString("dd.MM"))
                            .Select(g => new
                            {
                                Date = g.Key,
                                Amount = g.Sum(o => o.TotalPrice)
                            })
                            .OrderBy(g => g.Date)
                            .ToList();

                        if (revenueData.Any())
                        {
                            var revenueValues = revenueData.Select(x => (double)x.Amount).ToList();
                            var revenueLabels = revenueData.Select(x => x.Date).ToList();

                            RevenueLineChart.Series = new ISeries[]
                            {
                                new LineSeries<double>
                                {
                                    Values = revenueValues,
                                    GeometrySize = 10,
                                    Stroke = new SolidColorPaint(SKColors.DodgerBlue, 3),
                                    Fill = new SolidColorPaint(SKColors.DodgerBlue.WithAlpha(30))
                                }
                            };

                            RevenueLineChart.XAxes = new Axis[]
                            {
                                new Axis
                                {
                                    Labels = revenueLabels,
                                    LabelsRotation = 15
                                }
                            };
                        }
                    }

                    // ====================================================================
                    // 2. КРУГОВАЯ ДИАГРАММА И 3. ТОП-5 ТОВАРОВ
                    // ====================================================================
                    string itemsJson = await client.GetStringAsync($"{ApiBaseUrl}OrderItems/AllItems");
                    var allItems = JsonConvert.DeserializeObject<List<OrderItem>>(itemsJson);

                    string tovarsJson = await client.GetStringAsync($"{ApiBaseUrl}Tovar/AllProducts");
                    var allTovars = JsonConvert.DeserializeObject<List<Tovar>>(tovarsJson);

                    string categoriesJson = await client.GetStringAsync($"{ApiBaseUrl}Category/AllCategories");
                    var allCategories = JsonConvert.DeserializeObject<List<Category>>(categoriesJson);

                    if (allItems != null && allTovars != null && allCategories != null && allItems.Any())
                    {
                        // --- КРУГОВАЯ ДИАГРАММА ---
                        var categoriesData = allItems
                            .Join(allTovars, item => item.IdTovar, tovar => tovar.Id, (item, tovar) => new { tovar.IdCategory, item.Quantity })
                            .GroupBy(x => x.IdCategory)
                            .Select(g => new { IdCategory = g.Key, TotalCount = g.Sum(x => x.Quantity) })
                            .Join(allCategories, catData => catData.IdCategory, cat => cat.Id, (catData, cat) => new
                            {
                                CategoryName = cat.Title,
                                TotalCount = catData.TotalCount
                            })
                            .ToList();

                        var pieSeriesCollection = new List<ISeries>();
                        foreach (var cat in categoriesData)
                        {
                            pieSeriesCollection.Add(new PieSeries<int>
                            {
                                Name = cat.CategoryName,
                                Values = new int[] { cat.TotalCount },
                                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                                DataLabelsSize = 14
                            });
                        }
                        CategoriesPieChart.Series = pieSeriesCollection;

                        // --- СТОЛБЧАТЫЙ ГРАФИК ---
                        var topProductsData = allItems
                            .GroupBy(item => item.IdTovar)
                            .Select(g => new { IdTovar = g.Key, TotalSold = g.Sum(item => item.Quantity) })
                            .Join(allTovars, itemData => itemData.IdTovar, tovar => tovar.Id, (itemData, tovar) => new
                            {
                                ProductTitle = tovar.Title,
                                TotalSold = itemData.TotalSold
                            })
                            .OrderByDescending(p => p.TotalSold)
                            .Take(5)
                            .ToList();

                        var soldValues = topProductsData.Select(x => x.TotalSold).Reverse().ToList();
                        var productTitles = topProductsData.Select(x => x.ProductTitle).Reverse().ToList();

                        TopProductsChart.Series = new ISeries[]
                        {
                            new RowSeries<int>
                            {
                                Values = soldValues,
                                Fill = new SolidColorPaint(SKColors.MediumSeaGreen),
                                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.End,
                                DataLabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                                DataLabelsSize = 12
                            }
                        };

                        TopProductsChart.YAxes = new Axis[]
                        {
                            new Axis { Labels = productTitles }
                        };
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки графиков:\n{ex.Message}", "Ошибка");
                }
            }
        }

        private void BackBt_Click(object sender, RoutedEventArgs e)
        {
            var admin = new AdminWindow(currentUser);
            admin.Show();
            Close();
        }
    }
}