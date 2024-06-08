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

namespace WpfApp_08_06
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(numberTextBox.Text, out double number) &&
                double.TryParse(powerTextBox.Text, out double power))
            {
                try
                {
                    double result = await CalculatePowerAsync(number, power);
                    resultLabel.Content = $"{number} raised to the power of {power} is {result}";
                }
                catch (Exception ex)
                {
                    resultLabel.Content = $"Error: {ex.Message}";
                }
            }
            else
            {
                resultLabel.Content = "Invalid input.";
            }
        }

        private Task<double> CalculatePowerAsync(double number, double power)
        {
            return Task.Run(() =>
            {
                return Math.Pow(number, power);
            });
        }
    }
}