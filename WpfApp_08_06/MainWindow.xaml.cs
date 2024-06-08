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
            fibonacciListBox.Items.Clear();

            int limit;
            if (!int.TryParse(limitTextBox.Text, out limit) || limit < 0)
            {
                MessageBox.Show("Please enter a valid non-negative integer for the limit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await Task.Run(() =>
            {
                var fibonacciSequence = GenerateFibonacciSequence(limit);
                foreach (var number in fibonacciSequence)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        fibonacciListBox.Items.Add(number);
                    });
                }
            });
        }

        private List<BigInteger> GenerateFibonacciSequence(int limit)
        {
            List<BigInteger> sequence = new List<BigInteger>();
            BigInteger a = 0;
            BigInteger b = 1;
            sequence.Add(a);
            while (b <= limit)
            {
                sequence.Add(b);
                BigInteger temp = a + b;
                a = b;
                b = temp;
            }
            return sequence;
        }
    }
}