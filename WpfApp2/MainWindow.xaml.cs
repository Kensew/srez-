using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        private List<WeatherData> _weatherDataList = new List<WeatherData>();

        public MainWindow()
        {
            InitializeComponent();
            AddTestRecords();
            WeatherDataGrid.ItemsSource = _weatherDataList;
        }

        private void AddTestRecords()
        {
            _weatherDataList.AddRange(new List<WeatherData>
            {
                new WeatherData { Date = new DateTime(2024, 5, 1), Temperature = 15 },
                new WeatherData { Date = new DateTime(2024, 5, 2), Temperature = 18 },
                new WeatherData { Date = new DateTime(2024, 5, 3), Temperature = 22 },
                new WeatherData { Date = new DateTime(2024, 5, 4), Temperature = 19 },
                new WeatherData { Date = new DateTime(2024, 5, 5), Temperature = 25 },
                new WeatherData { Date = new DateTime(2024, 5, 6), Temperature = 28 },
                new WeatherData { Date = new DateTime(2024, 5, 7), Temperature = 17 },
                new WeatherData { Date = new DateTime(2024, 5, 8), Temperature = 20 },
                new WeatherData { Date = new DateTime(2024, 5, 9), Temperature = 23 },
                new WeatherData { Date = new DateTime(2024, 5, 10), Temperature = 21 }
            });
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (DatePicker.SelectedDate.HasValue && double.TryParse(TemperatureTextBox.Text, out double temperature))
            {
                _weatherDataList.Add(new WeatherData
                {
                    Date = DatePicker.SelectedDate.Value,
                    Temperature = temperature
                });
                WeatherDataGrid.Items.Refresh();
                UpdateStatistics();
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter a valid date and temperature.");
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            _weatherDataList = _weatherDataList.OrderBy(data => data.Temperature).ToList();
            WeatherDataGrid.ItemsSource = _weatherDataList;
            WeatherDataGrid.Items.Refresh();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var filteredData = _weatherDataList.Where(data => data.Temperature > 20).ToList();
            WeatherDataGrid.ItemsSource = filteredData;
            WeatherDataGrid.Items.Refresh();
        }

        private void UpdateStatistics()
        {
            if (_weatherDataList.Count > 0)
            {
                double averageTemperature = _weatherDataList.Average(data => data.Temperature);
                double maxTemperature = _weatherDataList.Max(data => data.Temperature);
                double minTemperature = _weatherDataList.Min(data => data.Temperature);

                var sameTemperatureCounts = _weatherDataList.GroupBy(data => data.Temperature)
                                                            .Where(group => group.Count() > 1)
                                                            .Select(group => new { Temperature = group.Key, Count = group.Count() });

                int anomalousIncreases = 0;
                int anomalousDecreases = 0;
                for (int i = 1; i < _weatherDataList.Count; i++)
                {
                    if (_weatherDataList[i].Temperature - _weatherDataList[i - 1].Temperature >= 10)
                    {
                        anomalousIncreases++;
                    }
                    if (_weatherDataList[i - 1].Temperature - _weatherDataList[i].Temperature >= 10)
                    {
                        anomalousDecreases++;
                    }
                }

                StatisticsTextBlock.Text = $"Average Temperature: {averageTemperature:F2}°C\n" +
                                           $"Max Temperature: {maxTemperature}°C\n" +
                                           $"Min Temperature: {minTemperature}°C\n" +
                                           $"Same Temperature Occurrences: {string.Join(", ", sameTemperatureCounts.Select(g => $"{g.Temperature}°C ({g.Count} times)"))}\n" +
                                           $"Anomalous Increases: {anomalousIncreases}\n" +
                                           $"Anomalous Decreases: {anomalousDecreases}";
            }
        }

        private void SaveAsTXT(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text file (*.txt)|*.txt",
                FileName = "WeatherData.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var data in _weatherDataList)
                {
                    sb.AppendLine($"{data.Date.ToShortDateString()}: {data.Temperature}°C");
                }

                File.WriteAllText(saveFileDialog.FileName, sb.ToString());
                MessageBox.Show("Data saved successfully!");
            }
        }
    }
}