using System.Numerics;
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
            if (int.TryParse(numberTextBox.Text, out int number))
            {
                if (number < 0)
                {
                    resultLabel.Content = "Number must be non-negative.";
                    return;
                }

                try
                {
                    BigInteger factorial = await CalculateFactorialAsync(number);
                    resultLabel.Content = $"Factorial of {number} is {factorial}";
                }
                catch (Exception ex)
                {
                    resultLabel.Content = $"Error: {ex.Message}";
                }
            }
            else
            {
                resultLabel.Content = "Invalid number.";
            }
        }

        private Task<BigInteger> CalculateFactorialAsync(int n)
        {
            return Task.Run(() =>
            {
                BigInteger result = 1;
                for (int i = 2; i <= n; i++)
                {
                    result *= i;
                }
                return result;
            });
        }
    }
}