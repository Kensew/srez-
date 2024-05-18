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

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        private List<WeatherData> _weatherDataList = new List<WeatherData>();

        public MainWindow()
        {
            InitializeComponent();
            WeatherDataGrid.ItemsSource = _weatherDataList;
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
            // Implement your filter logic here.
            // For example, filter by temperature > 20 degrees.
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

                // Calculate number of same temperature occurrences.
                var sameTemperatureCounts = _weatherDataList.GroupBy(data => data.Temperature)
                                                            .Where(group => group.Count() > 1)
                                                            .Select(group => new { Temperature = group.Key, Count = group.Count() });

                // Calculate anomalous temperature changes.
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

                // Update the statistics text block.
                StatisticsTextBlock.Text = $"Average Temperature: {averageTemperature:F2}°C\n" +
                                           $"Max Temperature: {maxTemperature}°C\n" +
                                           $"Min Temperature: {minTemperature}°C\n" +
                                           $"Same Temperature Occurrences: {string.Join(", ", sameTemperatureCounts.Select(g => $"{g.Temperature}°C ({g.Count} times)"))}\n" +
                                           $"Anomalous Increases: {anomalousIncreases}\n" +
                                           $"Anomalous Decreases: {anomalousDecreases}";
            }
        }

        private void WeatherDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}