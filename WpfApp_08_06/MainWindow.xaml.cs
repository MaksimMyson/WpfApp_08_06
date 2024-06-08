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
        private CancellationTokenSource cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int numBars;
            if (!int.TryParse(numBarsTextBox.Text, out numBars) || numBars <= 0)
            {
                MessageBox.Show("Please enter a valid positive integer for the number of progress bars.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();

            progressBarsContainer.Items.Clear();
            for (int i = 0; i < numBars; i++)
            {
                ProgressBar progressBar = new ProgressBar();
                progressBarsContainer.Items.Add(progressBar);

                Task.Run(() => Dance(progressBar, cancellationTokenSource.Token));
            }
        }

        private async Task Dance(ProgressBar progressBar, CancellationToken cancellationToken)
        {
            Random random = new Random();
            while (!cancellationToken.IsCancellationRequested)
            {
                int progress = random.Next(101);
                SolidColorBrush brush = new SolidColorBrush(Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)));

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    progressBar.Value = progress;
                    progressBar.Foreground = brush;
                });

                await Task.Delay(200);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            cancellationTokenSource?.Cancel();
        }
    }
}